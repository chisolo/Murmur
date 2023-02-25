using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lemegeton
{
    [RequireComponent(typeof(Camera))]
    public class CameraCtrl : MonoBehaviour
    {
        //相机跟随
        private Transform _lookAtTarget;
        
        //UI检测
        private bool _clickUI = false;
        private List<RaycastResult> _raycastResults;
        private PointerEventData _pointerEventData;
        
        //双指缩放
        private float _doubleTouchCurrDis;
        private bool _isZoom;
        private float _doubleTouchLastDis;
        
        //视距
        private float _maxDistance;
        private float _minDistance;
        private float _defaultDistance;
        
        //边界
        private Vector3 _minPosition;
        private Vector3 _maxPosition;
        private Vector3 _projMinPosition;
        private Vector3 _projMaxPosition;
        
        private Vector3 _offset = Vector3.zero;
        private Vector3 _initPosition = Vector3.zero;
        private Camera _camera;
        private Vector3 _inertia = Vector3.zero;
        // 高度距离
        private float _heightDistance;
        // 观察距离
        private double _forwardDistance = 0;
        // 相机sin值
        private double _cameraSin;
        // 逻辑位置
        private Vector3 _logicPosition;
        // 创建位置
        private Vector3 _spawnPosition = Vector3.zero;
        // 相机动画
        private Sequence _cameraAction;
        // 最后一次大小
        private float _lastOrthographicSize;
        // 是否锁住
        private bool _lock;

        // 设置默认坐标
        public void SetDefaultPos(Vector3 position)
        {
            _heightDistance = position.y;
            MoveCameraTo(position);
        }
        // 设置边界范围
        public void SetBoundary(Vector3 center, Vector3 size)
        {
            var halfSize = size / 2;
            _minPosition = center - halfSize;
            _maxPosition = center + halfSize;
            _lock = false;
        }

        // 移动相机到指定位置
        public void MoveCameraTo(Vector3 position)
        {
            if (_camera == null)
            {
                _spawnPosition = position;
            }
            else
            {
                transform.DOMove(position, 0.5f).SetUpdate(true);
            }
        }
        public void MoveCameraTo(Vector3 position, float size)
        {
            if (_lock) return;
            if (_cameraAction != null) {
                _cameraAction.Kill();
                _cameraAction = null;
            }
            _cameraAction = DOTween.Sequence();
            _cameraAction.Insert(0, transform.DOMove(position, 0.5f));
            _cameraAction.Insert(0, _camera.DOOrthoSize(size, 0.5f));
            _cameraAction.onComplete = () => {
                _cameraAction.Kill();
                _cameraAction = null;
            };
            _cameraAction.SetUpdate(true);
        }
        // 锁定相机
        public void LockCamera(Vector3 position, float size)
        {
            if (_lock) return;
            if (_cameraAction != null) {
                _cameraAction.Kill();
                _cameraAction = null;
            }
            _lastOrthographicSize = _camera.orthographicSize;
            _lock = true;
            _cameraAction = DOTween.Sequence();
            _cameraAction.Insert(0, transform.DOMove(position, 0.5f));
            _cameraAction.Insert(0, _camera.DOOrthoSize(size, 0.5f));
            _cameraAction.onComplete = () => {
                _cameraAction.Kill();
                _cameraAction = null;
            };
            _cameraAction.SetUpdate(true);
        }
        
        // 取消锁定
        public void CancelLock()
        {
            if (!_lock) return;
            _lock = false;
            if (_cameraAction != null) {
                _cameraAction.Kill();
                _cameraAction = null;
            }
            _cameraAction = DOTween.Sequence();
            _cameraAction.Insert(0, _camera.DOOrthoSize(_lastOrthographicSize, 0.5f));
            _cameraAction.onComplete = () => {
                _cameraAction.Kill();
                _cameraAction = null;
            };
            _cameraAction.SetUpdate(true);
        }
        
        // 设置相机跟随对象
        public void SetLookAtTarget(Transform target)
        {
            _lookAtTarget = target;
        }
        
        // 设置视距
        public void SetViewDistance(float min, float defaultValue, float max)
        {
            _maxDistance = max;
            _minDistance = min;
            _defaultDistance = defaultValue;
        }
        
        private void Start()
        {
            _camera = GetComponent<Camera>();
            _raycastResults = new List<RaycastResult>();
            _pointerEventData = new PointerEventData(EventSystem.current);
            _cameraSin = Math.Sin((transform.localEulerAngles.x) * math.PI / 180);
            _forwardDistance = _heightDistance / _cameraSin;
            _camera.orthographicSize = _defaultDistance;
            transform.position = _spawnPosition;
            _logicPosition = transform.position;
            UpdateProjBoundary();
        }
        
        private void Update()
        {
            if (_lock)
            {
                return;
            }
            
            if (_lookAtTarget != null)
            {
                var position = transform.position;
                var targetPos = _lookAtTarget.position + (-transform.forward * (float) _forwardDistance);
                var newPosition = Vector3.Slerp(position, targetPos, 0.1f);
                OnBoundaryCheck(ref newPosition);
                _logicPosition = newPosition;
                var forwardDistance = _logicPosition.y /_cameraSin;
                var length = _forwardDistance - forwardDistance;
                var moveLength = transform.forward * (float) length;
                transform.position = _logicPosition - moveLength;
                UpdateProjBoundary();
                return;
            }
            
    #if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                _logicPosition = transform.position;
                _inertia = Vector3.zero;
                _initPosition = Input.mousePosition;
                _initPosition = _camera.ScreenToWorldPoint(_initPosition);
                _raycastResults.Clear();
                _pointerEventData.position = Input.mousePosition;
                if (EventSystem.current == null)
                {
                    _clickUI = true;
                }
                else
                {
                    EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
                    if (_raycastResults.Count >= 1 && _raycastResults[0].gameObject.layer == LemegetonConst.UILayer)
                    {
                        _clickUI = true;
                    }
                    else
                    {
                        _clickUI = false;
                    }
                }
            }
            
            if (!_clickUI && Input.GetMouseButton(0))
            {
                var currentPos = Input.mousePosition;
                transform.position = _logicPosition;
                currentPos = _camera.ScreenToWorldPoint(currentPos);
                _inertia = _initPosition - currentPos;
                var position = transform.position;
                var targetPosition = position + _inertia;
                var newPosition = Vector3.Slerp(position, targetPosition, 0.1f);
                OnBoundaryCheck(ref newPosition);
                _logicPosition = newPosition;
                var forwardDistance = _logicPosition.y /_cameraSin;
                var length = _forwardDistance - forwardDistance;
                var moveLength = transform.forward * (float) length;
                transform.position = _logicPosition - moveLength;
                UpdateProjBoundary();
            }
            
            var scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollWheel) > 0.001)
            {
                var orthographicSize = _camera.orthographicSize;
                _camera.orthographicSize = orthographicSize - scrollWheel;
                if (_camera.orthographicSize < _minDistance)
                {
                    _camera.orthographicSize = _minDistance;
                }
                if (_camera.orthographicSize > _maxDistance)
                {
                    _camera.orthographicSize = _maxDistance;
                }
                UpdateProjBoundary();
            }
            
    #else
            if (Input.touchCount == 1)
            {
                TouchMoveHandle();
            }
            else if (Input.touchCount == 2)
            {
                TouchScaleHandle();
            }
            else if (Input.touchCount == 0)
            {
                _isZoom = false;
            }
    #endif
            
            if (_inertia.sqrMagnitude > 0.05f)
            {
                var position = transform.position;
                var targetPos = position + _inertia;
                var newPosition = Vector3.Slerp(position, targetPos, 0.1f);
                OnBoundaryCheck(ref newPosition);
                _logicPosition = newPosition;
                var forwardDistance = _logicPosition.y /_cameraSin;
                var length = _forwardDistance - forwardDistance;
                var moveLength = transform.forward * (float) length;
                transform.position = _logicPosition - moveLength;
                UpdateProjBoundary();
                _inertia *= 0.97f;
            }
        }
        
        // 触摸移动处理
        private void TouchMoveHandle()
        {
            if (_isZoom)
            {
                return;
            }
            Vector3 inputPosition = Input.GetTouch(0).position;
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _logicPosition = transform.position;
                _inertia = Vector3.zero;
                _initPosition = inputPosition;
                _initPosition = _camera.ScreenToWorldPoint(_initPosition);
                
                _raycastResults.Clear();
                _pointerEventData.position = inputPosition;
                if (EventSystem.current == null)
                {
                    _clickUI = true;
                }
                else
                {
                    EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
                    if (_raycastResults.Count >= 1 && _raycastResults[0].gameObject.layer == LemegetonConst.UILayer)
                    {
                        _clickUI = true;
                    }
                    else
                    {
                        _clickUI = false;
                    }
                }
            }
            
            if (_clickUI || Input.GetTouch(0).phase != TouchPhase.Moved) return;
            var currentPos = inputPosition;
            transform.position = _logicPosition;
            currentPos = _camera.ScreenToWorldPoint(currentPos);
            _inertia = _initPosition - currentPos;
            var position = transform.position;
            var targetPos = position + _inertia;
            var newPosition = Vector3.Slerp(position, targetPos, 0.1f);
            OnBoundaryCheck(ref newPosition);
            _logicPosition = newPosition;
            var forwardDistance = _logicPosition.y /_cameraSin;
            var length = _forwardDistance - forwardDistance;
            var moveLength = transform.forward * (float) length;
            transform.position = _logicPosition - moveLength;
            UpdateProjBoundary();
        }
        
        // 触摸缩放处理
        private void TouchScaleHandle()
        {
            if (_clickUI || (Input.GetTouch(0).phase != TouchPhase.Moved && Input.GetTouch(1).phase != TouchPhase.Moved)) return;
            var touch1 = Input.GetTouch(0);
            var touch2 = Input.GetTouch(1);
            _doubleTouchCurrDis = Vector2.Distance(touch1.position, touch2.position);
            if (!_isZoom)
            {
                _doubleTouchLastDis = _doubleTouchCurrDis;
                _isZoom = true;
            }
            var distance = _doubleTouchCurrDis - _doubleTouchLastDis;
            var orthographicSize = _camera.orthographicSize;
            var target = orthographicSize - (distance / 10);
            _camera.orthographicSize = Mathf.Lerp(orthographicSize, target, 0.1f);
            if (_camera.orthographicSize < _minDistance)
            {
                _camera.orthographicSize = _minDistance;
            }
            if (_camera.orthographicSize > _maxDistance)
            {
                _camera.orthographicSize = _maxDistance;
            }
            _doubleTouchLastDis = _doubleTouchCurrDis;
            UpdateProjBoundary();
        }
        
        // 更新投影边界
        private void UpdateProjBoundary()
        {
            UpdateOffset();
            var transform1 = transform;
            var forward = transform1.forward;
            var t = Vector3.Dot(Vector3.up, transform1.position - _minPosition) /
                    Vector3.Dot(Vector3.up, -forward);
            _projMinPosition = (_minPosition + t * -forward) + _offset;
            _projMaxPosition = (_maxPosition + t * -forward) - _offset;
        }
        
        // 更新近平面的偏移值
        private void UpdateOffset()
        {
            var orthographicSize = _camera.orthographicSize;
            var width = orthographicSize * ((float)Screen.width / Screen.height);
            var offset = transform.right * width + transform.up * orthographicSize;
            var forward = transform.forward;
            
            var t = Vector3.Dot(Vector3.up, -offset) / Vector3.Dot(Vector3.up, -forward);
            var rightUpOffset = (-forward * t) + offset;
            
            var offset1 = -transform.right * width + transform.up * orthographicSize;
            var t1 = Vector3.Dot(Vector3.up, -offset1) / Vector3.Dot(Vector3.up, -forward);
            var leftUpOffset = (-forward * t1) + offset1;
            
            _offset.x = Mathf.Max(Math.Abs(rightUpOffset.x), Math.Abs(leftUpOffset.x));
            _offset.z = Mathf.Max(Math.Abs(rightUpOffset.z), Math.Abs(leftUpOffset.z));
            _offset.y = 0;
        }
        
        /// 边界检查(每次更新摄像机位置的函数)
        private void OnBoundaryCheck(ref Vector3 newPosition)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, _projMinPosition.x, _projMaxPosition.x);
            newPosition.z = Mathf.Clamp(newPosition.z, _projMinPosition.z, _projMaxPosition.z);
        }
    }
}