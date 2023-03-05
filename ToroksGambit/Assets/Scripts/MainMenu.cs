using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{

    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton; // to change once there's stuff


    public static MainMenu instance;
    //public bool startPressed;

    private void Start()
    {
        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);
        //startPressed = false;
        if (instance == null) { instance = this; }
    }

    void OnEnable()
    {
        continueButton.interactable = false;
        optionsButton.interactable = false; // to change once there's stuff
    }

    public void NewGame() // have yet to test fully
    {
        //GameManager.instance.StartNewGame();
        // get first level

        //startPressed = true;

        GameStateManager.instance.currentLevelNumber = -1;
        GameStateManager.instance.SetNextLevel();

        // reset torok
        TorokPersonalityAI.instance.SetAngerLevel(1);
        InterruptManager.instance.ResetInterruptListTriggers();

        // reset inventory
        Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)0, 4); // pawns
        for (int i = 1; i < 3; i++)
        {
            Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)i, 5); // others (no king)
        }
        Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)4, 1); // queen

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
