using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary
{
    
    public enum Categories//17
    {
        //LevelIntro,
        LevelIntroNonPawn,//hooked up
        LevelIntroCTF,//hooked up
        LevelIntroKOTH,//hooked up
        LevelIntroCheckmate,//hooked up
        LosesPiece,//hooked up
        //TakesPiece,
        TakesKnight,//hooked up (i think)
        TakesBishop,//hooked up (i think)
        TakesRook,//hooked up (i think)
        TakesQueen,//hooked up (i think)
        MakesGoodMove,//hooked up (i think)
        MakesBadMove,//hooked up (i think)
        Idle,//hooked up
        ObjectiveQuip,// -> is this the same thing as levelintros
        MiscFunny,// -> could be rolled into misc in some type of way
        MiscAngry, // -> ^
        Misc,//        -> ^
        Interrupt
    }

    private List<AudioClip>[] masterDialogueList;
    public const int maxAngerLevels = 5;
    private string[] angerLevelFilePath = new string[maxAngerLevels];
    private string[] categoryNames;

    public SoundLibrary()
    {
        InitLibrary();
    }

    private void InitLibrary()
    {
        //create master dialogue lists
        categoryNames = Enum.GetNames(typeof(Categories));
                
        masterDialogueList = new List<AudioClip>[categoryNames.Length];
        for (int i = 0; i < masterDialogueList.Length; i++)
        {
            masterDialogueList[i] = new List<AudioClip>();
        }

        //create anger level string list
        for (int i = 0; i < angerLevelFilePath.Length; i++)
        {
            angerLevelFilePath[i] = "AngerLevel" + (i+1);
        }

    }

    public void LoadDialogue(int angerLevel)
    {
        //clear lists
        for (int i = 0; i < masterDialogueList.Length; i++)
        {
            masterDialogueList[i].Clear();
        }

        //load audio clips from resources into dialogueList
        for (int categoryIndex = 0; categoryIndex < categoryNames.Length; categoryIndex++)
        {
            //get audioClips from resources
            string path = angerLevelFilePath[angerLevel - 1] + "/Level" + angerLevel + categoryNames[categoryIndex];
            UnityEngine.Object[] clips = Resources.LoadAll(path , typeof(AudioClip));

            //put clips into dialogueList
            foreach (UnityEngine.Object clip in clips)
            {
                masterDialogueList[categoryIndex].Add(clip as AudioClip);
            }

            //unload resources
            for (int i = 0; i < clips.Length; i++)
            {
                Resources.UnloadAsset(clips[i]);
            }
        }
 
    }

    public AudioClip GetAudioClip(Categories from)
    {
        int rand = (int)UnityEngine.Random.Range(0, masterDialogueList[(int)from].Count-0.01f);
        return masterDialogueList[(int)from][rand];
    }
}
