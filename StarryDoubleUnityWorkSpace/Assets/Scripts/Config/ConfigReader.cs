using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public class ConfigReader : BaseMonoBehaviour
    {
        private Queue<IConfig> readyLoadConfigs;

        private IConfig currentLoadConfig;

        private bool isRunning = false;

        private void Awake()
        {
            readyLoadConfigs = new Queue<IConfig>();
        }

        public void SetLoadConfigs(IEnumerable<IConfig> configs)
        {
            foreach (var config in configs)
            {
                SetLoadConfig(config);
            }
        }

        public void SetLoadConfig(IConfig config)
        {
            readyLoadConfigs.Enqueue(config);
        }

        public void StartLoad()
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;
            StartCoroutine(LoadConfig());
        }

        public void StopLoad()
        {
            StopCoroutine(LoadConfig());
            if (currentLoadConfig != null && !currentLoadConfig.Loaded)
            {
                readyLoadConfigs.Enqueue(currentLoadConfig);
                currentLoadConfig = null;
            }

            isRunning = false;
        }

        private void OnDestroy()
        {
            StopLoad();
            readyLoadConfigs = null;
        }

        private IEnumerator LoadConfig()
        {
            if (readyLoadConfigs == null)
            {
                yield return null;
            }

            while (readyLoadConfigs.Count > 0)
            {
                currentLoadConfig = readyLoadConfigs.Dequeue();
                currentLoadConfig.Read();

                yield return currentLoadConfig.Loaded;
                yield return new WaitForEndOfFrame();

                EventManager.Fire(ConfigManager.OneConfigLoaded, new EventData<IConfig>(currentLoadConfig));
                currentLoadConfig = null;
            }

            EventManager.Fire(ConfigManager.AllConfigLoaded, new EventData<ConfigReader>(this));
            StopLoad();
        }
    }
}
