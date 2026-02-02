using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CostUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI costText;

    // Start is called before the first frame update
    void Start()
    {
        GameServiceLocator.Get<IMoveService>().OnMovesUpdated += UpdateCost;

        int remainingCost = GameServiceLocator.Get<IMoveService>().GetRemainingMoves();
        UpdateCost(remainingCost);
    }

    private void UpdateCost(int remainingCost)
    {
        costText.text = $"Remaining Cost : {remainingCost}";
    }

}
