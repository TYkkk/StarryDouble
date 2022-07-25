using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public static class AssetExtension
    {
        public static void AddTextureAsset(this GameObject go)
        {
            if (go.GetComponent<TextureAssetHolder>() == null)
            {
                go.AddComponent<TextureAssetHolder>();
            }
        }
    }
}
