using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIService 
{
    public event Action<bool> OnToggleRaycast;
    void ToggleRaycast(bool value);
}
