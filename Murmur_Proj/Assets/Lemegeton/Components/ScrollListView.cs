using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lemegeton
{
    public interface IScrollListItem
    {

    }

    // TODO
    [RequireComponent(typeof(ScrollRect))]
    public abstract class ScrollListView<T> : MonoBehaviour where T : IScrollListItem
    {

        private ScrollRect _scroll;
        private RectTransform _content;
        private void Start()
        {
            _scroll = GetComponent<ScrollRect>();
            _content = _scroll.content;
        }


        private void OnDestroy()
        {
            _scroll = null;
            _content = null;
        }

        public void Init(List<T> itemList)
        {

        }
    }
}