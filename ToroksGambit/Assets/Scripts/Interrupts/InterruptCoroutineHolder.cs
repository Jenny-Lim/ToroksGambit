using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptCoroutineHolder : MonoBehaviour
{
    public enum Coroutines
    {
        AddPiece, MovePiece
    }

    private BaseInterrupt holder;
    public bool isRunning { get; private set; } = false;

    public void Initialize(BaseInterrupt thisHolder)
    {
        holder = thisHolder;
    }

    public void RunCoroutine(Coroutines coroutineType)
    {
        if (isRunning)
        {
            return;
        }

        StopAllCoroutines();
        switch (coroutineType)
        {
            case Coroutines.AddPiece:
                StartCoroutine(InterruptCoroutineAddPiece());
                break; 
            case Coroutines.MovePiece:
                StartCoroutine(InterruptCoroutineMovePiece());
                break;
        }
    }

    private IEnumerator InterruptCoroutineAddPiece()
    {

        //check if thing is possible
        AddPieceInterrupt holderType = (AddPieceInterrupt)holder;
        
        //gets out if there is a piece there, remove this and this interrupt will replace that piece regardless of team, which we might want
        if (Board.pieceBoard[holderType.placeAt.x, holderType.placeAt.y] != null)
        {
            yield break; 
        }

        holder.hasTriggered = true;//tell the interrupt it has triggered
        isRunning = true;//set this coroholder to be runnning
        GameStateManager.GameState returnState = GameStateManager.instance.GetGameState();//get what state game was in before coro runs
        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.Interrupt);//set game state to interrupt
        yield return CameraHeadMovements.instance.StartCoroutine(CameraHeadMovements.instance.LookAtTorokExclusively());

        yield return TorokPersonalityAI.instance.StartCoroutine(TorokPersonalityAI.instance.PlayAnimationAndSoundCoRo(SoundLibrary.Categories.Interrupt));//this plays the animation of looking at torok and stuff

        if ((int)holderType.piece > (int)Piece.PieceType.king)
        {
            Board.instance.PlaceObstacle(holderType.placeAt.x, holderType.placeAt.y, (int)holderType.piece);
        }
        else
        {
            Board.instance.PlacePieceTorok(holderType.placeAt.x, holderType.placeAt.y, (int)holderType.piece);
        }

        GameObject placedPiece = Board.pieceBoard[holderType.placeAt.x, holderType.placeAt.y];// manipulate piece GO for animation effect thingy

        //return camera to board can be placed here for before effect 
        yield return CameraHeadMovements.instance.StartCoroutine(CameraHeadMovements.instance.LookAtBoardExclusively());

        //***this would be where we do some type of effect like smoke appears or the piece falls from the sky or something***

        //return camera to board can be placed here for after effect 


        GameStateManager.instance.ChangeGameState(returnState);//change game state back to what it was before this ran
        isRunning = false;//set coro running to false
        yield return null;

    }

    private IEnumerator InterruptCoroutineMovePiece()
    {
        //check if thing is possible
        MovePieceInterrupt holderType = (MovePieceInterrupt)holder;
        if (!Piece.InBoundsCheck(holderType.moveFrom.x, holderType.moveFrom.y) || !Piece.InBoundsCheck(holderType.moveTo.x, holderType.moveTo.y))
        {
            yield break;
        }
        if (Board.pieceBoard[holderType.moveFrom.x, holderType.moveFrom.y] == null)
        {
            yield break;
        }

        holder.hasTriggered = true;//tell the interrupt it has triggered
        isRunning = true;//set this coroholder to be runnning
        GameStateManager.GameState returnState = GameStateManager.instance.GetGameState();//get what state game was in before coro runs
        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.Interrupt);//set game state to interrupt
        yield return CameraHeadMovements.instance.StartCoroutine(CameraHeadMovements.instance.LookAtTorokExclusively());

        yield return TorokPersonalityAI.instance.StartCoroutine(TorokPersonalityAI.instance.PlayAnimationAndSoundCoRo(SoundLibrary.Categories.Interrupt));//this plays the animation of looking at torok and stuff

        yield return Board.instance.StartCoroutine(Board.instance.MovePieceVisual(holderType.moveFrom.x, holderType.moveFrom.y, holderType.moveTo.x, holderType.moveTo.y));//move the piece visually
        //some effect


        Board.instance.MovePiece(holderType.moveFrom.x, holderType.moveFrom.y, holderType.moveTo.x, holderType.moveTo.y);

        yield return CameraHeadMovements.instance.StartCoroutine(CameraHeadMovements.instance.LookAtBoardExclusively());

        GameStateManager.instance.ChangeGameState(returnState);//change game state back to what it was before this ran
        isRunning = false;//set coro running to false
        yield return null;

    }

}
