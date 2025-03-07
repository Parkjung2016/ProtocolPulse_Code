using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PJH.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace PJH.Manager
{
    public class AddressableManager
    {
        private Dictionary<string, Object> _resources = new();
        private Dictionary<string, AsyncOperationHandle> _handles = new();

        #region load reousources

        public T Load<T>(string key) where T : Object
        {
            if (_resources.TryGetValue(key, out Object resource))
            {
                var result = resource as T;
                Debug.Log(result);
                if (result == null)
                    if (resource is GameObject go)
                    {
                        return go.GetComponent<T>();
                    }

                return result;
            }

            return null;
        }

        public T Instantiate<T>(string key, Transform parent = null) where T : Component
        {
            GameObject prefab = Load<GameObject>(key);

            if (!prefab)
            {
                Debug.LogError($"Failed to load prefab : {key}");
                return null;
            }

            var prefabInstantiate = Object.Instantiate(prefab, parent);
            T go = prefabInstantiate.GetComponent<T>();
            go!.gameObject.name = prefab.name;
            return go;
        }

        public GameObject Instantiate(string key, Transform parent = null)
        {
            GameObject prefab = Load<GameObject>(key);

            if (!prefab)
            {
                Debug.LogError($"Failed to load prefab : {key}");
                return null;
            }

            GameObject go = Object.Instantiate(prefab, parent);
            go.gameObject.name = prefab.name;
            return go;
        }

        #endregion

        #region addressable

        private async UniTask<T> LoadAsync<T>(string key) where T : Object
        {
            if (_resources.TryGetValue(key, out Object resource))
            {
                return resource as T;
            }

            string loadKey = key;
            if (key.Contains(".sprite"))
                loadKey = $"{key}[{key.Replace(".sprite", "")}]";

            var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
            await asyncOperation.Task;

            T result = asyncOperation.Result;
            _resources.TryAdd(key, result);
            _handles.TryAdd(key, asyncOperation);
            return result;
        }


        public async UniTask LoadALlAsync<T>(string label, Action<string, int, int> callBack = null) where T : Object
        {
            var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
            await opHandle.Task;

            int loadCount = 0;
            int totalCount = opHandle.Result.Count;

            foreach (var result in opHandle.Result)
            {
                bool isContainsDotSprite = result.PrimaryKey.Contains(".sprite");
                if (isContainsDotSprite)
                {
                    await LoadAsync<Sprite>(result.PrimaryKey);
                    loadCount++;
                    callBack?.Invoke(result.PrimaryKey, loadCount, totalCount);
                }
                else
                {
                    await LoadAsync<T>(result.PrimaryKey);
                    loadCount++;
                    callBack?.Invoke(result.PrimaryKey, loadCount, totalCount);
                }
            }
            // await DownloadDependenciesAsync(label, async () =>
            // {
            //    
            // });
        }

        public async Task DownloadDependenciesAsync(object label, Action CompleteCallback)
        {
            var getDownloadHandle = Addressables.GetDownloadSizeAsync(label);
            await getDownloadHandle.Task;
            if (getDownloadHandle.Status == AsyncOperationStatus.Succeeded && getDownloadHandle.Result > 0)
            {
                var downloadHandle = Addressables.DownloadDependenciesAsync(label, true);
                await downloadHandle.Task;
                if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
                    return;

                CompleteCallback?.Invoke();
            }
            else
            {
                CompleteCallback?.Invoke();
            }
        }

        #endregion
    }
}