using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Shake : MonoBehaviour
{
    [SerializeField] float end = 10;
    [SerializeField] float duration = 0.07f;

    [SerializeField] int _count = 2;

    [SerializeField] bool _loop;

    [SerializeField] bool _autoStart;
    [SerializeField] float _autoStartDelay;

    Sequence _action;

    void Start()
    {
        if (_autoStart) {
            Play();
            if (_autoStartDelay > 0) {
                _action.PrependInterval(_autoStartDelay);
            }
        }
    }

    void OnDestroy()
    {
        //Debug.Log("OnDestroy");
        if (_action != null) _action.Kill();
    }


    public void Play()
    {
        if (_action != null) _action.Kill();

        _action = DOTween.Sequence();

        var left = new Vector3(0, 0, end);
        var right = new Vector3(0, 0, -end);

        _action.Append(transform.DOLocalRotate(left, duration));
        _action.Append(transform.DOLocalRotate(right, duration * 2));

        for (int i = 0; i < _count - 1; i++)
        {
            _action.Append(transform.DOLocalRotate(left, duration * 2));
            _action.Append(transform.DOLocalRotate(right, duration * 2));
        }

        _action.Append(transform.DOLocalRotate(Vector3.zero, duration));

        _action.AppendInterval(2);

        _action.OnComplete(() => {
            //Debug.Log("onComplete");
            if (_loop) {
                Play();
            }
        });

    }

}
