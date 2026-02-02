using System;
using UnityEngine;
public class GameStateService : IGameStateService
{
    private E_GameState currentGameState;


    public event Action<E_GameState> OnGameStateChanged;


    public GameStateService()
    {
        GameServiceLocator.Register<IGameStateService>(this);
    }


    public void ChangeGameState(E_GameState newGameState)
    {
        currentGameState = newGameState;
        Debug.Log("[GameStateService] Game State changed to: " + currentGameState);
        OnGameStateChanged?.Invoke(currentGameState);
    }

    public E_GameState GetCurrentGameState()
    {
        return currentGameState;
    }

    public void Unregister()
    {
        GameServiceLocator.Unregister<IGameStateService>();
    }

    ~GameStateService()
    {
        Unregister();
    }
}
