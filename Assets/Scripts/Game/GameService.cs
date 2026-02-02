using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameService : MonoBehaviour
{
    private IGameStateService gameStateService;

    private IScoreService scoreService;

    private IMoveService moveService;

    private ILevelService levelService;

    private IUIService UIService;

    private void Awake()
    {
        gameStateService = new GameStateService();
        scoreService = new ScoreService();
        moveService = new MoveService();
        levelService = new LevelService();
        UIService = new UIService();

        moveService.Init();

        scoreService.Init();

    }


    private void Start()
    {
        gameStateService.ChangeGameState(E_GameState.MENU);
        GameServiceLocator.Get<IMoveService>().OnOutOfMoves += OnMovesDepleted;
    }


    private void OnMovesDepleted()
    {
        GameServiceLocator.Get<ITransitionService>().TransitionToState(E_GameState.LEVEL_SELECTION);
    }

    private void OnDestroy()
    {
        if (moveService != null)
            moveService.OnOutOfMoves -= OnMovesDepleted;
    }

}
