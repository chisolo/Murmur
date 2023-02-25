using UnityEngine;
using UnityEngine.UI;
using Lemegeton;
public class HoleMask : MaskableGraphic, ICanvasRaycastFilter
{
    [SerializeField] RectTransform _hole;
    [SerializeField] Material _holMat;
    [SerializeField] RectTransform _hand;
    public void SetHole(Vector2 center, float silder)
    {
        _hole.AlignCenter(_hand.anchoredPosition, new Vector2(silder, silder));
        _holMat.SetVector("_Center", new Vector4(_hand.anchoredPosition.x, _hand.anchoredPosition.y, 0, 0));
        _holMat.SetFloat("_Silder", silder/2);
    }
    bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        if (null == _hole) return true;
        return !RectTransformUtility.RectangleContainsScreenPoint(_hole, screenPos, eventCamera);
    }

    public void SetTransparent(bool enable)
    {
        if(enable) {
            color = new Color32(5,5,5,1);
        } else {
            color = new Color32(5,5,5,155);
        }
    }
}