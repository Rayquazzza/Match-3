using UnityEngine;

public class AudioService : IAudioService
{

    private FMOD.Studio.EventInstance _activeMusic;

    public AudioService()
    {
        GameServiceLocator.Register<IAudioService>(this);
    }

    public void PlayOneShot(SoundEvent soundEvent, Vector3 position)
    {
        soundEvent.Play(position);
    }

    public void PlayMatch(SoundEvent soundEvent, Vector3 position, int combo)
    {
        soundEvent.Play(position, combo);
    }

    public void PlayMusic(SoundEvent music)
    {
        StopMusic(true); 

        if (music == null) return;
        _activeMusic = FMODUnity.RuntimeManager.CreateInstance(music.fmodEvent);
        _activeMusic.start();
    }

    public void StopMusic(bool fadeOut = true)
    {
        if (_activeMusic.isValid())
        {
            var mode = fadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE;
            _activeMusic.stop(mode);
            _activeMusic.release();
        }
    }
}