using System;
using UnityEngine;

// Singleton solution based on Martin Zikmund's: https://blog.mzikmund.com/2019/01/a-modern-singleton-in-unity/
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static readonly Lazy<T> lazyInstance = new Lazy<T>(InitSingleton);

    public static T Instance => lazyInstance.Value;

    private static T InitSingleton() {
        T existingObject = FindObjectOfType<T>();
        if (existingObject != null) {
            DontDestroyOnLoad(existingObject);
            return existingObject;
		}
        GameObject ownerObject = new GameObject($"{typeof(T).Name}_singleton");
        T instance = ownerObject.AddComponent<T>();
        DontDestroyOnLoad(ownerObject);
        return instance;
    }
}
