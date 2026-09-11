using UnityEngine;
using UnityEngine.Pool;

public static class UnityPoolHelper
{
    public static ObjectPool<T> CreatePool<T>(this MonoBehaviour monoBehaviour, T prefab, Transform spawnPoint) where T : MonoBehaviour
    {
        return new ObjectPool<T>
        (
            () => Object.Instantiate(prefab, spawnPoint),
            @object => @object.gameObject.SetActive(true),
            @object => @object.gameObject.SetActive(false),
            @object => Object.Destroy(@object.gameObject)
        );
    }
}