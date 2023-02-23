using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{

    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton; // to change once there's stuff

    void OnEnable()
    {
        continueButton.interactable = false;
        optionsButton.interactable = false; // to change once there's stuff
    }

    public void NewGame()
    {
        GameManager.instance.StartNewGame();
    }

    public void ContinueGame()
    {
        // nothing atm
    }

    public void Options()
    {
        // nothing atm
    }

    public void ExitGame()
    {
        #if UNITY_STANDALONE
        Application.Quit();
        #endif

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
