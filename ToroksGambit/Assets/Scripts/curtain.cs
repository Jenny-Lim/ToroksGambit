using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class curtain : MonoBehaviour
{
    public void callCameraMove()
    {
        CameraHeadMovements.instance.LookAtPlayArea();
    }
}
