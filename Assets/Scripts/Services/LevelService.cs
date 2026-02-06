using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void SaveLevelProgress(int levelID, int starsEarned)
    {
        int currentSavedStars = PlayerPrefs.GetInt($"Level_{levelID}_Stars", 0);

        if (starsEarned > currentSavedStars)
        {
            PlayerPrefs.SetInt($"Level_{levelID}_Stars", starsEarned);
            PlayerPrefs.Save();
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    ~LevelService()
    {
        GameServiceLocator.Unregister<ILevelService>();
    }
}
