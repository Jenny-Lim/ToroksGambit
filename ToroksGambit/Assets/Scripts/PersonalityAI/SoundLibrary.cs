using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary
{
    
    public enum Categories//10
    {
        LevelIntro,
        LosesPiece,
        TakesPiece,
        MakesGoodMove,
        MakesBadMove,
        Idle,
        ObjectiveQuip,
        MiscFunny,
        MiscAngry,
        Misc
    }

    private List<List<AudioClip>> masterDialogueList;

    private void Start() {
        masterDialogueList = new List<List<AudioClip>>(Enum.GetNames(typeof(Categories)).Length);

        for (int i = 0; i < masterDialogueList.Count; i++)
        {
            masterDialogueList[i] = new List<AudioClip>();
        }
    }

    private void LoadDialogue(int angerLevel)
    {

    }

    //returns 
    public AudioClip GetAudioClip(Categories from)
    {
        return null;
    }
}
