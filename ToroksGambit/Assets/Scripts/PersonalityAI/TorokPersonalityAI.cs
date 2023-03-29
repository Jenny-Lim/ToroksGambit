using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this class handles torok dialog and anaimation selection
public class TorokPersonalityAI : MonoBehaviour
{

    //1 + (index/2) -> angerlevel from level index 

    [SerializeField] private int currentAngerLevel = 1;
    [Range(0f, 1f)]
    [SerializeField] private float[] dialogLikelyhoodByCategory = { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}; // 17 total 
    public float minTimeBetweenIdleBark = 12;
    public float maxTimeBetweenIdleBark = 25;


    private SoundLibrary library;
    private AudioSource audioPlayer;

    public static TorokPersonalityAI instance;

    private Animator anim;
    private bool isPlaying = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (instance == null) { instance = this; }
        audioPlayer = GetComponent<AudioSource>();
        library = new SoundLibrary();

        library.LoadDialogue(currentAngerLevel);
    }

    private void Update()
    {
        if (Input.GetKeyDown("g"))
        {
            PlaySoundFromCategory(SoundLibrary.Categories.LosesPiece);
        }
    }

    public float PlaySoundFromCategory(SoundLibrary.Categories from)
    {
        if (audioPlayer.isPlaying)
        {
            Debug.Log("audio was already playing");
            //return 0; //<-this causes some weirdness as when u/him captues a piece
            //it could play dialogue but if the game is over after that move then it kinda clashes with the dialogue that should play when you lose for example
            //doing it like this without the return overrides the last call, which might be the best solution rn but is a little jarring when it happens
        }

        audioPlayer.clip = library.GetAudioClip(from);
        audioPlayer.Play();
        return audioPlayer.clip.length;
    }

    public bool ShouldPlay(SoundLibrary.Categories from, float randomNum)
    {
        return randomNum < dialogLikelyhoodByCategory[(int)from];
    }

    private void OpenMouth()
    {
        anim.SetBool("OpenMouth", true);
    }

    private void CloseMouth()
    {
        anim.SetBool("OpenMouth", false);
    }

    //plays an animation
    private float PlayAnimation(int which = -1)
    {
        if (which <= -1)
        {
            anim.SetFloat("SelectedAnimation", 1);
        }
        else
        {

        }
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        return state.length;
    }

    public void PlayAnimationAndSound(SoundLibrary.Categories category)
    {
        if (isPlaying)
        {
            StopAllCoroutines();
            isPlaying = false;
        }
        StartCoroutine(PlayAnimationAndSoundCoRo(category));
    }

    public IEnumerator PlayAnimationAndSoundCoRo(SoundLibrary.Categories category)
    {
        anim.SetBool("Talk", true);
        float animClipLength = PlayAnimation();
        float audioClipLength = PlaySoundFromCategory(category);
        Debug.Log("ClipLenth " + audioClipLength);
        isPlaying = true;
        yield return new WaitForSeconds(Mathf.Max(audioClipLength, animClipLength));
        anim.SetFloat("SelectedAnimation", 0);
        anim.SetBool("Talk", false);
        CloseMouth();
        isPlaying = false;
    }

    public void IncreaseAngerLevel()
    {
        currentAngerLevel = Mathf.Min(currentAngerLevel + 1, SoundLibrary.maxAngerLevels);
        library.LoadDialogue(currentAngerLevel);
    }

    public void SetAngerLevel(int level) // jenny
    {
        currentAngerLevel = level;
    }
}
