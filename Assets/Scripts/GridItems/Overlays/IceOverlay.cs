using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceOverlay : GridItem
{
    public override bool IsMovable => false;

    public override bool IsMatchable => false;

    public override void BreakLayer()
    {
        Debug.Log("Ice overlay broken at " + transform.position);
        GameServiceLocator.Get<IEffectService>().PlayExplosion(transform.position);

        Vector2Int pos = grid.GetPositionOf(this);
            
        if (pos.x != -1) 
        {
            grid.ClearOverlayAt(pos.x, pos.y);
        }
    }
}
