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
    [SerializeField] private Vector3 lookAtShopPosition;
    [SerializeField] private Vector3 titleScreenPosition;
    public static CameraHeadMovements instance;
    //private Animator ani;
    public bool menuDone = false;

    private void Awake()
    {
        //initialRotation = transform.eulerAngles;
        initialRotation = new Vector3(50.885f, 0, 0);
        //ani = gameObject.GetComponent<Animator>();
        if (instance == null ) { instance = this; }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))// just for testing purposes
        {
            LookAtTorok(2);
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
    private IEnumerator LookAtTorokCoRo(float lookAtDuration)
    {
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
            StopAllCoroutines();
            StartCoroutine(LookAtShopCoRo());
    }

    public void LookAtBoard()
    {
        StopAllCoroutines();
        StartCoroutine(LookAtBoardCoRo());
    }

    private IEnumerator LookAtShopCoRo()
    {
        movementInProgress = true;
        while (Vector3.Distance(transform.eulerAngles, LookAtShopRotation) > 0.1f)// move to look at shop
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, LookAtShopRotation, speed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, lookAtShopPosition, speed * Time.deltaTime);
            yield return null;
        }
        movementInProgress = false;
    }

    private IEnumerator LookAtBoardCoRo()
    {
        movementInProgress = true;
        while (Vector3.Distance(transform.eulerAngles, initialRotation) > 0.1f)// move to look at initial position
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, initialRotation, speed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, lookAtBoardPosition, speed * Time.deltaTime);
            yield return null;
        }
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
        movementInProgress = true;
        //ani.SetBool("StartPressed", MainMenu.instance.startPressed);

        //while (Vector3.Distance(transform.eulerAngles, LookAtTorokRotation) > 0.1f)
        while (Vector3.Distance(transform.position, lookAtBoardPosition) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, lookAtBoardPosition, speed * Time.deltaTime);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, initialRotation, speed * Time.deltaTime);
            yield return null;
        }

        //yield return new WaitUntil(delegate { return ani.GetCurrentAnimatorStateInfo(0).IsName("CameraIdle"); });
        menuDone = true;
        movementInProgress = false;
    }

    public void GetOutPlayArea()
    {
        Inventory.instance.DisableDeployUI();
        StopAllCoroutines();
        //if (!movementInProgress)
        //{
        StartCoroutine(GetOutPlayAreaCoRo());
        //}
    }

    private IEnumerator GetOutPlayAreaCoRo()
    {
        movementInProgress = true;
        while (Vector3.Distance(transform.position, titleScreenPosition) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, titleScreenPosition, speed * Time.deltaTime);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.zero, speed * Time.deltaTime);
            yield return null;
        }
        movementInProgress = false;
    }

    public bool GetIsMoving()
    {
        return movementInProgress;
    }

    //looks only at torok, doesnt go back down once fully looking
    public IEnumerator LookAtTorokExclusively()
    {
        movementInProgress = true;

        float timeElapsed = 0f;
        float percentDone = 0f;
        float desiredTime = 1.5f;

        Vector3 startPos = transform.position;
        Vector3 startAngle = transform.eulerAngles;

        while (percentDone < 1.0f)
        {
            timeElapsed += Time.deltaTime * speed;
            percentDone = timeElapsed / desiredTime;
            transform.position = Vector3.Lerp(startPos, lookAtBoardPosition, percentDone);
            transform.eulerAngles = Vector3.Lerp(startAngle, LookAtTorokRotation, percentDone);
            yield return null;
        }
        transform.position = lookAtBoardPosition;
        transform.eulerAngles = LookAtTorokRotation;
    }

    public IEnumerator LookAtBoardExclusively()
    {
        movementInProgress = true;

        float timeElapsed = 0f;
        float percentDone = 0f;
        float desiredTime = 1.5f;

        Vector3 startPos = transform.position;
        Vector3 startAngle = transform.eulerAngles;

        while (percentDone < 1.0f)
        {
            timeElapsed += Time.deltaTime * speed;
            percentDone = timeElapsed / desiredTime;
            transform.position = Vector3.Lerp(startPos, lookAtBoardPosition, percentDone);
            transform.eulerAngles = Vector3.Lerp(startAngle, initialRotation, percentDone);
            yield return null;
        }
        transform.position = lookAtBoardPosition;
        transform.eulerAngles = initialRotation;
    }
}
