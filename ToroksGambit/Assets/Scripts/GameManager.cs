using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private string currentSceneName;

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

    IEnumerator LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentSceneName);
            while (!asyncUnload.isDone)
            {
                yield return null;
            }
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        currentSceneName = sceneName;
    }
}
