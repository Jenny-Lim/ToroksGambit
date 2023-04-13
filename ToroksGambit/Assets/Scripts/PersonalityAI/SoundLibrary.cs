using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary
{
    
    public enum Categories//19
    {
        LevelIntroNonPawn,//hooked up
        LevelIntroCTF,//hooked up
        LevelIntroKOTH,//hooked up
        LevelIntroCheckmate,//hooked up
        LosesPiece,//hooked up
        TakesKnight,//hooked up (i think)
        TakesBishop,//hooked up (i think)
        TakesRook,//hooked up (i think)
        TakesQueen,//hooked up (i think)
        MakesGoodMove,//hooked up (i think)
        MakesBadMove,//hooked up (i think)
        Idle,//hooked up
        Interrupt,//hooked up
        LoseGame,//torok loses, or player wins
        WinGame,//torok wins, or player loses
        WinWholeGame,//player wins entire game
        ShopEnter,
        ShopExit,
        ShopBuy,
    }


    private List<AudioClip>[] masterDialogueList;
    public const int maxAngerLevels = 5;
    private string[] angerLevelFilePath = new string[maxAngerLevels];
    private string[] categoryNames;

    private int lastPlayedIndex = 1;

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

            //this was causing the audio to not play, need to ask rick about this cuz i think if this is not done it never unloads them so it just eats up memory
            //unload resources
            /*for (int i = 0; i < clips.Length; i++)
            {
                Resources.UnloadAsset(clips[i]);
            }*/
        }
 
    }

    public AudioClip GetAudioClip(Categories from)
    {
        int rand = (int)UnityEngine.Random.Range(0, masterDialogueList[(int)from].Count-0.01f);
        if (masterDialogueList[(int)from].Count > 1)
        {
            while (rand == lastPlayedIndex)
            {
                rand = (int)UnityEngine.Random.Range(0, masterDialogueList[(int)from].Count - 0.01f);
            }
        }
        lastPlayedIndex = rand;
        return masterDialogueList[(int)from][rand];
    }
}
