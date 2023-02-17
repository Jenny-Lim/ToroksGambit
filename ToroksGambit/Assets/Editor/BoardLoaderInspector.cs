using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BoardLoader))]
public class BoardLoaderInspector : Editor
{
    public string BoardName;
    

    public void Awake()
    {
        BoardLoader loader = (BoardLoader)target;
        loader.savedBoardNames = loader.GetAllSavedBoardNames();
    }

    public override void OnInspectorGUI()
    {
        BoardLoader loader = (BoardLoader)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Current Board"))
        {
            loader.WriteCurrentBoard(loader.boardName);
            loader.savedBoardNames = loader.GetAllSavedBoardNames();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Load Board"))
        {
            loader.LoadBoard(loader.boardName);
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Clear Board"))
        {
            Board.instance.ClearBoard();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Delete Board"))
        {
            loader.DeleteSavedBoard(loader.boardName);
            loader.savedBoardNames = loader.GetAllSavedBoardNames();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset Game To Deploy State"))
        {
            GameStateManager.instance.ResetGameDeploy();
        }

        if (GUILayout.Button("Reset Game To Game State"))
        {
            GameStateManager.instance.ResetGame();
        }

        GUILayout.EndHorizontal();

        base.OnInspectorGUI();

    }
}
