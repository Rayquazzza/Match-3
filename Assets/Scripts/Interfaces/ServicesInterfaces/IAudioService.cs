using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioService 
{

    void PlayMusic(SoundEvent music);
    void StopMusic(bool fadeOut = true);

    void PlayOneShot(SoundEvent soundEvent, Vector3 position);
    void PlayMatch(SoundEvent soundEvent, Vector3 position, int combo);
}
