using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeadMovements : MonoBehaviour
{
    private bool movementInProgress = false;
    private Vector3 initialRotation;
    [SerializeField] private Vector3 LookAtTorokRotation;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Vector3 LookAtShopRotation;
    [SerializeField] private Vector3 lookAtBoardPosition;
    [SerializeField] private Vector3 lookAtBoardRotation;
    [SerializeField] private Vector3 lookAtShopPosition;
    [SerializeField] private Vector3 titleScreenPosition;
    public static CameraHeadMovements instance;

    [SerializeField] private Vector3 maxScrollPos;
    private Vector3 minScrollPos;
    [SerializeField] private float scrollSpeed = 1;
    public float scrollPercent = 0.0f;
    public static bool canScroll = false;

    //private Animator ani;
    public bool menuDone = false;

    private void Awake()
    {
        initialRotation = transform.eulerAngles;
        titleScreenPosition = transform.position;
        //ani = gameObject.GetComponent<Animator>();
        if (instance == null ) { instance = this; }
        minScrollPos = lookAtBoardPosition;
    }

    private void Update()
    {
        //print("canScroll: " + canScroll);
        //if (Input.GetButtonDown("Jump"))// just for testing purposes
        //{
        //    LookAtTorok(2);
        //}

        if (!movementInProgress && canScroll)//if not moving by coro
        {
            //Debug.Log(canScroll);
            scrollPercent += Input.mouseScrollDelta.y * Time.deltaTime * scrollSpeed;
            scrollPercent = Mathf.Clamp01(scrollPercent);
            transform.position = Vector3.Lerp(minScrollPos, maxScrollPos, scrollPercent);
        }

    }

    //calls the coro for looking at torok, but checks if it can beforehand
    public void LookAtTorok(float forHowLong)
    {
        if (!movementInProgress)
        {
            StartCoroutine(LookAtTorokCoRo(forHowLong));
        }
        
    }

    //coro which moves the camera towards torok, stops for x seconds, then looks back at the board
    public IEnumerator LookAtTorokCoRo(float lookAtDuration)
    {
        Debug.Log("insdie LookAtTorok");
        movementInProgress = true;//inform that movement is in progress

        while (Vector3.Distance(transform.eulerAngles, LookAtTorokRotation) > 0.1f)// move to look at torok
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, LookAtTorokRotation, speed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(lookAtDuration);//look at torok for lookAtDuration seconds

        while (Vector3.Distance(transform.eulerAngles, initialRotation) > 0.1f)// move to look at initial position
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, initialRotation, speed * Time.deltaTime);
            yield return null;
        }

        movementInProgress = false;
    }


    // below -- jenny added
    public void LookAtShop()
    {
        //StopAllCoroutines();
        if (!movementInProgress)
        {
            StartCoroutine(LookAtShopCoRo());
        }
    }

    public void LookAtBoard()
    {
            //StopAllCoroutines();
            if (!movementInProgress)
            {
                StartCoroutine(LookAtBoardCoRo());
            }
    }

    private IEnumerator LookAtShopCoRo()
    {
        movementInProgress = true;
        //while (Vector3.Distance(transform.eulerAngles, LookAtShopRotation) > 0.1f)// move to look at shop
        //{
        //    transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, LookAtShopRotation, speed * Time.deltaTime);
        //    transform.position = Vector3.Lerp(transform.position, lookAtShopPosition, speed * Time.deltaTime);
        //    yield return null;
        //}
        float elapsedTime = 0f;
        float desiredTime = 1.5f;
        float percentDone = 0f;

        Vector3 initPos = transform.position;
        Vector3 initRot = transform.eulerAngles;
        while (percentDone <= 1.0f)
        {
            elapsedTime += Time.deltaTime * speed * 0.5f;//<- just to make it a litte slower
            percentDone = elapsedTime / desiredTime;
            //transform.eulerAngles = Vector3.Slerp(initRot, lookAtBoardRotation, percentDone);
            transform.eulerAngles = AngleLerp(initRot, LookAtShopRotation, percentDone);
            transform.position = Vector3.Lerp(initPos, lookAtShopPosition, percentDone);
            yield return null;
        }
        transform.eulerAngles = LookAtShopRotation;
        transform.position = lookAtShopPosition;
        TorokPersonalityAI.instance.PlaySoundFromCategory(SoundLibrary.Categories.ShopEnter);

        movementInProgress = false;
    }

    public IEnumerator LookAtBoardCoRo()
    {
        movementInProgress = true;
        /*while (Vector3.Distance(transform.eulerAngles, initialRotation) > 0.1f)// move to look at initial position
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, initialRotation, speed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, lookAtBoardPosition, speed * Time.deltaTime);
            yield return null;
        }*/
        Debug.Log("insdie LookAtBoard");
        float elapsedTime = 0f;
        float desiredTime = 1.5f;
        float percentDone = 0f;

        Vector3 initPos = transform.position;
        Vector3 initRot = transform.eulerAngles;
        while (percentDone <= 1.0f)
        {
            elapsedTime += Time.deltaTime * speed;
            percentDone = elapsedTime / desiredTime;
            //transform.eulerAngles = Vector3.Slerp(initRot, lookAtBoardRotation, percentDone);
            transform.eulerAngles = AngleLerp(initRot, lookAtBoardRotation, percentDone);
            transform.position = Vector3.Lerp(initPos, lookAtBoardPosition, percentDone);
            yield return null;
        }
        transform.eulerAngles = lookAtBoardRotation;
        transform.position = lookAtBoardPosition;

        yield return new WaitForSeconds(1.5f);

        movementInProgress = false;
    }


    public void LookAtPlayArea()
    {
        if (!movementInProgress)
        {
            StartCoroutine(LookAtPlayAreaCoRo());
        }
    }
    public IEnumerator LookAtPlayAreaCoRo()
    {
        Debug.Log("insdie LookAtPlay");
        movementInProgress = true;
        //ani.SetBool("StartPressed", MainMenu.instance.startPressed);

        //while (Vector3.Distance(transform.eulerAngles, LookAtTorokRotation) > 0.1f)
        //while (Vector3.Distance(transform.position, lookAtBoardPosition) > 0.1f)
        //{
        //    transform.position = Vector3.Lerp(transform.position, lookAtBoardPosition, speed * Time.deltaTime);
        //    transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, initialRotation, speed * Time.deltaTime);
        //    yield return null;
        //}

        float elapsedTime = 0f;
        float desiredTime = 1.5f;
        float percentDone = 0f;

        Vector3 initPos = transform.position;
        Vector3 initRot = transform.eulerAngles;
        while (percentDone <= 1.0f)
        {
            elapsedTime += Time.deltaTime;
            percentDone = elapsedTime / desiredTime;
            //transform.eulerAngles = Vector3.Slerp(initRot, lookAtBoardRotation, percentDone);
            transform.eulerAngles = AngleLerp(initRot, lookAtBoardRotation, percentDone);
            transform.position = Vector3.Lerp(initPos, lookAtBoardPosition, percentDone);
            yield return null;
        }
        transform.position = lookAtBoardPosition;
        transform.eulerAngles = lookAtBoardRotation;

        //yield return new WaitUntil(delegate { return ani.GetCurrentAnimatorStateInfo(0).IsName("CameraIdle"); });
        menuDone = true;
        movementInProgress = false;
    }

    Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }

    public void GetOutPlayArea()
    {
        //Inventory.instance.DisableDeployUI();
        StopAllCoroutines();
        //if (!movementInProgress)
        //{
        StartCoroutine(GetOutPlayAreaCoRo());
        //}
    }

    private IEnumerator GetOutPlayAreaCoRo()
    {
        Debug.Log("get out LookAtPlay");
        movementInProgress = true;

        float elapsedTime = 0f;
        float desiredTime = 2f;
        float percentDone = 0f;

        Vector3 initPos = transform.position;
        Vector3 initRot = transform.eulerAngles;
        while (percentDone <= 1.0f)
        {
            elapsedTime += Time.deltaTime * speed;
            percentDone = elapsedTime / desiredTime;
            //transform.eulerAngles = Vector3.Slerp(initRot, lookAtBoardRotation, percentDone);
            transform.eulerAngles = AngleLerp(initRot, initialRotation, percentDone);
            transform.position = Vector3.Lerp(initPos, titleScreenPosition, percentDone);
            yield return null;
        }
        transform.eulerAngles = initialRotation;
        transform.position = titleScreenPosition; // for whatever reason it seems like this is set to the board pos ? -- works fine in shop tho lmao

        
        movementInProgress = false;
    }

    public bool GetIsMoving()
    {
        return movementInProgress;
    }

    //looks only at torok, doesnt go back down once fully looking
    public IEnumerator LookAtTorokExclusively()
    {
        Debug.Log("insdie LookATorok exl");
        movementInProgress = true;

        float timeElapsed = 0f;
        float percentDone = 0f;
        float desiredTime = 2f;

        Vector3 startAngle = transform.eulerAngles;
        //eulerBeforeExlusiveCall = transform.eulerAngles;

        while (percentDone < 1.0f)
        {
            timeElapsed += Time.deltaTime * speed;
            percentDone = timeElapsed / desiredTime;
            //transform.eulerAngles = Vector3.Slerp(startAngle, LookAtTorokRotation, percentDone);
            transform.eulerAngles = AngleLerp(startAngle, LookAtTorokRotation, percentDone);
            yield return null;
        }
        transform.eulerAngles = LookAtTorokRotation;
        movementInProgress = false;
    }

    public IEnumerator LookAtBoardExclusively()
    {
        Debug.Log("insdie LookAtBoard exl");
        movementInProgress = true;

        float timeElapsed = 0f;
        float percentDone = 0f;
        float desiredTime = 2f;

        Vector3 startAngle = transform.eulerAngles;

        while (percentDone < 1.0f)
        {
            timeElapsed += Time.deltaTime * speed;
            percentDone = timeElapsed / desiredTime;
            //transform.eulerAngles = Vector3.Slerp(startAngle, lookAtBoardRotation, percentDone);
            transform.eulerAngles = AngleLerp(startAngle, lookAtBoardRotation, percentDone);
            yield return null;
        }
        transform.eulerAngles = lookAtBoardRotation;
        movementInProgress = false;
    }
}
