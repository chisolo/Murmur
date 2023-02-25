using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Lemegeton
{
    [RequireComponent(typeof(Toggle))]
    public class Tab : MonoBehaviour
    {
        private Toggle _toggle;
        public Toggle Toggle => _toggle;

        [SerializeField]
        private GameObject _target;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private bool _fade = true;
        [SerializeField]
        private float _interval = 0.15f;
        [SerializeField]
        private GameObject _tabOn;
        [SerializeField]
        private GameObject _tabOff;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(OnValueChanged);

            _target.SetActive(_toggle.isOn);
            _tabOn.SetActive(_toggle.isOn);
            _tabOff.SetActive(!_toggle.isOn);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveAllListeners();
        }


        public void OnValueChanged(bool isOn)
        {
            //UnityEditor.EditorApplication.isPaused = true;
            if (isOn && !_target.activeSelf) {
                _target.SetActive(true);
                _tabOn.SetActive(true);
                _tabOff.SetActive(false);

                if (_fade) {
                    _canvasGroup.alpha = 0;

                    // 等待当前标签消失
                    var action = DOTween.Sequence();
                    action.AppendInterval(_interval);
                    action.Append(_canvasGroup.DOFade(1, _interval));
                }

            } else if (!isOn && _target.activeSelf) {
                _tabOn.SetActive(false);
                _tabOff.SetActive(true);
                if (_fade) {
                    _canvasGroup.DOFade(0, _interval).OnComplete(() => {
                        _target.SetActive(false);
                    });
                } else {
                    _target.SetActive(false);
                }
            }

        }
    }
}
