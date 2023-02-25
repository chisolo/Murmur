using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lemegeton
{
    public class CenterOnChild : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        public enum Direction
        {
            Horizontal,
            // TODO
            //Vertical
        }

        public RectTransform Content => _content;

        public UnityEvent<int> OnFinished;

        public int padding;
        public int spacing;

        [SerializeField]
        private Direction _direction = Direction.Horizontal;

        private ScrollRect _scrollView;
        private int _pageCount;
        private RectTransform _content;
        private float[] _pages;
        private bool _moving = false;
        private bool _dragging = false;

        private Sequence _action;

        private int _idTarget;
        private Vector2 _currentPos;
        private float _childWidth;
        private int _childCount;

        private void Awake()
        {
            _scrollView = GetComponent<ScrollRect>();
            _content = _scrollView.content;
        }

        public void RefreshView()
        {
            TaskModule.Instance.NextFrame(() => {
                _idTarget = 0;
                var layout = _content.GetComponent<HorizontalLayoutGroup>();
                layout.padding.left = padding;
                layout.padding.right = padding;
                layout.spacing = spacing;

                var childCount = _content.childCount;
                var autoScroll = _pageCount != childCount;
                _pageCount = childCount;

                _pages = new float[_pageCount];
                if (childCount == 0) {
                    return;
                }
                var child = _content.GetChild(0) as RectTransform;
                var singleWidth = child == null ? 0 : child.rect.width;
                _childWidth = singleWidth;
                _childCount = childCount;

                _scrollView.horizontalNormalizedPosition  = 0;
                //var width = (_pageCount * singleWidth) + (padding * 2) + ((_pageCount - 1) * spacing);
                // var sizeDelta = _content.sizeDelta;
                // var width = sizeDelta.x;

                //sizeDelta.x = width;
                //_content.sizeDelta = sizeDelta;


                var startPosX = _content.anchoredPosition.x;
                for (var i = 0; i < _pages.Length; i++) {
                    _pages[i] = (int) (-i * (singleWidth + spacing)) + startPosX;
                }

                // TODO: center no animation
                //LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
                //CenterOn(0, false);
            });
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
            _currentPos = _content.anchoredPosition;
            if (_action != null) {
                _action.Kill();
                _action = null;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;
            if (_action != null) {
                return;
            }

            var moveRate = 0.15f;
            if (_content.anchoredPosition.x > _currentPos.x) {
                // 0 ~ 0.1 * witdh -> self
                // 0.1 * width ~ width -> back
                // > width -> near
                var dis = _content.anchoredPosition.x - _currentPos.x;
                if (dis <= moveRate * _childWidth) {
                    CenterOn(_idTarget);
                } else if (dis <= _childWidth) {
                    CenterBack();
                } else {
                    CenterNear();
                }
            } else {
                var dis = _currentPos.x - _content.anchoredPosition.x;
                if (dis <= moveRate * _childWidth) {
                    CenterOn(_idTarget);
                } else if (dis <= _childWidth) {
                    CenterNext();
                } else {
                    CenterNear();
                }
            }

        }

        public void CenterNext() {
            if (_idTarget < _childCount - 1) {
                CenterOn(_idTarget + 1);
            } else {
                CenterOn(_idTarget);
            }
        }

        public void CenterBack() {
            if (_idTarget > 0) {
                CenterOn(_idTarget - 1);
            } else {
                CenterOn(_idTarget);
            }
        }

        private void CenterNear() {
            var minDistance = 100000.0f;
            var id = 0;
            for (var i = 0; i < _pages.Length; i++) {
                var distance = Mathf.Abs(_pages[i] - _content.anchoredPosition.x);
                if (distance < minDistance) {
                    minDistance = distance;
                    id = i;
                }
            }

            CenterOn(id);
        }

        public void CenterOn(int id, bool animation = true)
        {
            if (_action != null) {
                _action.Kill();
                _action = null;
            }

            _idTarget = id;
            var nearPosition = _pages[id];

            if (animation) {
                _action = DOTween.Sequence();
                _action.Append(_content.DOAnchorPosX(nearPosition, 0.25f).SetEase(Ease.OutBack));
                _action.OnComplete(() => {
                    _action = null;
                });
            } else {
                _content.anchoredPosition = new Vector2(nearPosition, 0);
            }

            OnFinished?.Invoke(id);
        }
    }
}