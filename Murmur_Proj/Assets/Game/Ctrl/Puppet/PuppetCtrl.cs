using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using Lemegeton;
using DG.Tweening;

public class PuppetCtrl : MonoBehaviour
{
    [SerializeField] NavMeshAgent _navAgent;
    [SerializeField] Animator _animator;
    [SerializeField] Renderer _render;
    [SerializeField] Transform _effectRoot;
    [SerializeField] PuppetProgressView _progressView;
    [SerializeField] PuppetEmojiView _emojiView;
    [SerializeField] List<IngredientHubView> _igredientHubViews;
    public int ID { get; set; }
    public bool Active { get; set; }
    public string Target { get; set;}
    public float Timeout { get; set; }
    public bool InLine { get; set; }
    public int WaitingIndex { get; set;}
    public NavMeshAgent NavAgent => _navAgent;
    public Animator Animator => _animator;
    public PuppetProgressView ProgressView => _progressView;
    public PuppetEmojiView EmojiView => _emojiView;
    public float Speed => _speed;
    private PuppetConfig _puppetConfig;
    private float _speed;
    private GameObject _food;
    private PuppetAction _curAction;
    private Queue<PuppetAction> _actions;
    private GameObject _cookEffect;
    private Action _jobDone;

    public void Init(int id, PuppetConfig puppet, float speed, string service)
    {
        ID = id;
        _puppetConfig = puppet;
        _speed = speed;
        _navAgent.speed = _speed;
        Target = service;
        Active = false;
        InLine = false;
        WaitingIndex = -1;
        _curAction = null;
        _actions = new Queue<PuppetAction>();
        _progressView?.Hide();
        _progressView?.SetCamera(RuntimeMgr.Instance.GetWorldCamera());
        _emojiView?.Hide();
        _emojiView?.SetCamera(RuntimeMgr.Instance.GetWorldCamera());

        if(GameUtil.ServiceFoodPrefab.TryGetValue(Target, out var foodPrefab)) {
            _food = AddressablePoolModule.Instance.Get(foodPrefab);
            _food.transform.SetParent(transform);
            _food.transform.localPosition = ConfigModule.Instance.Common().food_take_pos;
            _food.transform.localScale = Vector3.one;
            _food.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            _food.SetActive(false);
        }
        if(_puppetConfig.type == PuppetType.Guest) {
            _render.material.SetFloat("_Opacity", 0f);
            _render.material.DOFloat(1f, "_Opacity", 3f);
        }
    }

    private IEnumerator ClearCoroutine()
    {
        _render.material.DOFloat(0f, "_Opacity", 3f);
        yield return new WaitForSeconds(3f);
        Reset();
        AddressablePoolModule.Instance.Return(_puppetConfig.prefab, gameObject);
    }
    public void EnableNav(bool enable)
    {
        _navAgent.enabled = enable;
    }
    public void Clear()
    {
        if(_puppetConfig.type == PuppetType.Guest) StartCoroutine(ClearCoroutine());
        else {
            Reset();
            AddressablePoolModule.Instance.Return(_puppetConfig.prefab, gameObject);
        }
    }
    private void Reset()
    {
        _progressView?.Hide();
        _emojiView?.Hide();
        if(_food != null) {
            DestroyImmediate(_food);
            _food = null;
        }
        Active = false;
        CancelAction();
    }
    private void AddAction(PuppetAction command) 
    {
         _actions.Enqueue(command);
    }
    public void CancelAction()
    {
        if(_actions != null) {
            while(_actions.Count > 0) {
                _actions.Dequeue().Cancel();
            }
        }
        if(_curAction != null && !_curAction.finish) {
            _curAction.Cancel();
            _curAction = null;
        }
    }
    public void FixedUpdate()
    {
        if(!Active) return;
        if(_curAction == null) {
            if(_actions.Count > 0) {
                _curAction = _actions.Dequeue();
                _curAction.Execute(this);
            }
        }
        if(_curAction == null) return;

        _curAction.Tick();

        if(_curAction == null) return;

        if(_curAction.finish) {
            _curAction.Finish();
            _curAction = null;
        }
    }
    public void Move(SpotConfig spot, Action<PuppetCtrl> onComplete = null, float dist = 0)
    {
        MoveAction moveAction = MoveAction.Get(this, spot, onComplete, dist);

        CancelAction();
        AddAction(moveAction);
    }
    public void Do(string action, float duration, Action<PuppetCtrl> onComplete = null, bool showHub = false)
    {
        OperateAction operateAction = OperateAction.Get(this, action, duration, onComplete, showHub);

        CancelAction();
        AddAction(operateAction);
    }

    public void MoveDo(SpotConfig spot, string action, float duration, Action<PuppetCtrl> onComplete = null, bool showHub = false)
    {
        MoveAction moveAction = MoveAction.Get(this, spot, null, 0);
        OperateAction operateAction = OperateAction.Get(this, action, duration, onComplete, showHub);

        CancelAction();
        AddAction(moveAction);
        AddAction(operateAction);
    }

    public void MoveClear(SpotConfig spot)
    {
        MoveAction moveAction = MoveAction.Get(this, spot, (puppet) => {
            puppet.Clear();
        }, 4f);
        
        CancelAction();
        AddAction(moveAction);
    }
    public void TakeFood(bool enable)
    {
        _animator.SetBool(ActionDefine.Food, enable);
        if(enable) {
            _food.transform.localPosition = ConfigModule.Instance.Common().food_take_pos;
            _food.SetActive(true);
        } else {
            _food.SetActive(false);
        }
    }
    public void EatFood(bool enable)
    {
        if(enable) {
            _food.transform.localPosition = ConfigModule.Instance.Common().food_eat_pos;
            _food.SetActive(true);
        } else {
            _food.SetActive(false);
        }
    }
    public void SetSpeed(float speed)
    {
        if(Math.Abs(speed - _speed) < 0.001f) return;
        _speed = speed;
        _navAgent.speed = speed;
    }
    public void SetIngredients(int s, int c, int f, bool init)
    {
        if(_igredientHubViews != null && _igredientHubViews.Count > 0) {
            _igredientHubViews[0]?.SetCount(s, init);
            _igredientHubViews[1]?.SetCount(c, init);
            _igredientHubViews[2]?.SetCount(f, init);
        }
    }
    public Vector3 GetEffectRoot()
    {
        if(_effectRoot == null) return Vector3.zero;
        return _effectRoot.position;
    }
    public async void PlayCookEffect(bool show)
    {
        if(_effectRoot == null) return;
        if(show) {
            _cookEffect = await AddressablePoolModule.Instance.LoadAndGetAsyncTask(GameUtil.CookEffect);
            if (_cookEffect != null) {
                _cookEffect.transform.parent = _effectRoot;
                _cookEffect.transform.position = _effectRoot.position;
                _cookEffect.transform.rotation = Quaternion.identity;
            }
        } else {
            //if(_cookEffect == null) return;
            AddressablePoolModule.Instance.Return(GameUtil.CookEffect, _cookEffect);
            _cookEffect = null;
        }
    }
}
