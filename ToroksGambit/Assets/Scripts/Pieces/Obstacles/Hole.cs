using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : Piece // pieces can jump over these, just cannot land
{
    // Start is called before the first frame update
    void Start()
    {
        type = "hole";
    }

}
