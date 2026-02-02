using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CandyData", menuName = "Match3/CandyData")]
public class CandyData : ScriptableObject
{
    public Sprite IceCreamSprite;
    public Sprite MuffinSprite;
    public Sprite RuskSprite;

    public Sprite GetSpriteForType(E_CandyType type)
    {   
        switch (type)
        {
            case E_CandyType.None:
                return null;

            case E_CandyType.IceCream:
                return IceCreamSprite;

            case E_CandyType.Muffin:
                return MuffinSprite;

            case E_CandyType.Rusk:
                return RuskSprite;

            default:
                return null;
        }
    }
}


