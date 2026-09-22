using System;
using System.Collections.Generic;
using UnityEngine;
public interface IMatchService
{
    event Action<MatchData> OnMatchExecuted;
    event Action<Vector3, CandyItemData> OnItemDestroyed;
    event Action<Vector3, List<Vector3>, float> OnColorBombEffect;
    event Action<Vector3, int> OnScorePointGenerated;


    void NotifyMatch(MatchData data);
    void NotifyItemDestroyed(Vector3 position, CandyItemData data);

    void NotifyColorBomb(Vector3 origin, List<Vector3> targets, float duration);

    void NotifyScorePoint(Vector3 position, int score);
}