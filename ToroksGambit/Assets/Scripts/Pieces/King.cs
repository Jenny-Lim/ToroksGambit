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

    public override void UpdateMoves()
    {
        moves.Clear();

        if (InBoundsCheck(pieceX + 1, pieceY + 1))
        {
            moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 1]));
        }

        if (InBoundsCheck(pieceX - 1, pieceY - 1))
        {
            moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY - 1]));
        }

        if (InBoundsCheck(pieceX + 1, pieceY))
        {
            moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY]));
        }

        if (InBoundsCheck(pieceX, pieceY + 1))
        {
            moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + 1]));
        }

        if (InBoundsCheck(pieceX - 1, pieceY))
        {
            moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY]));
        }

        if (InBoundsCheck(pieceX, pieceY - 1))
        {
            moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - 1]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTaken)
        {
            this.gameObject.SetActive(false);
            // match over if there's a king
        }
    }
}
