using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SoundLibrary
{
    
    public enum Categories//10
    {
        LevelIntro,
        LosesPiece,
        TakesPiece,
        MakesGoodMove,//player makes good move
        MakesBadMove,//player makes "bad" move
        Idle,
        ObjectiveQuip,
        MiscFunny,
        MiscAngry,
        Misc
    }

    private List<AudioClip>[] masterDialogueList;
    private const int maxAngerLevels = 5;
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
        //Debug.Log("list count: " + masterDialogueList[(int)from].C);
        int rand = (int)UnityEngine.Random.Range(0, masterDialogueList[(int)from].Count-0.01f);
        return masterDialogueList[(int)from][rand];
    }
}
