using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptManager : MonoBehaviour
{
    [SerializeField] private List<BaseInterrupt> levelInterrupts;
    public static InterruptManager instance;
    public enum InterruptTrigger
    {
        GameStart,
        AfterTurn,
        AfterPlayerTurn,
        AfterTorokTurn,
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        } 
        ResetInterruptListTriggers();
    }

    public void Update()
    {
        if (Input.GetKey("p"))
        {
            EnactInterrupts(InterruptTrigger.GameStart);
        }
    }

    public void EnactInterrupts(InterruptTrigger type)
    {
        if (levelInterrupts.Count < 1) { return; }

        foreach (BaseInterrupt interrupt in levelInterrupts)
        {
            if (interrupt == null)//gaurd clause
            {
                continue;
            }

            if ( interrupt.triggerType == type && interrupt.ShouldTrigger() )//if the trigger type is met and it should trigger then enact the interrupt
            {
                interrupt.Enact();
            }
        }
    }

    public void ResetInterruptListTriggers()
    {
        foreach (BaseInterrupt interrupt in levelInterrupts)
        {
            interrupt.ResetHasTrigger();
        }
    }
}
