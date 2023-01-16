using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        type = "king";
    }

    // Update is called once per frame
    void Update()
    {
        if (isTaken)
        {
            // match over
        }
    }
}
