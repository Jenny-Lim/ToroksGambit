using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton; // to change once there's stuff
    [SerializeField] public PauseMenu pauseFxn;

    [SerializeField] private GameObject MainMenuButtons;
    [SerializeField] private GameObject OptionsButtons;

    [SerializeField] public Slider volumeSlider;
    [SerializeField] AudioClip buttonAudioClip;

    public static MainMenu instance;
    //public bool startPressed;

    private void Start()
    {
        if (instance == null) { instance = this; }

        //GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);

        // to change once there's stuff
        //if(SaveManager.instance.hasSaveGame)
        //{
        //    continueButton.interactable = true;

        //}
        //else 
        //{
            //continueButton.interactable = false;
        //}
        //continueButton.interactable = true;
        //optionsButton.interactable = false;
        pauseFxn.enabled = false;
        //startPressed = false;
        volumeSlider.value = SaveManager.instance.savedVolume;

    }



    void OnEnable()
    {
        TorokPersonalityAI.instance.ResetGameAnimationParameter();
        TorokPersonalityAI.instance.anim.SetBool("InTitle", true); // just for torok
        startButton.interactable = false;
        continueButton.interactable = false;
        //print("yeye");
        Invoke("GoBackToTitle", 1.0f); // it was some shit with execution order
        //GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);
        CameraHeadMovements.instance.menuDone = false;
    }

    void GoBackToTitle()
    {
        startButton.interactable = true; //sometimes intro doesnt play -- if you press play when not in the title coro. so heres a bandaid fix
        if (SaveManager.instance.hasSaveGame)
        {
            continueButton.interactable = true;
        }
        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);
    }

    public void NewGame() // have yet to test fully
    {
        //GameManager.instance.StartNewGame();
        // get first level

        //startPressed = true;

        // reset torok
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
        TorokPersonalityAI.instance.SetAngerLevel(1);
        InterruptManager.instance.ResetInterruptListTriggers();

        GameStateManager.instance.currentLevelNumber = -1;
        GameStateManager.instance.SetNextLevel(); // bug possibly here

        // reset inventory
        Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)0, 1); // pawns
        for (int i = 1; i < 3; i++)
        {
            Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)i, 0); // bishop + knight
        } // no king / queen / rook
        Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)3, 0); // bishop + knight
        Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)4, 0); // bishop + knight


        // reset currency
        Currency.instance.SetCurrency(0);
        SaveManager.instance.StartNew();
        Inventory.instance.updateCountText();
        Board.instance.ResetBoardPositions();
        Board.instance.ClearMoveTiles();
        // move to proper area
        //if (GameStateManager.instance.currentState == GameStateManager.GameState.title)
        //{
            //print("sup");

        CurtainManager.instance.OpenCurtains();

            
            //CameraHeadMovements.instance.LookAtPlayArea(); - CALLED IN CURTAIN OPEN ANIMATION AS AN EVENT!!
            //CHECK CURTAIN CALLCAMERAMOVE IF ANY CHANGE NEEDS TO BE DONE TO LookAtPlayArea() !!!
        //}

        gameObject.SetActive(false); // hide main menu
    }

    public void ContinueGame()
    {
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
        SaveManager.instance.LoadSave();
        Inventory.instance.updateCountText();
        GameStateManager.instance.SetNextLevel(); // bug possibly here
        Board.instance.ClearMoveTiles();
        
        CurtainManager.instance.OpenCurtains();
        
        //CameraHeadMovements.instance.LookAtPlayArea(); - CALLED IN CURTAIN OPEN ANIMATION AS VENT!!
        gameObject.SetActive(false); // hide main menu

        // nothing atm -- should save level, angerlvl, interrupt thing??, inventory, currency
    }

    public void Save()
    {
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
        AudioManager.instance.SetVolume(volumeSlider.value);
        SaveManager.instance.SaveVolume(volumeSlider.value);

        OptionsButtons.SetActive(false);
        MainMenuButtons.SetActive(true);
    }

    public void Options()
    {
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
        MainMenuButtons.SetActive(false);
        OptionsButtons.SetActive(true);
    }

    public void ExitGame()
    {
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
        #if UNITY_STANDALONE
        Application.Quit();
        #endif

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
