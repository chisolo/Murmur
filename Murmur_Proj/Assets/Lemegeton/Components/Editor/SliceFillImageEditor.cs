using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Linq;

[CustomEditor(typeof(SliceFillImage))]
public class SliceFillImageEditor : ImageEditor
{
    SerializedProperty m_FillMethod;
    SerializedProperty m_FillOrigin;
    SerializedProperty m_FillAmount;
    SerializedProperty m_FillClockwise;
    SerializedProperty m_Type;
    SerializedProperty m_FillCenter;
    SerializedProperty m_Sprite;
    SerializedProperty m_PreserveAspect;
    GUIContent m_SpriteContent;
    GUIContent m_SpriteTypeContent;
    GUIContent m_ClockwiseContent;
    AnimBool m_ShowSlicedOrTiled;
    AnimBool m_ShowSliced;
    AnimBool m_ShowFilled;
    AnimBool m_ShowType;

    void SetShowNativeSize(bool instant)
    {
        SliceFillImage.Type type = (SliceFillImage.Type)m_Type.enumValueIndex;
        bool showNativeSize = (type == SliceFillImage.Type.Simple || type == SliceFillImage.Type.Filled);
        base.SetShowNativeSize(showNativeSize, instant);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        m_SpriteContent = new GUIContent("Source SilceFillImage");
        m_SpriteTypeContent = new GUIContent("SilceFillImage Type");
        m_ClockwiseContent = new GUIContent("Clockwise");

        m_Sprite = serializedObject.FindProperty("m_Sprite");
        m_Type = serializedObject.FindProperty("m_Type");
        m_FillCenter = serializedObject.FindProperty("m_FillCenter");
        m_FillMethod = serializedObject.FindProperty("m_FillMethod");
        m_FillOrigin = serializedObject.FindProperty("m_FillOrigin");
        m_FillClockwise = serializedObject.FindProperty("m_FillClockwise");
        m_FillAmount = serializedObject.FindProperty("m_FillAmount");
        m_PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");

        m_ShowType = new AnimBool(m_Sprite.objectReferenceValue != null);
        m_ShowType.valueChanged.AddListener(Repaint);

        var typeEnum = (SliceFillImage.Type)m_Type.enumValueIndex;
        m_ShowSlicedOrTiled = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == SliceFillImage.Type.Sliced);
        m_ShowSliced = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == SliceFillImage.Type.Sliced);
        m_ShowFilled = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == SliceFillImage.Type.Filled);
        m_ShowSlicedOrTiled.valueChanged.AddListener(Repaint);
        m_ShowSliced.valueChanged.AddListener(Repaint);
        m_ShowFilled.valueChanged.AddListener(Repaint);

        SetShowNativeSize(true);
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SpriteGUI();
        AppearanceControlsGUI();
        RaycastControlsGUI();

        m_ShowType.target = m_Sprite.objectReferenceValue != null;
        if (EditorGUILayout.BeginFadeGroup(m_ShowType.faded))
        {
            EditorGUILayout.PropertyField(m_Type, m_SpriteTypeContent);

            ++EditorGUI.indentLevel;
            {
                SliceFillImage.Type typeEnum = (SliceFillImage.Type)m_Type.enumValueIndex;
                bool showSlicedOrTiled = (!m_Type.hasMultipleDifferentValues && (typeEnum ==SliceFillImage.Type.Sliced|| typeEnum == SliceFillImage.Type.Tiled));
                if (showSlicedOrTiled && targets.Length > 1)
                    showSlicedOrTiled = targets.Select(obj => obj as SliceFillImage).All(img => img.hasBorder);

                m_ShowSlicedOrTiled.target = showSlicedOrTiled;
                m_ShowSliced.target = (showSlicedOrTiled && !m_Type.hasMultipleDifferentValues && typeEnum == SliceFillImage.Type.Sliced);
                m_ShowFilled.target = (!m_Type.hasMultipleDifferentValues && typeEnum == SliceFillImage.Type.Filled);

                SliceFillImage cImage = target as SliceFillImage;

                if (EditorGUILayout.BeginFadeGroup(m_ShowSlicedOrTiled.faded))
                {
                    if (cImage.hasBorder)
                    {
                        EditorGUILayout.PropertyField(m_FillCenter);
                        EditorGUILayout.PropertyField(m_FillAmount);
                        EditorGUILayout.PropertyField(m_FillMethod);
                    }

                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowSliced.faded))
                {
                    if (cImage.sprite != null && !cImage.hasBorder)
                        EditorGUILayout.HelpBox("This SilceFillImage doesn't have a border.", MessageType.Warning);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowFilled.faded))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_FillMethod);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_FillOrigin.intValue = 0;
                    }
                    switch ((SliceFillImage.FillMethod)m_FillMethod.enumValueIndex)
                    {
                        case SliceFillImage.FillMethod.Horizontal:
                            m_FillOrigin.intValue = (int)(SliceFillImage.OriginHorizontal)EditorGUILayout.EnumPopup("Fill Origin", (SliceFillImage.OriginHorizontal)m_FillOrigin.intValue);
                            break;
                        case SliceFillImage.FillMethod.Vertical:
                            m_FillOrigin.intValue = (int)(SliceFillImage.OriginVertical)EditorGUILayout.EnumPopup("Fill Origin", (SliceFillImage.OriginVertical)m_FillOrigin.intValue);
                            break;
                        case SliceFillImage.FillMethod.Radial90:
                            m_FillOrigin.intValue = (int)(SliceFillImage.Origin90)EditorGUILayout.EnumPopup("Fill Origin", (SliceFillImage.Origin90)m_FillOrigin.intValue);
                            break;
                        case SliceFillImage.FillMethod.Radial180:
                            m_FillOrigin.intValue = (int)(SliceFillImage.Origin180)EditorGUILayout.EnumPopup("Fill Origin", (SliceFillImage.Origin180)m_FillOrigin.intValue);
                            break;
                        case SliceFillImage.FillMethod.Radial360:
                            m_FillOrigin.intValue = (int)(SliceFillImage.Origin360)EditorGUILayout.EnumPopup("Fill Origin", (SliceFillImage.Origin360)m_FillOrigin.intValue);
                            break;
                    }
                    EditorGUILayout.PropertyField(m_FillAmount);
                    if ((SliceFillImage.FillMethod)m_FillMethod.enumValueIndex > SliceFillImage.FillMethod.Vertical)
                    {
                        EditorGUILayout.PropertyField(m_FillClockwise, m_ClockwiseContent);
                    }
                }
                EditorGUILayout.EndFadeGroup();
            }
            --EditorGUI.indentLevel;
        }

        EditorGUILayout.EndFadeGroup();

        SetShowNativeSize(false);
        if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_PreserveAspect);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
        NativeSizeButtonGUI();

        serializedObject.ApplyModifiedProperties();
    }
}
