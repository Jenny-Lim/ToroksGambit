using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class handles torok dialog and anaimation selection
public class TorokPersonalityAI : MonoBehaviour
{
    [SerializeField] private int currentAngerLevel = 1;

    private SoundLibrary library;
    private AudioSource audioPlayer;

    private void IncreaseAngerLevel() { 
        library.LoadDialogue(++currentAngerLevel);//increment anger level and reload dialogue
    }

    private void Start()
    {
        audioPlayer= GetComponent<AudioSource>();
        library = new SoundLibrary();

        library.LoadDialogue(currentAngerLevel);

        PlaySoundFromCategory(SoundLibrary.Categories.MiscAngry);
    }

    private void PlaySoundFromCategory(SoundLibrary.Categories from)
    {
        audioPlayer.clip = library.GetAudioClip(from);
        audioPlayer.Play();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown("j"))
        {
            PlaySoundFromCategory(SoundLibrary.Categories.LevelIntro);
        }*/
        
    }
}
