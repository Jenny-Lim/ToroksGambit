using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddPieceInterrupt", menuName = "ScriptableObjects/AddPieceInterrupt", order = 1)]
public class AddPieceInterrupt : ScriptableObject, IInterrupt
{
    publicint PieceType;
    public Vector2 placeAt;
    public int placeOnTurn;

    public void Enact()
    {
        //throw new System.NotImplementedException();
    }
}
