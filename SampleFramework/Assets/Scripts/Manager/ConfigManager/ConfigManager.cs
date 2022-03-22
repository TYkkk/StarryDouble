using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BaseFramework
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        public const string OneConfigLoaded = "OneConfigLoaded";
        public const string AllConfigLoaded = "AllConfigLoaded";

        private ConfigReader configReader;

        public override void Init()
        {
            base.Init();
            EventManager.Register(AllConfigLoaded, AllConfigLoadedHandler);
        }

        public override void Release()
        {
            EventManager.UnRegister(AllConfigLoaded, AllConfigLoadedHandler);
            base.Release();
        }

        public void LoadConfig()
        {
            IConfig[] configs = new IConfig[] {
                new TestReadConfig(),
                new TestReadConfig(),
                new TestReadConfig(),
                new TestReadConfig(),
                new TestReadConfig(),
                new TestReadConfig(),
                new TestReadConfig(),
                new TestReadConfig(),
            };

            if (configReader == null)
            {
                GameObject reader = new GameObject("configReader");
                configReader = reader.AddComponent<ConfigReader>();
            }

            configReader.SetLoadConfigs(configs);
            configReader.StartLoad();
        }

        private void AllConfigLoadedHandler(IEventData data)
        {
            var cr = data as EventData<ConfigReader>;
            UnityEngine.Object.Destroy(cr.Data.gameObject);
        }
    }
}