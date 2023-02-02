using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeadMovements : MonoBehaviour
{
    private bool movementInProgress = false;
    private Vector3 initialRotation;
    [SerializeField] private Vector3 LookAtTorokRotation;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Vector2 LookAtShopRotation;

    private void Start()
    {
        initialRotation = transform.eulerAngles;
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
        if (!movementInProgress)
        {
            StartCoroutine(LookAtShopCoRo());
        }
    }

    public void LookAtBoard()
    {
        if (!movementInProgress)
        {
            StartCoroutine(LookAtBoardCoRo());
        }
    }

    private IEnumerator LookAtShopCoRo()
    {
        movementInProgress = true;
        while (Vector3.Distance(transform.eulerAngles, LookAtShopRotation) > 0.1f)// move to look at shop
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, LookAtShopRotation, speed * Time.deltaTime);
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
            yield return null;
        }
        movementInProgress = false;
    }

}
