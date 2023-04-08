using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this class handles torok dialog and anaimation selection
public class TorokPersonalityAI : MonoBehaviour
{

    //1 + (index/2) -> angerlevel from level index 

    [SerializeField] private float numRandAnimations;

    [SerializeField] private int currentAngerLevel = 1;
    [Range(0f, 1f)]
    [SerializeField] private float[] dialogLikelyhoodByCategory = { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}; // 17 total 
    private int[] categoryPriorities = { 3, 3, 3, 3, 2, 2, 2, 2, 2, 1, 1, 1, 3, 3, 3, 2, 2, 2 };
    public float minTimeBetweenIdleBark = 12;
    public float maxTimeBetweenIdleBark = 25;
    

    private SoundLibrary.Categories lastCatPlayed;
    private SoundLibrary library;
    private AudioSource audioPlayer;

    public static TorokPersonalityAI instance;

    private Animator anim;
    [SerializeField] private Animator shopAnim;
    private bool isPlaying = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (instance == null) { instance = this; }
        audioPlayer = GetComponent<AudioSource>();
        library = new SoundLibrary();

        library.LoadDialogue(currentAngerLevel);
    }

    public float PlaySoundFromCategory(SoundLibrary.Categories from)
    {
        if (audioPlayer.isPlaying && categoryPriorities[(int)from] < categoryPriorities[(int)lastCatPlayed])
        {
            
            return 0; //<-this causes some weirdness as when u/him captues a piece
            //it could play dialogue but if the game is over after that move then it kinda clashes with the dialogue that should play when you lose for example
            //doing it like this without the return overrides the last call, which might be the best solution rn but is a little jarring when it happens
        }

        audioPlayer.clip = library.GetAudioClip(from);
        audioPlayer.Play();
        lastCatPlayed = from;
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
    private void PlayAnimation(SoundLibrary.Categories category, int which = -1)
    {
        if (which <= -1)//use random anim
        {
            if ((int)category >= (int)SoundLibrary.Categories.ShopEnter)
            {
                shopAnim.SetFloat("SelectedAnimation", 1);
            }
            else
            {
                anim.SetBool("PlayAnim", true);
                int randInt = (int)Random.Range(0, numRandAnimations - 0.01f);
                print(randInt);
                anim.SetFloat("SelectedAnimation", randInt * (1/numRandAnimations));
            }
            
        }
        else// use certain animation, probably wont be used idk
        {
            if ((int)category >= (int)SoundLibrary.Categories.ShopEnter)
            {
                shopAnim.SetFloat("SelectedAnimation", which);
            }
            else
            {
                anim.SetBool("PlayAnim", true);
                anim.SetFloat("SelectedAnimation", which);
            }

        }
        
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
        //anim.SetBool("Talk", true);
        PlayAnimation(category);
        float animClipLength;
        if (category >= SoundLibrary.Categories.ShopEnter)
        {
            animClipLength = shopAnim.GetCurrentAnimatorClipInfo(0).Length;
        }
        else
        {
            animClipLength = anim.GetCurrentAnimatorClipInfo(0).Length;
        }

        float audioClipLength = PlaySoundFromCategory(category);
        Debug.Log("ClipLenth " + audioClipLength);
        isPlaying = true;
        print(audioClipLength + "," + animClipLength);
        yield return new WaitForSeconds(Mathf.Max(audioClipLength, animClipLength));
        anim.SetFloat("SelectedAnimation", 0);
        anim.SetBool("Talk", false);
        //CloseMouth();
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

    public void ResetGameAnimationParameter()
    {
        anim.SetFloat("SelectedAnimation", -1);
        anim.SetBool("PlayAnim", false);
    }
}
