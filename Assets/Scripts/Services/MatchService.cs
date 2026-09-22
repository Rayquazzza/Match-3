using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchService : IMatchService
{
    public event Action<MatchData> OnMatchExecuted;
    public event Action<Vector3, CandyItemData> OnItemDestroyed;
    public event Action<Vector3, List<Vector3>, float> OnColorBombEffect;
    public event Action<Vector3, int> OnScorePointGenerated;

    public MatchService()
    {
        GameServiceLocator.Register<IMatchService>(this);
    }

    public void NotifyMatch(MatchData data)
    {
        OnMatchExecuted?.Invoke(data);
    }

    public void NotifyItemDestroyed(Vector3 position, CandyItemData data)
    {
        OnItemDestroyed?.Invoke(position, data);
    }

    public void NotifyColorBomb(Vector3 origin, List<Vector3> targets, float duration)
    {
       OnColorBombEffect?.Invoke(origin, targets, duration);
    }

    public void NotifyScorePoint(Vector3 position, int score)
    {
        OnScorePointGenerated?.Invoke(position, score);
    }
}
