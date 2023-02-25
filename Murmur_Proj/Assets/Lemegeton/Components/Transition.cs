using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Lemegeton
{
    public class Transition : MonoBehaviour
    {
        public enum Direction
        {
            Right,
            Left,
            Up,
            Down,
        }
        [SerializeField]
        private RectTransform _container = null;
        [SerializeField]
        private CanvasGroup _canvasGroup = null;
        [SerializeField]
        private float _interval = 0.2f;

        private Vector2 _originPos;
        private void Awake()
        {
            _originPos = _container.anchoredPosition;
        }

        public void Left()
        {
            Move(Direction.Left);
        }

        public void Right()
        {
            Move(Direction.Right);
        }

        public void Up()
        {
            Move(Direction.Up);
        }

        public void Down()
        {
            Move(Direction.Down);
        }

        public void Move(Direction directoin)
        {
            switch(directoin) {
                case Direction.Up:
                    _container.DOAnchorPos(new Vector2(_originPos.x, _originPos.y + _container.rect.height), _interval);
                    break;
                case Direction.Down:
                    _container.DOAnchorPos(new Vector2(_originPos.x, _originPos.y - _container.rect.height), _interval);
                    break;
                case Direction.Left:
                    _container.DOAnchorPos(new Vector2(_originPos.x - _container.rect.width, _originPos.y), _interval);
                    break;
                case Direction.Right:
                    _container.DOAnchorPos(new Vector2(_originPos.x + _container.rect.width, _originPos.y), _interval);
                    break;
            }
        }

        public void Back()
        {
            _container.DOAnchorPos(_originPos, _interval);
        }

        public void FadeIn()
        {
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1, _interval).SetEase(Ease.InOutSine);

        }

        public void FadeOut()
        {
            _canvasGroup.DOFade(0, _interval)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => gameObject.SetActive(false));
        }


    }
}