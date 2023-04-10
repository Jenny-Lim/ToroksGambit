using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainManager : MonoBehaviour
{

    public static CurtainManager instance;

    private Animation curtainAnim;

    public void Start()
    {
        instance = this;
        curtainAnim = GetComponent<Animation>();
    }

    public void OpenCurtains()
    {
        curtainAnim.Play("CurtainsOpenFinal");
        //curtainOne.Play("CurtainOpenFinal");
        //curtainTwo.Play("CurtainOpenFinal");
    }

    public void CloseCurtains() //CALLED IN LOOKATPLAYAREA !!!
    {
        curtainAnim.Play("CurtainsCloseFinal");
        //curtainOne.Play("CurtainCloseFinal");
        //curtainTwo.Play("CurtainCloseFinal");
    }

    public void CallCameraMove()
    {
        CameraHeadMovements.instance.LookAtPlayArea();
    }
}