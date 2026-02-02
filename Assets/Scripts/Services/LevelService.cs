using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelService : ILevelService
{
    public LevelData CurrentLevelData { get; private set; }

    public event Action<LevelData> OnLoadLevelData;

    public LevelService()
    {
        GameServiceLocator.Register<ILevelService>(this);
    }

    public void LoadLevelData(LevelData levelData)
    {
        CurrentLevelData = levelData;
        GameServiceLocator.Get<ITransitionService>().TransitionToState(E_GameState.IN_GAME);
    }

    public void LoadLevelData(LevelData levelData, E_GameState gameState)
    {
        CurrentLevelData = levelData;
        OnLoadLevelData?.Invoke(levelData);
        GameServiceLocator.Get<ITransitionService>().TransitionToState(gameState);
    }

    ~LevelService()
    {
        GameServiceLocator.Unregister<ILevelService>();
    }
}
