using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BaseFramework
{
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
            MonoBehaviourManager.Instance.Init();
            DownloadManager.Instance.Init();
            ConfigManager.Instance.Init();
            CameraManager.Instance.Init();
            UIManager.Instance.Init();
            SocketHelper.Instance.Init();
        }

        private void Start()
        {
            //SocketHelper.Instance.InitConnect();
        }

        private void OnDestroy()
        {
            Instance.Release();
            MonoBehaviourManager.Instance.Release();
            DownloadManager.Instance.Release();
            ConfigManager.Instance.Release();
            CameraManager.Instance.Release();
            UIManager.Instance.Release();
            SocketHelper.Instance.Release();
        }
    }

}