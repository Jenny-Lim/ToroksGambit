using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseInterrupt : ScriptableObject
{

    protected bool hasTriggered = false;
    public InterruptManager.InterruptTrigger triggerType;
    public abstract void Enact();

    public abstract bool ShouldTrigger();
}
