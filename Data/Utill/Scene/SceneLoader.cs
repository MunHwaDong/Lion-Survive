using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static TaskQueue _taskQueue;
    public static event Action OnAnySceneLoadStart;
    public static event Action<SceneId> OnSceneLoadStart;
    public static event Action OnAnySceneLoadFinish;
    public static event Action<SceneId> OnSceneLoadFinish;

    public static Func<UniTask> SceneAnimation;
    public static Func<UniTask> LoadingTask;
    
    public static LoadingUI LoadingUI;
    
    public static SceneId CurrentLoadingScene { get; private set; } = SceneId.Unknown;
    public static AsyncOperation AsyncOperation { get; private set; }

    public static void OnLoadComplete(Scene scene, LoadSceneMode _)
    {
        SceneId sceneId = BI.NAME_TO_ID[scene.name];
        
        OnAnySceneLoadFinish?.Invoke();
        OnSceneLoadFinish?.Invoke(sceneId);

        if (CurrentLoadingScene is not SceneId.Unknown && sceneId == CurrentLoadingScene)
        {
            CurrentLoadingScene = SceneId.Unknown;
            _taskQueue.DoneTask();
        }
    }
    public static void Load(this SceneId id, bool isAsync = false, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (_taskQueue is null)
        {
            _taskQueue = new TaskQueue();
            SceneManager.sceneLoaded += OnLoadComplete;
        }
        
        _taskQueue.RunTask(() => TaskLoad(id, mode, isAsync));
    }

    private static async void TaskLoad(SceneId id, LoadSceneMode mode, bool isAsync)
    {
        string sceneName = BI.ID_TO_NAME[id];

        if (sceneName is null)
        {
            throw new NullReferenceException("Scene not found");
        }

        CurrentLoadingScene = id;
        
        OnAnySceneLoadStart?.Invoke();
        OnSceneLoadStart?.Invoke(id);

        if (isAsync)
        {
            //Fade Out
            if(SceneAnimation is not null)
                await SceneAnimation.Invoke();
            else
                throw new NullReferenceException("Scene not loaded");
            
            AsyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            
            //Do Async Task at Process Loading
            if(LoadingTask is not null)
                await LoadingTask.Invoke();
            
            //Fade In
            await SceneAnimation.Invoke();
        }
        else SceneManager.LoadScene(sceneName, mode);
    }
}
