using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Flags]
public enum ToggleUIColorType
{
    Image = 0x01,
    RawImage = 0x02,
    Text = 0x04
}
public class ToggleUIColor : MonoBehaviour
{   
    [SerializeField] ToggleUIColorType toggleType;
    [SerializeField] Image image;
    [SerializeField] RawImage rawImage;
    [SerializeField] Text text;
    [SerializeField] List<Color> colors;
    public int index;

    public void Start()
    {
        SetIndex(index);
    }
    
    public void SetIndex(int index)
    {
        if(image != null) {
            if(index < 0 || index >= colors.Count) image.color = Color.white;
            image.color = colors[index];
        } 
        if(rawImage != null) {
            if(index < 0 || index >= colors.Count) rawImage.color = Color.white;
            rawImage.color = colors[index];
        }
        if(text == null) {
            if(index < 0 || index >= colors.Count) text.color = Color.white;
            text.color = colors[index];
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ToggleUIColor))]
public class ToggleUIColorEditor : Editor
{
    private SerializedObject obj;

    private SerializedProperty toggleType;
    private SerializedProperty image;
    private SerializedProperty rawImage;
    private SerializedProperty text;
    private SerializedProperty colors;
    private void OnEnable()
    {
        obj = new SerializedObject(target);
        toggleType = obj.FindProperty("toggleType");
        image = obj.FindProperty("image");
        rawImage = obj.FindProperty("rawImage");
        text = obj.FindProperty("text"); 
        colors = obj.FindProperty("colors");
    }

    public override void OnInspectorGUI()
    {
        obj.Update();
        EditorGUILayout.PropertyField(toggleType);

        var typeValue = toggleType.intValue;
        if((typeValue & (int)ToggleUIColorType.Image) != 0) {
            EditorGUILayout.PropertyField(image);
        } else {
            image.objectReferenceValue = null;
        }
        if((typeValue & (int)ToggleUIColorType.RawImage) != 0) {
            EditorGUILayout.PropertyField(rawImage);
        } else {
            rawImage.objectReferenceValue = null;
        } 
        if((typeValue & (int)ToggleUIColorType.Text) != 0) {
            EditorGUILayout.PropertyField(text);
        } else {
            text.objectReferenceValue = null;
        } 

        EditorGUILayout.PropertyField(colors);

        obj.ApplyModifiedProperties();
    }
}
#endif