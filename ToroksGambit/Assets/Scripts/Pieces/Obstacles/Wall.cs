using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Piece // dummy piece, pieces can't land on or jump over this
{
    // Start is called before the first frame update
    void Start()
    {
        type = "wall";
    }

}
