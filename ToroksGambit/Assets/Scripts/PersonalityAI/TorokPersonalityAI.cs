using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this class handles torok dialog and anaimation selection
public class TorokPersonalityAI : MonoBehaviour
{
    [SerializeField] private int currentAngerLevel = 1;
    [Range(0f, 1f)]
    [SerializeField] private float[] dialogLikelyhoodByCategory = new float[10];

    private SoundLibrary library;
    private AudioSource audioPlayer;

    public static TorokPersonalityAI instance;

    private void IncreaseAngerLevel() { 
        library.LoadDialogue(++currentAngerLevel);//increment anger level and reload dialogue
    }

    private void Start()
    {
        if (instance == null) { instance = this; }
        audioPlayer = GetComponent<AudioSource>();
        library = new SoundLibrary();

        library.LoadDialogue(currentAngerLevel);
    }

    private void PlaySoundFromCategory(SoundLibrary.Categories from)
    {
        audioPlayer.clip = library.GetAudioClip(from);
        audioPlayer.Play();
    }
}
