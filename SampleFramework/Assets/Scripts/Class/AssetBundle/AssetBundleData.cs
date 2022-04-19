using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleData
{
    public string BundleName { get; private set; }
    public AssetBundle Bundle { get; private set; }
    public string[] DependBundlesName { get; private set; }
    public int ReferenceCount { get; set; }

    public AssetBundleData(string bundleName, AssetBundle bundle, string[] dependBundlesName)
    {
        BundleName = bundleName;
        Bundle = bundle;
        DependBundlesName = dependBundlesName;
        ReferenceCount = 0;
    }
}
