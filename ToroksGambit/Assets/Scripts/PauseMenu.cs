using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool escPressed = false;
    public static PauseMenu instance;
    [SerializeField] private GameObject PauseMenuButtons;
    [SerializeField] private GameObject PauseOptionsButtons;

    [SerializeField] public Slider pauseVolumeSlider;
    [SerializeField] AudioClip buttonAudioClip;

    //[SerializeField] private Button optionsButton;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        //optionsButton.interactable = false;
    }

    void OnEnable()
    {
        CameraHeadMovements.canScroll = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !escPressed)
        {
            escPressed = true;
            pauseMenu.SetActive(true);
            pauseVolumeSlider.value = SaveManager.instance.savedVolume;
            //Time.timeScale = 0;

        }
        else if (Input.GetKeyDown(KeyCode.Escape) && escPressed)
        {
            escPressed = false;
            pauseMenu.SetActive(false);
            //Time.timeScale = 1;
        }
    }

    public void Resume()
    {
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
        pauseMenu.SetActive(false);
        CameraHeadMovements.canScroll = true;
        //Time.timeScale = 1;
    }

    public void Options() // this is kind of iffy
    {
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
        PauseMenuButtons.SetActive(false);
        PauseOptionsButtons.SetActive(true);
        //SaveManager.instance.SaveGame();
        //ReturnToMainMenu();
        //MainMenu.instance.Options();
    }

    public void SavePauseOptions()
    {
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
        AudioManager.instance.SetVolume(pauseVolumeSlider.value);
        SaveManager.instance.SaveVolume(pauseVolumeSlider.value);
        MainMenu.instance.volumeSlider.value = SaveManager.instance.savedVolume;
        PauseOptionsButtons.SetActive(false);
        PauseMenuButtons.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        //Time.timeScale = 1;
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
        CameraHeadMovements.canScroll = false;
        Inventory.instance.objectiveArea.SetActive(false);
        Inventory.instance.startButton.SetActive(false);
        LegendUI.instance.DisableLegendObject();
        Inventory.instance.DisableDeployUI();
        Inventory.instance.HideInventoryPanel();
        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);
        CameraHeadMovements.instance.GetOutPlayArea();
        MainMenu.instance.gameObject.SetActive(true);
        pauseMenu.SetActive(false);
        this.enabled = false;
    }
}
