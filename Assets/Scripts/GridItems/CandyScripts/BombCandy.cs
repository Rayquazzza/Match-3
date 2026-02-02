using System.Collections.Generic;
using UnityEngine;

public class BombCandy : Candy
{

    public override void ExecuteEffect(Vector2Int myPos)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                grid.ClearMatchAt(myPos.x + x, myPos.y + y);
            }
        }
    }

}