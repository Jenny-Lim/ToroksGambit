using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployLines : MonoBehaviour
{
    [SerializeField] private GameObject lines;
    public GameObject left, right, up, down;
    public GameObject lNeighbor, rNeighbor, upNeighbor, downNeighbor; // set in board

    // Start is called before the first frame update
    void Awake()
    {
        left = lines.transform.Find("DeploySpotLine_L").gameObject;
        right = lines.transform.Find("DeploySpotLine_R").gameObject;
        up = lines.transform.Find("DeploySpotLine_UP").gameObject;
        down = lines.transform.Find("DeploySpotLine_DOWN").gameObject;
    }
}
