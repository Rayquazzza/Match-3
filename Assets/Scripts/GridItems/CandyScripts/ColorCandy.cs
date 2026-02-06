using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCandy : Candy
{

    // Cette méthode est appelée par le GridGenerator
    public void ExecuteColorEffect(CandyItemData candyType)
    {
        // Grâce à l'injection, le bonbon dit à la grille quoi faire
        grid.ClearColor(candyType);

        // On pourrait ajouter un effet visuel ici (particules, éclairs...)
        Debug.Log($"Color Bomb activée sur la couleur : {candyType}");
    }

    // On override l'ID pour être sûr qu'il ne match pas avec des bonbons normaux

    public override bool TriggerSpecialEffect(Candy swappedWith)
    {
        // La ColorCandy s'auto-exécute avec l'ID du bonbon avec lequel elle a switché
        CandyItemData targetID = swappedWith.GetItemType();

        // On nettoie la grille
        grid.ClearColor(targetID);

        // On se détruit soi-même et l'autre
        swappedWith.Destroy();
        this.Destroy();

        return true; // On confirme qu'un effet a eu lieu
    }
}
