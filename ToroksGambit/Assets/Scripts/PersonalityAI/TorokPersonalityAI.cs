using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this class handles torok dialog and anaimation selection
public class TorokPersonalityAI : MonoBehaviour
{

    //1 + (index/2) -> angerlevel from level index 

    [SerializeField] private float numGeneralAnimsGame;
    [SerializeField] private float numInterruptAnimsGame;
    [SerializeField] private float numWinAnimsGame;
    [SerializeField] private float numLoseAnimsGame;

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

    public Animator anim;
    [SerializeField] private Animator shopAnim;
    private bool isPlaying = false;
    [SerializeField] AudioClip[] audioClips;

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
            return 0; 
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
        //assuming all animations will be randomized within there category
        //select category
        if (category == SoundLibrary.Categories.Interrupt)//Interrupt
        {
            anim.SetBool("PlayAnim", true);
            anim.SetInteger("SelectedAnimCategory", 2);
            anim.SetInteger("SelectedAnimation", (int)Random.Range(1,numInterruptAnimsGame + 0.99f));
        }
        else if (category == SoundLibrary.Categories.LoseGame)//torok wins i think
        {
            anim.SetBool("PlayAnim", true);
            anim.SetInteger("SelectedAnimCategory", 3);
            anim.SetInteger("SelectedAnimation", (int)Random.Range(1, numLoseAnimsGame + 0.99f));
        }
        else if (category == SoundLibrary.Categories.WinGame)//torok loses i think
        {
            anim.SetBool("PlayAnim", true);
            anim.SetInteger("SelectedAnimCategory", 4);
            anim.SetInteger("SelectedAnimation", (int)Random.Range(1, numWinAnimsGame + 0.99f));
        }
        else if (category == SoundLibrary.Categories.WinWholeGame)
        {
            anim.SetBool("PlayAnim", true);
            anim.SetInteger("SelectedAnimCategory", 5);
            anim.SetInteger("SelectedAnimation", 1);
        }
        else if (category == SoundLibrary.Categories.ShopEnter)
        {
            shopAnim.SetBool("EnteredShop", true);
        }
        else if (category == SoundLibrary.Categories.ShopExit)
        {
            shopAnim.SetBool("ExitedShop", true);
        }
        else if (category == SoundLibrary.Categories.ShopBuy)
        {
            shopAnim.SetBool("PieceSold", true);
        }
        else//general animation
        {
            anim.SetBool("PlayAnim", true);
            anim.SetInteger("SelectedAnimCategory", 1);
            anim.SetInteger("SelectedAnimation", (int)Random.Range(1, numGeneralAnimsGame + 0.99f));//
        }


        /*
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
        */
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
        Debug.Log("in coro");
        isPlaying = true;
        if (category >= SoundLibrary.Categories.ShopEnter)
        {
            shopAnim.SetBool("Talking", true);
            Debug.Log("shop naim test");
        }
        else
        {
            anim.SetBool("Talk", true);
            Debug.Log("shop naim test");
        }
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

        float counter = 0f;//time counter for while loop              V- buffer room
        float maxTime = Mathf.Max(audioClipLength, animClipLength) + 2;//how long to stay in while loop
        
        while (counter < maxTime)
        {
            if (counter >= audioClipLength)//stop doing talk when audio ends
            {
                anim.SetBool("Talk", false);//might case issues with repeated calling 
            }

            counter += Time.deltaTime;
            yield return null;
        }

        //might need this just in case
        //ResetGameAnimationParameter();

        anim.SetBool("Talk", false);
        shopAnim.SetBool("Talking", false);
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
        anim.SetInteger("SelectedAnimation", -1);
        anim.SetInteger("SelectedAnimCategory", 0);
        anim.SetBool("PlayAnim", false);
    }

    public void PlaySteam1() // animation event
    {
        SoundObjectPool.instance.GetPoolObject().Play(audioClips[0]);
    }
    public void PlaySteam2() // animation event
    {
        SoundObjectPool.instance.GetPoolObject().Play(audioClips[1]);
    }
    public void PlayImpact() // animation event
    {
        SoundObjectPool.instance.GetPoolObject().Play(audioClips[2]);
    }
}
