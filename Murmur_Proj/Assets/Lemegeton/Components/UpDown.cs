using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UpDown : MonoBehaviour
{
    [SerializeField] float end = 10;
    [SerializeField] float duration = 0.5f;

    private Tweener _tweener;

    public void Play()
    {
        var rect = transform as RectTransform;
        var ori = rect.anchoredPosition;
        _tweener = rect.DOAnchorPos(new Vector2(ori.x, ori.y + end), duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    void OnDestroy()
    {
        if (_tweener != null) {
            _tweener.Kill();
        }
        _tweener = null;
    }

}
