using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour, IManager where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T[] objs = FindObjectsOfType<T>();

                if (objs.Length > 0)
                {
                    MDebug.LogError($"MonoSingleton {typeof(T).Name} Should Never Be More Than 1 In Scene");

                    return objs[0];
                }

                GameObject monoSingleton = new GameObject(typeof(T).Name);
                instance = monoSingleton.AddComponent<T>();

                DontDestroyOnLoad(monoSingleton);
            }

            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = GetComponent<T>();

            DontDestroyOnLoad(gameObject);
        }
    }

    public void Init()
    {
    }

    public void Release()
    {
    }
}
