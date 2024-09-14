using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject credits;

    public void LoadScene(string sceneName)
    {
        PlayerPrefs.SetInt("Hidden", 0);
        PlayerPrefs.SetInt("Retry", 0);
        PlayerPrefs.SetInt("DayToLoad", 4);
        PlayerPrefs.SetInt("HiJohn", 0);
        PlayerPrefs.SetInt("Smiley", 0);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleCredits()
    {
        credits.SetActive(!credits.activeSelf);
    }
}
