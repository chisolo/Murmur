using UnityEditor;
using UnityEngine;
using System.Collections;

namespace App.Editor
{
    public class MyTexturePostprocessor  : AssetPostprocessor
    {
        // void OnPostprocessTexture(Texture2D texture)
        // {
        //     if (assetPath.StartsWith("Assets/Res/UI/Atlas")) {

        //     }
        // }

        void OnPreprocessTexture()
        {
            if (assetPath.StartsWith("Assets/Res/UI/Atlas")) {
                ChangeToSprite();
            }
        }

        private void ChangeToSprite()
        {

            TextureImporter textureImporter  = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
        }
    }
}