using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIService : IUIService
{
    public event Action<bool> OnToggleRaycast;

    public UIService()
    {
        GameServiceLocator.Register<IUIService>(this);
    }

    public void ToggleRaycast(bool value)
    {
        OnToggleRaycast?.Invoke(value);
    }

    ~UIService()
    {
        Unregister();
    }

    private void Unregister()
    {
        GameServiceLocator.Unregister<IUIService>();
    }
}
