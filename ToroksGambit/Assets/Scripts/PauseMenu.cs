using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool escPressed = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !escPressed)
        {
            escPressed = true;
            pauseMenu.SetActive(true);
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
        pauseMenu.SetActive(false);
        //Time.timeScale = 1;
    }

    public void ReturnToMainMenu()
    {
        //Time.timeScale = 1;
        CameraHeadMovements.instance.GetOutPlayArea();
        MainMenu.instance.gameObject.SetActive(true);
        pauseMenu.SetActive(false);
        this.enabled = false;
    }
}
