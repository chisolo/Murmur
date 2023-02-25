using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class PopupUIArgs
{
    public static PopupUIArgs Empty => new PopupUIArgs();
}

public abstract class PopupUIBaseCtrl : MonoBehaviour
{

    public enum PopupType
    {
        None = 0,
        Up,
        Scale,
        Left,
        Right,
    };

    [SerializeField]
    private PopupType _popupType;

    [SerializeField]
    private float _interval = 0.4f;

    [SerializeField]
    private RectTransform _container = null;
    [SerializeField]
    private CanvasGroup _canvasGroup = null;
    [SerializeField]
    private bool _fade = true;
    public Action onOpen = null;

    public Action OnClose = null;

    private bool stop = false;

    private Vector3 _defaultScale;

    public abstract void Init(PopupUIArgs arg);

    public void Show()
    {
#if UNITY_EDITOR
        if (stop) UnityEditor.EditorApplication.isPaused = true;
#endif
        EventModule.Instance.Register(EventDefine.ForceCloseUI, OnForceCloseUIEvent);
        if (_container == null) _container = this.transform as RectTransform;
        var container = _container;

        // TODO: use Transition
        switch(_popupType) {
            case PopupType.Up:
                var startPos = -container.rect.height;
                var originPos = container.anchoredPosition.y;
                container.anchoredPosition = new Vector2(0, startPos);
                container?.DOAnchorPosY(originPos, _interval).SetEase(Ease.OutBack).OnComplete(() => {
                    onOpen?.Invoke();
                });

                FadeIn();
                break;
            case PopupType.Scale:
                _defaultScale = container.localScale;
                container.localScale = Vector3.zero;
                container?.DOScale(_defaultScale.x, _interval).SetEase(Ease.OutBack).OnComplete(() => {
                    onOpen?.Invoke();
                });;

                FadeIn();
                break;
            case PopupType.Left:
                //container.localPosition = new Vector3(DefineUtil.StandardResolution.x, 0, 0);
                //gameObject.SetActive(true);
                //container.DOLocalMoveX(0, _interval);
                break;
            case PopupType.Right:
                //container.localPosition = new Vector3(-DefineUtil.StandardResolution.x, 0, 0);
                //container.DOLocalMoveX(0, _interval);
                break;

            case PopupType.None:
            default:
                this.gameObject.SetActive(true);
                onOpen?.Invoke();
                break;
        }
    }

    public void Hide()
    {
#if UNITY_EDITOR
        if (stop) UnityEditor.EditorApplication.isPaused = true;
#endif

        if (_container == null) _container = this.transform as RectTransform;
        var container = _container;

        OnBeforeHide();

        switch(_popupType) {
            case PopupType.Up:
                container?.DOAnchorPosY(-2000, _interval).SetEase(Ease.InQuart).OnComplete(() => {
                    Close();
                });
                FadeOut();
                break;
            case PopupType.Scale:
                container?.DOScale(0.2f, _interval).SetEase(Ease.InOutCubic).OnComplete(() => {
                    Close();
                });

                FadeOut();
                break;
            case PopupType.Left:
                Close();
                // container.DOLocalMoveX(DefineUtil.StandardResolution.x, _interval).OnComplete(() => {
                //     Close();
                // });
                break;
            case PopupType.Right:
                Close();
                // container.DOLocalMoveX(-DefineUtil.StandardResolution.x, _interval).OnComplete(() => {
                //     Close();
                // });
                break;
            case PopupType.None:
            default:
                Close();
                break;
        }
    }
    public void HideForce()
    {
        Close();
    }
    private void FadeIn()
    {
        if (_fade) {
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1, _interval).SetEase(Ease.InOutSine);
        }
    }

    private void FadeOut()
    {
        if (_fade) {
            _canvasGroup.DOFade(0, _interval).SetEase(Ease.OutSine);
        }
    }

    private void Close()
    {
        OnBeforeClose();
        OnClose?.Invoke();
        OnClose = null;
        EventModule.Instance?.UnRegister(EventDefine.ForceCloseUI, OnForceCloseUIEvent);
        Destroy(gameObject);
    }

    protected virtual void OnBeforeHide()
    {

    }

    protected virtual void OnBeforeClose()
    {

    }

    public void OnClickClose()
    {
        Hide();
    }

    private void OnForceCloseUIEvent(Component sender, EventArgs args)
    {
        HideForce();
    }
}
