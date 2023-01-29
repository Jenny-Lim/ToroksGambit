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



        if (GUILayout.Button("Save Current Board"))
        {
            loader.WriteCurrentBoard(loader.boardName);
        }

        if (GUILayout.Button("Load Board"))
        {
            loader.LoadBoard(loader.boardName);
        }
        
    }
}
