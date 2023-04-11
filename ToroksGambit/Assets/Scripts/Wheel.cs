using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 2.5f * Time.deltaTime, 0);
    }
}
