using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private string currentLevelName;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        GoToMainMenu();
    }

    public void GoToMainMenu()
    {
        StartCoroutine("LoadScene", "MainMenu");
    }

    public void StartNewGame()
    {
        StartCoroutine("LoadScene", "SampleScene");
    }

    IEnumerator LoadScene(string levelName)
    {
        if (!string.IsNullOrEmpty(currentLevelName))
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentLevelName);
            while (!asyncUnload.isDone)
            {
                yield return null;
            }
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));

        currentLevelName = levelName;
    }
}
