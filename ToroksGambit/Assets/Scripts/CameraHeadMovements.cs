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
    public static CameraHeadMovements instance;
    private Animator ani;

    private void Start()
    {
        initialRotation = transform.eulerAngles;
        ani = gameObject.GetComponent<Animator>();
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
        StopAllCoroutines();
        StartCoroutine(LookAtPlayAreaCoRo());
    }
    private IEnumerator LookAtPlayAreaCoRo()
    {
        movementInProgress = true;
        //while (transform.position.z > -1.77) {
        //ani.Play("Base Layer.CameraIdle", 0, 1.0f);
        ani.SetBool("StartPressed", MainMenu.instance.startPressed);
        //}
        yield return null;
        movementInProgress = false;
    }

    public bool GetIsMoving()
    {
        return movementInProgress;
    }
}
