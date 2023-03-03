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

    private Animator anim;
    private bool isPlaying = false;

    private void Awake()
    {
        //amim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (instance == null) { instance = this; }
        audioPlayer = GetComponent<AudioSource>();
        library = new SoundLibrary();

        library.LoadDialogue(currentAngerLevel);
    }

    private float PlaySoundFromCategory(SoundLibrary.Categories from)
    {
        audioPlayer.clip = library.GetAudioClip(from);
        audioPlayer.Play();
        return audioPlayer.clip.length;
    }

    private void OpenMouth()
    {
        
    }

    private void CloseMouth()
    {

    }

    private void PlayAnimation(int which = -1)
    {

    }

    public void PlayAnimationAndSound(SoundLibrary.Categories category)
    {
        if (isPlaying)
        {
            StopAllCoroutines();
        }
        StartCoroutine(PlayAnimationAndSoundCoRo(category));
    }

    private IEnumerator PlayAnimationAndSoundCoRo(SoundLibrary.Categories category)
    {
        isPlaying = true;
        OpenMouth();
        PlayAnimation();
        float clipLength = PlaySoundFromCategory(category);
        yield return new WaitForSeconds(clipLength);
        CloseMouth();
        isPlaying = false;
    }

    private void IncreaseAngerLevel()
    {
        library.LoadDialogue(++currentAngerLevel);//increment anger level and reload dialogue
    }

    public void SetAngerLevel(int level) // jenny
    {
        currentAngerLevel = level;
    }
}
