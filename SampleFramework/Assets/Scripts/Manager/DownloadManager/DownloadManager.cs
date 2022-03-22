using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace BaseFramework
{
    public class DownloadManager : MonoSingleton<DownloadManager>
    {
        private readonly string configPath;

        private Dictionary<string, string> downloadedTextData = new Dictionary<string, string>();
        private const int downloadedTextDataCacheCount = 100;

        private IEnumerator DownloadData<T>(string fileName, string rootPath, Action<string, byte[], Action<T>, bool> callback, Action<T> userCallback, Action errorCallback, bool needCache)
        {
            string url = $"{rootPath}/{fileName}";

            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                MDebug.LogError(request.error);
                errorCallback?.Invoke();
                yield break;
            }

            callback?.Invoke(fileName, request.downloadHandler.data, userCallback, needCache);
        }

        private void DownloadTextData(string fileName, Action<string> callback, Action errorCallback, bool needCache)
        {
            StartCoroutine(DownloadData(fileName, configPath, DownloadDataConvertToText, callback, errorCallback, needCache));
        }

        private void DownloadDataConvertToText(string fileName, byte[] data, Action<string> callback, bool needCache)
        {
            string result = System.Text.Encoding.Default.GetString(data);

            if (needCache)
            {
                if (!downloadedTextData.ContainsKey(fileName))
                {
                    if (downloadedTextData.Count > downloadedTextDataCacheCount)
                    {
                        downloadedTextData.Clear();
                    }

                    downloadedTextData.Add(fileName, result);
                }
            }

            callback?.Invoke(result);
        }

        public void ResourceLoadText(string fileName, Action<string> callback, Action errorCallback = null, bool needCache = true)
        {
            if (needCache)
            {
                if (downloadedTextData.ContainsKey(fileName))
                {
                    callback?.Invoke(downloadedTextData[fileName]);
                    return;
                }
            }

            DownloadTextData(fileName, callback, errorCallback, needCache);
        }
    }

}