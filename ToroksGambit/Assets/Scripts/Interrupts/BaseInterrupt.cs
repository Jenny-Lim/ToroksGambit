using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class BaseInterrupt : ScriptableObject
{
    public bool hasTriggered { get;  protected set; } = false;
    public InterruptManager.InterruptTrigger triggerType;
    public abstract void Enact();

    public abstract bool ShouldTrigger();

    public void ResetHasTrigger()
    {
        hasTriggered = false;
    }
}