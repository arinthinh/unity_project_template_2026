using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public static class GlobalObjectPool
{
    private static readonly Dictionary<Object, object> _pools = new Dictionary<Object, object>();
    private static readonly Dictionary<Object, Object> _instanceToPrefab = new Dictionary<Object, Object>();

    public static T Spawn<T>(T prefab) where T : Object
    {
        var pool = GetOrCreatePool(prefab);
        var instance = pool.Get();
        _instanceToPrefab[instance] = prefab;
        return instance;
    }

    public static void Despawn<T>(T instance) where T : Object
    {
        if (!_instanceToPrefab.TryGetValue(instance, out var prefab))
            return;

        _instanceToPrefab.Remove(instance);
        ((ObjectPool<T>)_pools[prefab]).Release(instance);
    }

    private static ObjectPool<T> GetOrCreatePool<T>(T prefab) where T : Object
    {
        if (_pools.TryGetValue(prefab, out var existing))
            return (ObjectPool<T>)existing;

        var pool = new ObjectPool<T>
        (
            () => Object.Instantiate(prefab),
            obj => SetActive(obj, true),
            OnRelease,
            Destroy
        );
        _pools.Add(prefab, pool);
        return pool;
    }

    private static void OnRelease<T>(T obj) where T : Object
    {
        SetActive(obj, false);
        SetParent(obj, null);
    }

    private static void SetActive<T>(T obj, bool active) where T : Object
    {
        switch (obj)
        {
            case GameObject go:
                go.SetActive(active);
                break;
            case Component component:
                component.gameObject.SetActive(active);
                break;
        }
    }

    private static void SetParent<T>(T obj, Transform parent) where T : Object
    {
        switch (obj)
        {
            case GameObject go:
                go.transform.SetParent(parent);
                break;
            case Component component:
                component.transform.SetParent(parent);
                break;
        }
    }

    private static void Destroy<T>(T obj) where T : Object
    {
        switch (obj)
        {
            case GameObject go:
                Object.Destroy(go);
                break;
            case Component component:
                Object.Destroy(component.gameObject);
                break;
        }
    }
}
