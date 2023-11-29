using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public static event Action<string, Vector3> TransitionEvent;

    public static void CallTransitionEvent(string sceneName,Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }

    public static event Action BeforeSceneloadedEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneloadedEvent?.Invoke();
    }

    public static event Action AfterSceneloadedEvent;

    public static void CallAfterSceneUnloadEvent()
    {
        AfterSceneloadedEvent?.Invoke();
    }
}

