using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Lemegeton;

public class PuppetEmojiView : MonoBehaviour
{
    [SerializeField] GameObject _bubble;
    [SerializeField] Image _emojiImg;
    [SerializeField] Canvas _canvas;

    private Quaternion _rotation = Quaternion.Euler(40f, 130f, 0);
    public void SetCamera(Camera camera)
    {
        _canvas.worldCamera = camera;
    }
    public void Show(string emoji)
    {
        if(EmojiType.EmojiIcon.TryGetValue(emoji, out string emojiPath)) {
            gameObject.SetActive(true);
            _bubble.transform.localScale = Vector3.zero;
            _emojiImg.ShowSprite(emojiPath);
            Sequence seq = DOTween.Sequence();
            seq.Append(_bubble.transform.DOScale(Vector3.one, 1.0f))
                .AppendInterval(2.0f)
                .Append(_bubble.transform.DOScale(Vector3.zero, 0.5f))
                .OnComplete(()=>{ gameObject.SetActive(false); });
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    void Update()
    {
        _canvas.transform.rotation = _rotation;
    }
}
