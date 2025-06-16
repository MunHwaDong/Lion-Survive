using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    public readonly UniTaskQueue TaskQueue = new();
    
    //public Transform PlayerPosition { get; private set; }
    //For Test
    [SerializeField] public Transform PlayerPosition;
    [SerializeField] public PathFindingManager pathFindingManager;

    //Point 관리하는 사람이 이 함수를 등록해줘야함
    public Func<bool> IsOnBossBattle;
    
    //보스 관리하는 사람이 이 함수를 등록해줘야함
    public Func<bool> IsBossClear;
    
    private IState _currentState;

    public int dataControllerInstanceID = -1;

    public readonly Invoker invoker = new();
    
    public GameObject testGameOverUI;

    [SerializeField] public NavGraph mapData;

    public int MaxOfWorldXCoordinate => mapData.MAX_OF_WORLD_X_COORDINATE;
    public int MaxOfWorldZCoordinate => mapData.MAX_OF_WORLD_Z_COORDINATE;
    
    new void Awake()
    {
        base.Awake();

        SceneLoader.OnSceneLoadStart += (sceneId) =>
        {
            if (sceneId is SceneId.TitleScene)
                DataController.Instance.UnObserveData(Status.CURRENT_HP, GameOverCondition);
        };
        SceneLoader.OnSceneLoadFinish += InitializeScene;
    }

    public void InitializeScene(SceneId sceneId)
    {
        if (sceneId is SceneId.GameScene)
        {
            PlayerPosition = FindObjectOfType<PlayerController>().transform;
            pathFindingManager = FindObjectOfType<PathFindingManager>();
            mapData ??= Resources.Load<NavGraph>("NavGraph");
            mapData.Initialize();
            
            ChangeState(new InitState());
        }
        else if (sceneId is SceneId.TitleScene)
        {
            _currentState = null;
        }
    }
    
    public void ChangeState(IState newState)
    {
        _currentState?.ExitState();
        
        _currentState = newState;
        
        _currentState?.EnterState();
    }

    void Update()
    {
        // Debug.Log($"{_currentState.GetType().Name} : !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        // foreach (var (key, value) in invoker._stateBehaviours)
        // {
        //     Debug.Log($"{key}");
        //
        //     foreach (ICommand command in value)
        //     {
        //         Debug.Log($"{command.GetType().Name}");
        //     }
        // }

        _currentState?.UpdateState();
    }
    
    public void GameOverCondition(object data)
    {
        if (data is <= 0f)
        {
            ChangeState(new GameOverState());
        }
    }

    public void ToggleUseDeltaTime()
    {
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0f) ? 1f : 0f;
    }
}