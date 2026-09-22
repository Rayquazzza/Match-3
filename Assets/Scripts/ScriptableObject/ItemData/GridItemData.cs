using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewItemData", menuName = "Match3/GridItemData")]

public class GridItemData : ScriptableObject
{
    public string itemName;
    public GameObject prefab;
    public Sprite icon;       
}
