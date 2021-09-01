using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Camera MainCamera => mainCamera;

    private Camera mainCamera;

    public override void Init()
    {
        base.Init();

        InitMainCamera();
    }

    public override void Release()
    {
        base.Release();

        ClearMainCamera();
    }

    private void InitMainCamera()
    {
        ClearMainCamera();

        GameObject obj = new GameObject("MainCamera");
        mainCamera = obj.AddComponent<Camera>();
        mainCamera.tag = "MainCamera";
        obj.AddComponent<AudioListener>();
    }

    private void ClearMainCamera()
    {
        if (mainCamera != null)
        {
            Object.Destroy(mainCamera.gameObject);
        }
    }
}
