using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        GameServiceLocator.Get<IUIService>().OnToggleRaycast += ToggleRaycast;
    }

    private void ToggleRaycast(bool value)
    {
        canvasGroup.blocksRaycasts = value;
    }
}