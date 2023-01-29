using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoardLoader))]
public class BoardLoaderInspector : Editor
{
    public string BoardName;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BoardLoader loader = (BoardLoader)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Current Board"))
        {
            loader.WriteCurrentBoard(loader.boardName);
        }

        GUILayout.Space(50);

        if (GUILayout.Button("Load Board"))
        {
            loader.LoadBoard(loader.boardName);
        }

        GUILayout.Space(50);

        if (GUILayout.Button("Clear Board"))
        {
            Board.instance.ClearBoard();
        }

        GUILayout.EndHorizontal();

    }
}
