using UnityEngine;

public class MusicObserver : AudioObserverBase
{
    [Header("Music Settings")]
    [SerializeField] private SoundEvent menuMusic;
    [SerializeField] private SoundEvent gameplayMusic;

    private E_GameState _lastState = E_GameState.NONE;

    private void HandleStateChanged(E_GameState newState)
    {
        if (newState == _lastState) return;

        var audioService = GameServiceLocator.Get<IAudioService>();

        switch (newState)
        {
            case E_GameState.MENU:
                audioService.PlayMusic(menuMusic);
                break;
            case E_GameState.IN_GAME:
                audioService.PlayMusic(gameplayMusic);
                break;
        }
        _lastState = newState;
    }

    protected override void SubscribeEvents()
    {
        var gameStateService = GameServiceLocator.Get<IGameStateService>();
        if (gameStateService != null)
        {
            gameStateService.OnGameStateChanged += HandleStateChanged;
            HandleStateChanged(gameStateService.GetCurrentGameState());
        }
    }

    protected override void UnsubscribeEvents()
    {
        var service = GameServiceLocator.Get<IGameStateService>();
        if (service != null) service.OnGameStateChanged -= HandleStateChanged;
    }
}