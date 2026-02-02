using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectService 
{
    void PlayExplosion(Vector3 position);

    void ShowScorePopup(Vector3 position, int score);
}
