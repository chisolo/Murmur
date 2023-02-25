using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

public class SliceFillImage : Image
{
    private static Vector2[] s_VertScratch = {Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero};
    private static Vector2[] s_UVScratch = {Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero};
    
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);
        if (overrideSprite == null)
        {
            base.OnPopulateMesh(toFill);
            return;
        }
        if (type == Type.Sliced)
        {
            GenerateSlicedSprite_(toFill);
        }
    }
    
    Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
    {
        for (int axis = 0; axis <= 1; axis++)
        {
            float combinedBorders = border[axis] + border[axis + 2];
            if (rect.size[axis] < combinedBorders && combinedBorders != 0)
            {
                float borderScaleRatio = rect.size[axis] / combinedBorders;
                border[axis] *= borderScaleRatio;
                border[axis + 2] *= borderScaleRatio;
            }
        }
        return border;
    }
    
    private void GenerateSlicedSprite_(VertexHelper toFill)
    {
        Vector4 outer, inner, padding, border;
        if (overrideSprite != null)
        {
            outer = DataUtility.GetOuterUV(overrideSprite);
            inner = DataUtility.GetInnerUV(overrideSprite);
            padding = DataUtility.GetPadding(overrideSprite);
            border = overrideSprite.border;
        }
        else
        {
            outer = Vector4.zero;
            inner = Vector4.zero;
            padding = Vector4.zero;
            border = Vector4.zero;
        }
        
        var rect = GetPixelAdjustedRect();
        var adjustedBorders = GetAdjustedBorders(border / multipliedPixelsPerUnit, rect);
        padding = padding / multipliedPixelsPerUnit;
        
        s_VertScratch[0] = new Vector2(padding.x, padding.y);
        
        var xFillAmount = 1.0f;
        var yFillAmount = 1.0f;
        if (fillMethod == FillMethod.Horizontal)
        {
            xFillAmount = fillAmount;
        }
        else
        {
            yFillAmount = fillAmount;
        }
        
        var xCondition = (border.z + border.x) / rect.width;
        var yCondition = (border.w + border.y) / rect.height;
        
        s_VertScratch[1].x = xFillAmount < xCondition ? xFillAmount / 2 * rect.width : border.x;
        s_VertScratch[1].y = yFillAmount < yCondition ? yFillAmount / 2 * rect.height : border.y;
        
        s_VertScratch[2].x = xFillAmount < xCondition ? s_VertScratch[1].x : rect.width * xFillAmount - border.z;
        s_VertScratch[2].y = yFillAmount < yCondition ? s_VertScratch[1].y : rect.height * yFillAmount - border.w;
        
        s_VertScratch[3].x = xFillAmount < xCondition ? s_VertScratch[1].x * 2 : s_VertScratch[2].x + border.z;
        s_VertScratch[3].y = yFillAmount < yCondition ? s_VertScratch[1].y * 2 : s_VertScratch[2].y + border.w;
        
        for (int i = 0; i < 4; ++i)
        {
            s_VertScratch[i].x += rect.x;
            s_VertScratch[i].y += rect.y;
        }
        
        s_UVScratch[0] = new Vector2(outer.x, outer.y);
        s_UVScratch[1].x = xFillAmount < xCondition ? xFillAmount * rect.width / 2 / sprite.rect.size.x : inner.x;
        s_UVScratch[1].y = yFillAmount < yCondition ? yFillAmount * rect.height / 2 / sprite.rect.size.y : inner.y;
        s_UVScratch[2].x = xFillAmount < xCondition ?  1 - s_UVScratch[1].x : inner.z;
        s_UVScratch[2].y = yFillAmount < yCondition ?  1 - s_UVScratch[1].y : inner.w;
        s_UVScratch[3] = new Vector2(outer.z, outer.w);
        
        toFill.Clear();
        
        for (int x = 0; x < 3; ++x)
        {
            int x2 = x + 1;

            for (int y = 0; y < 3; ++y)
            {
                if (!fillCenter && x == 1 && y == 1)
                    continue;
                int y2 = y + 1;
                AddQuad(toFill,
                    new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                    new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                    color,
                    new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                    new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y));
            }
        }
    }
    
    static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
    {
        int startIndex = vertexHelper.currentVertCount;

        vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y));
        vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y));
        vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y));
        vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y));

        vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
    }
}