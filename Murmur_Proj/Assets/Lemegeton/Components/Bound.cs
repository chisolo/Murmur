using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class Bound : MonoBehaviour
{
    [SerializeField] float _durtion = 0.16f;
    [SerializeField] int _count = 1;
    [SerializeField] float _jumpPower = 10;
    [SerializeField] CanvasGroup _clover;
    [SerializeField] float _cloverEndAphal = 0.6f;

    private Vector3 _currentPos;

    private Sequence _action;
    private Sequence _flashAction;

    void Awake()
    {
        _currentPos = transform.localPosition;
        if (_clover != null)
        {
            _clover.gameObject.SetActive(false);
        }
    }

    public void Play()
    {
        //Debug.Log("play " + this +" : " + Time.frameCount);
        if (_action != null)
        {
            _action.Complete();
            _flashAction?.Complete();
        }

        _action = DOTween.Sequence();
        _action.Append(transform.DOLocalJump(_currentPos, 0, 1, 0.16f));
        for (int i = 0; i < _count; i++)
        {
            _action.Append(transform.DOLocalJump(_currentPos, _jumpPower, 1, _durtion));
        }
        _action.OnComplete(() =>
        {
            //Debug.Log("OnComplete "  + this +" : " + Time.frameCount);
            _action = null;
        });


        if (_clover != null)
        {
            _clover.gameObject.SetActive(true);
            _clover.alpha = 0;
            _flashAction = DOTween.Sequence();
            _flashAction.Append(_clover.DOFade(0, 0.16f));
            _flashAction.Append(_clover.DOFade(_cloverEndAphal, _durtion * 0.5f));
            _flashAction.Append(_clover.DOFade(0, _durtion * 0.5f));
            _flashAction.OnComplete(() =>
            {
                //Debug.Log("OnComplete "  + this +" : " + Time.frameCount);
                _flashAction = null;
                _clover.gameObject.SetActive(false);
            });
        }

    }

}
