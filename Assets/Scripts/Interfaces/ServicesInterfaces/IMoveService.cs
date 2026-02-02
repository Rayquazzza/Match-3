using System;
using UnityEngine;

public interface IMoveService
{
    event Action<int> OnMovesUpdated;
    event Action OnOutOfMoves;
    event Action<Candy,Vector2Int> OnSwapAttempt;
    void UseMove();
    int GetRemainingMoves();

    void AttemptSwap(Candy candy, Vector2Int direction);
    void Init();    
}