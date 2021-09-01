using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoSingleton<GameManager>, IManager
{
    public override void Awake()
    {
        base.Awake();

        InitManager();
    }

    private void InitManager()
    {
        Instance.Init();
        CameraManager.Instance.Init();
        UIManager.Instance.Init();
    }
}
