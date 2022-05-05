using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace BaseFramework
{
    public class DownloadManager : MonoSingleton<DownloadManager>
    {
        private readonly string configPath;
        private readonly string texturePath;
        private readonly string assetBundlePath;

        public override void Init()
        {
            base.Init();
        }

        public override void Release()
        {
            base.Release();
        }

        #region Private Method

        #region Download
        private IEnumerator DownloadData<T>(string fileName, string rootPath, Action<string, DownloadHandler, Action<T>, bool> downloadedCallback, Action<T> successCallback, Action errorCallback, bool needCache = false, int retryCount = 0)
        {
            string downloadPath = $"{rootPath}{fileName}";
            Uri uri = new Uri(downloadPath);

            UnityWebRequest request = GetUnityWebRequest<T>(uri);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                MDebug.LogError(request.error);

                if (retryCount > 0)
                {
                    retryCount--;
                    yield return DownloadData(fileName, rootPath, downloadedCallback, successCallback, errorCallback, needCache, retryCount);
                }
                else
                {
                    errorCallback?.Invoke();
                }
                yield break;
            }

            downloadedCallback?.Invoke(fileName, request.downloadHandler, successCallback, needCache);
        }

        private static UnityWebRequest GetUnityWebRequest<T>(Uri uri)
        {
            UnityWebRequest request;
            if (typeof(T) == typeof(Texture) || typeof(T) == typeof(Texture2D))
            {
                request = UnityWebRequestTexture.GetTexture(uri);
            }
            else if (typeof(T) == typeof(AssetBundle))
            {
                request = UnityWebRequestAssetBundle.GetAssetBundle(uri);
            }
            else
            {
                request = UnityWebRequest.Get(uri);
            }

            return request;
        }
        #endregion

        #region Download Text
        private void DownloadDataConvertToText(string fileName, DownloadHandler downloadHandler, Action<string> callback, bool needCache)
        {
            string result = System.Text.Encoding.UTF8.GetString(downloadHandler.data);

            callback?.Invoke(result);
        }
        #endregion

        #region Download AssetBundle
        private void DownloadDataConvertToAssetBundle(string fileName, DownloadHandler downloadHandler, Action<AssetBundle> callback, bool needCache)
        {
            if (!(downloadHandler is DownloadHandlerAssetBundle))
            {
                return;
            }

            DownloadHandlerAssetBundle handlerAssetBundle = downloadHandler as DownloadHandlerAssetBundle;

            callback?.Invoke(handlerAssetBundle.assetBundle);
        }
        #endregion

        #region Download Texture
        private void DownloadDataConvertToTexture(string fileName, DownloadHandler downloadHandler, Action<Texture> callback, bool needCache)
        {
            if (!(downloadHandler is DownloadHandlerTexture))
            {
                return;
            }

            DownloadHandlerTexture handlerTexture = downloadHandler as DownloadHandlerTexture;

            callback?.Invoke(handlerTexture.texture);
        }
        #endregion

        #region Download Data
        private void DownloadDataConvertBytes(string fileName, DownloadHandler downloadHandler, Action<byte[]> callback, bool needCache)
        {
            callback?.Invoke(downloadHandler.data);
        }
        #endregion

        #endregion

        #region Public Method

        public void ResourceLoadText(string fileName, Action<string> callback, Action errorCallback = null, bool needCache = true, int retryCount = 0)
        {
            StartCoroutine(DownloadData(fileName, configPath, DownloadDataConvertToText, callback, errorCallback, needCache, retryCount));
        }

        public void ResourceLoadTexture(string fileName, Action<Texture> callback, Action errorCallback = null, bool needCache = true, int retryCount = 0)
        {
            StartCoroutine(DownloadData(fileName, texturePath, DownloadDataConvertToTexture, callback, errorCallback, needCache, retryCount));
        }

        public void ResourceLoadData(string fileName, Action<byte[]> callback, Action errorCallback = null, int retryCount = 0)
        {
            StartCoroutine(DownloadData(fileName, "", DownloadDataConvertBytes, callback, errorCallback, false, retryCount));
        }

        public void ResourceLoadAssetBundle(string fileName, Action<AssetBundle> callback, Action errorCallback = null, int retryCount = 0)
        {
            StartCoroutine(DownloadData(fileName, assetBundlePath, DownloadDataConvertToAssetBundle, callback, errorCallback, false, retryCount));
        }

        #endregion
    }

}