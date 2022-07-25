using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle
{
    [MenuItem("EditorTools/AssetBundle/BuildTest")]
    public static void BuildTest()
    {
        string outPath = "./AssetBundles/Windows/";

        if (Directory.Exists(outPath))
        {
            Directory.Delete(outPath);
        }

        Directory.CreateDirectory(outPath);

        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
    }
}
