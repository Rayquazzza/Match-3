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

    private IAudioService audioService;

    private IMatchService matchService;


    [SerializeField] private bool ResetSaveProgressOnAwake = false;


    private void Awake()
    {

        // --- Initialize Services ---
        gameStateService = new GameStateService();
        scoreService = new ScoreService();
        moveService = new MoveService();
        levelService = new LevelService();
        UIService = new UIService();
        audioService = new AudioService();
        matchService = new MatchService();
        // ---------------------------


        // --- Initialize service that require to listen to events ---
        moveService.Init();
        scoreService.Init();
        // ---------------------------


        if (ResetSaveProgressOnAwake)
        {
            levelService.ResetProgress();
        }

    }

    private void Start()
    {
        gameStateService.ChangeGameState(E_GameState.MENU);
        GameServiceLocator.Get<IMoveService>().OnOutOfMoves += OnMovesDepleted;
    }

    private void OnMovesDepleted()
    {
        int finalScore = scoreService.Score;
        LevelData currentLevel = levelService.CurrentLevelData;

        int stars = currentLevel.goals.GetStarsEarned(finalScore);

        levelService.SaveLevelProgress(currentLevel.levelID, stars);

        gameStateService.ChangeGameState(E_GameState.GAME_ENDED);
    }

    private void OnDestroy()
    {
        if (moveService != null)
            moveService.OnOutOfMoves -= OnMovesDepleted;
    }

}
