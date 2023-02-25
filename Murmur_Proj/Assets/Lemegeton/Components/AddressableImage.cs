using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lemegeton
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class AddressableImage : MonoBehaviour
    {
        [SerializeField]
        private AssetReferenceSprite _spriteReference = null;

        public AssetReferenceSprite SpriteReference => _spriteReference;

        private Image _imgObj;

        void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                // 非运行时不自动加载
                return;
            }
#endif

            _imgObj = GetComponent<Image>();
            LoadSpriteAync();
        }

        private void LoadSpriteAync()
        {
            if (_spriteReference.RuntimeKeyIsValid()) {
                _spriteReference.LoadAssetAsync<Sprite>().Completed += (op) => {
                    _imgObj.sprite = op.Result;
                };
            }
        }

        private void UnloadSprite()
        {
            _spriteReference.ReleaseAsset();
            _imgObj.sprite = null;
        }

#if UNITY_EDITOR
        void OnEnable()
        {
            if (!Application.isPlaying) {
                if (_imgObj == null) {
                    _imgObj = GetComponent<Image>();
                }

                if (_spriteReference != null && _spriteReference.editorAsset != null) {
                    if (_spriteReference.editorAsset is SpriteAtlas) {
                        if (!string.IsNullOrEmpty(_spriteReference.SubObjectName)) {
                            var sp = (_spriteReference.editorAsset as SpriteAtlas).GetSprite(_spriteReference.SubObjectName);
                            // do not save
                            sp.hideFlags = HideFlags.HideAndDontSave;
                            _imgObj.sprite = sp;
                        }
                    } else if (_spriteReference.editorAsset is Texture2D) {
                        var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(_spriteReference.AssetGUID);
                        var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                        var sp = Sprite.Create((Texture2D)_spriteReference.editorAsset, sprite.rect, sprite.pivot);
                        sp.name = "(Clone)";
                        // do not save
                        sp.hideFlags = HideFlags.HideAndDontSave;
                        _imgObj.sprite = sp;
                    } else {
                        Debug.LogError("invaild type " + _spriteReference.editorAsset);
                    }
                }

                return;
            }
        }

        void OnDisable()
        {
            if (!Application.isPlaying) {
                if (_imgObj == null) {
                    _imgObj = GetComponent<Image>();
                }

                if (deleteBySelf) return;
                if (_imgObj.sprite != null) {
                    _imgObj.sprite = null;

                    // TOOD: delete created in scene
                }

                return;
            }
        }

        private bool deleteBySelf = false;
        [ContextMenu("UseOrigin")]
        void UseOrigin()
        {
            var spriteReference = _spriteReference;
            if (spriteReference != null && spriteReference.editorAsset != null) {
                if (spriteReference.editorAsset is SpriteAtlas) {

                    if (!string.IsNullOrEmpty(spriteReference.SubObjectName)) {
                        var path = AssetDatabase.GetAssetPath(spriteReference.editorAsset);
                        var subPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path), spriteReference.SubObjectName+".png");
                        var main = AssetDatabase.LoadAssetAtPath<Sprite>(subPath);
                        if (main != null) {
                            _imgObj.sprite = main as Sprite;
                            deleteBySelf = true;
                            DestroyImmediate(this);
                        }

                    }
                }
            }
        }
#endif

        protected void OnDestroy()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                return;
            }
#endif
            UnloadSprite();
        }

    }


#if UNITY_EDITOR
    [CustomEditor(typeof(AddressableImage))]
    public class AddressableImageEditor : Editor
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
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var spp = (AddressableImage)target;
            var spriteReference = spp.SpriteReference;

            // var prop = typeof(AddressableImage).GetField("_spriteReference", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            // Debug.Log(prop);

            if (spriteReference != null && spriteReference.editorAsset != null) {
                if (spriteReference.editorAsset is SpriteAtlas) {

                    if (!string.IsNullOrEmpty(spriteReference.SubObjectName)) {
                        var path = AssetDatabase.GetAssetPath(spriteReference.editorAsset);
                        var subPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path), spriteReference.SubObjectName+".png");
                        var main = AssetDatabase.LoadMainAssetAtPath(subPath);
                        if (main != null) {
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.ObjectField("source", main, main.GetType(), false);
                            EditorGUI.EndDisabledGroup();
                        }

                    }
                }
            }

        }
    }
#endif
}