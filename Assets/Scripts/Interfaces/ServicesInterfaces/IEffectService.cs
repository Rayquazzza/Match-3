using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectService 
{
    void PlayColorBombEffect(Vector3 vector, List<Vector3> list, float arg3);
    void PlayExplosion(Vector3 position);

    void ShowScorePopup(Vector3 position, int score);
}
