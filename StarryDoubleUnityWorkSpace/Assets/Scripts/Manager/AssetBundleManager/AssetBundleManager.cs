using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        private Dictionary<string, AssetBundleData> loadedAssetBundles;

        public override void Init()
        {
            base.Init();
        }

        public override void Release()
        {
            base.Release();
        }

        public void ResourceLoadAsset<T>()
        {
            
        }
    }
}
