using UnityEngine;

public abstract class AudioObserverBase : MonoBehaviour
{
    protected IAudioService audioService;

    protected virtual void Start()
    {
        audioService = GameServiceLocator.Get<IAudioService>();
        SubscribeEvents();
    }

    protected abstract void SubscribeEvents();
    protected abstract void UnsubscribeEvents();

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}