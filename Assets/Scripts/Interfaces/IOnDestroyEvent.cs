using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnDestroyEvent
{
    public event OnDestroyDelegate OnDestroyEvent;

    public delegate void OnDestroyDelegate(GameObject instance);
}
