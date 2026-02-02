using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreService : IScoreService
{
    private int score;

    public int Score
    {
        get { return score; }
        set { score = value; }
    }


    public event Action<int> OnScoreUpdated;

   public ScoreService()
    {
        GameServiceLocator.Register<IScoreService>(this);
    }
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        OnScoreUpdated?.Invoke(score);
    }

    public void Unregister()
    {
        GameServiceLocator.Unregister<IScoreService>();
    }

    public void Init()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged += GameStateChanged;
    }

    private void GameStateChanged(E_GameState state)
    {
        if(state == E_GameState.IN_GAME)
        {
            score = 0;
            OnScoreUpdated?.Invoke(score);
        }
    }

    ~ScoreService()
    {
        Unregister();
    }


}
