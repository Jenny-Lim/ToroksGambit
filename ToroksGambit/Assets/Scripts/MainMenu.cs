using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{

    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton; // to change once there's stuff
    [SerializeField] public PauseMenu pauseFxn;

    public static MainMenu instance;
    //public bool startPressed;

    private void Start()
    {
        if (instance == null) { instance = this; }

        //GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);

        // to change once there's stuff
        continueButton.interactable = false;
        optionsButton.interactable = false;
        pauseFxn.enabled = false;
        //startPressed = false;
    }

    void OnEnable()
    {
        print("yeye");
        Invoke("GoBackToTitle", 1.0f); // it was some shit with execution order
        //GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);
        CameraHeadMovements.instance.menuDone = false;
    }

    void GoBackToTitle()
    {
        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);
    }

    public void NewGame() // have yet to test fully
    {
        //GameManager.instance.StartNewGame();
        // get first level

        //startPressed = true;

        // reset torok
        TorokPersonalityAI.instance.SetAngerLevel(1);
        InterruptManager.instance.ResetInterruptListTriggers();

        GameStateManager.instance.currentLevelNumber = -1;
        GameStateManager.instance.SetNextLevel(); // bug possibly here

        // reset inventory
        /*
        Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)0, 4); // pawns
        for (int i = 1; i < 3; i++)
        {
            Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)i, 5); // others (no king)
        }
        Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)4, 1); // queen
        */

        // reset currency
        Currency.instance.SetCurrency(0);

        // move to proper area
        CameraHeadMovements.instance.LookAtPlayArea();

        gameObject.SetActive(false); // hide main menu
    }

    public void ContinueGame()
    {
        // nothing atm -- should save level, angerlvl, interrupt thing??, inventory, currency
    }

    public void Options()
    {
        // nothing atm -- probably difficulty settings, audio, dialogue / cutscenes
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
