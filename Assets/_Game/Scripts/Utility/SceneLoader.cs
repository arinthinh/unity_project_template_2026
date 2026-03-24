using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    public static async UniTask LoadScene(string sceneName, float delayTime = 0,
        LoadSceneMode loadSceneMode = LoadSceneMode.Single, Action onComplete = null)
    {
        await UniTask.Yield();
        var async = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        if (async == null) return;

        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            await UniTask.Yield();
        }
        await UniTask.Yield();

        if (delayTime > 0)
        {
            await UniTask.WaitForSeconds(delayTime);
        }

        async.allowSceneActivation = true;
        onComplete?.Invoke();
    }

    public static async UniTask UnloadScene(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid()) return;
        if (!scene.isLoaded) return;

        var async = SceneManager.UnloadSceneAsync(scene);
        if (async == null) return;

        while (async.progress < 0.9f)
        {
            await UniTask.Yield();
        }
    }
}