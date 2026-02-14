using System;
using UnityEngine;

public interface IMoveService
{
    event Action<int> OnMovesUpdated;
    event Action OnOutOfMoves;
    event Action<Candy,Vector2Int> OnSwapAttempt;
    event Action<Vector3, int> OnMatchPerformed;
    event Action<Vector3> OnSwapFailed;
    void UseMove();
    int GetRemainingMoves();

    void AttemptSwap(Candy candy, Vector2Int direction);
    void Init();    

    void PerformMatch(Vector3 position, int matchSize);
    void SwapFailed(Vector3 position);
}