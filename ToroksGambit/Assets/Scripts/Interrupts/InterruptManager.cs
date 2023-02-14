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
        AfterTurn
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        } 
        ResetInterruptListTriggers();
    }

    public void Update()//for testing purposes
    {
        if (Input.GetKeyDown("p"))
        {
            EnactInterrupts(InterruptTrigger.GameStart);
        }
    }

    public List<BaseInterrupt> GetActiveInterrupts()
    {
        return levelInterrupts;
    }

    public void EnactInterrupts(InterruptTrigger type)
    {
        if (levelInterrupts.Count < 1) { return; }

        foreach (BaseInterrupt interrupt in levelInterrupts)
        {
            if (interrupt == null)//null gaurd clause
            {
                continue;
            }

            if (interrupt.triggerType == type && type == InterruptTrigger.GameStart && !interrupt.hasTriggered)//if given trigger is gamestart and interrupt's type is gamestart, just enact
            {
                interrupt.Enact();
            }
            else if(interrupt.triggerType == type && interrupt.ShouldTrigger())//if the type is correct and interrupt's conditions are met, enact
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
