using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGameplayObserver : AudioObserverBase
{
    [SerializeField] private SoundEvent matchSound;
    [SerializeField] private SoundEvent swapSound;
    [SerializeField] private SoundEvent swapFailedSound;
    private float lastMatchTime;

    private void HandleSwapAttempt(Candy candy, Vector2Int direction)
    {
        if(swapSound) audioService.PlayOneShot(swapSound, candy.transform.position);
    }

    private void HandleSwapFailed(Vector3 vector)
    {
       if(swapFailedSound) audioService.PlayOneShot(swapFailedSound, vector);
    }

    private void HandleMatch(Vector3 vector, int combo)
    {
        if (Time.time - lastMatchTime < 0.1f) return; 
        lastMatchTime = Time.time;

        if (matchSound) audioService.PlayMatch(matchSound, vector, combo);
    }

    protected override void SubscribeEvents()
    {
        IMoveService moveService = GameServiceLocator.Get<IMoveService>();

        moveService.OnSwapAttempt += HandleSwapAttempt;
        moveService.OnMatchPerformed += HandleMatch;
        moveService.OnSwapFailed += HandleSwapFailed;
    }

    protected override void UnsubscribeEvents()
    {
        IMoveService moveService = GameServiceLocator.Get<IMoveService>();
        if (moveService != null)
        {
            moveService.OnSwapAttempt -= HandleSwapAttempt;
            moveService.OnMatchPerformed -= HandleMatch;
            moveService.OnSwapFailed -= HandleSwapFailed;
        }
    }
}
