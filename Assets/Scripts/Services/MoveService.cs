using System;
using System.Diagnostics;
using UnityEngine;

public class MoveService : IMoveService
{
    public event Action<int> OnMovesUpdated;
    public event Action OnOutOfMoves;
    public event Action<Candy, Vector2Int> OnSwapAttempt;
    public event Action<Vector3, int> OnMatchPerformed;
    public event Action<Vector3> OnSwapFailed;

    private int _remainingMoves = 20;


    public MoveService()
    {
        GameServiceLocator.Register<IMoveService>(this);
    }

    public void UseMove()
    {
        if (_remainingMoves <= 0) return;

        _remainingMoves--;
        OnMovesUpdated?.Invoke(_remainingMoves);

        if (_remainingMoves <= 0)
        {
            OnOutOfMoves?.Invoke();
        }
    }
    

    public void SetMove(int move)
    {
        _remainingMoves = move;
    }
    public int GetRemainingMoves() => _remainingMoves;

    public void Init()
    {
        GameServiceLocator.Get<ILevelService>().OnLoadLevelData += HandleLevelLoaded;
    }

    private void HandleLevelLoaded(LevelData data)
    {
        if (data != null)
        {
            SetMove(data.maxMoves);
            OnMovesUpdated?.Invoke(_remainingMoves);
        }
    }

   

    ~MoveService()
    {
        Unregister();
    }

    private void Unregister()
    {
        GameServiceLocator.Unregister<IMoveService>();
    }

    public void AttemptSwap(Candy candy, Vector2Int direction)
    {
       OnSwapAttempt?.Invoke(candy, direction);
    }

    public void PerformMatch(Vector3 position, int matchSize)
    {
        OnMatchPerformed?.Invoke(position, matchSize);
    }

    public void SwapFailed(Vector3 position)
    {
        OnSwapFailed?.Invoke(position);
    }
}