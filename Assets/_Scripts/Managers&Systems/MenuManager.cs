using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [SerializeField] AudioSource buttonAudioSource;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] GameObject credits;

    [Header("Stars")]
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;
    [SerializeField] private GameObject star4;
    [SerializeField] private GameObject star5;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Star1Awarderd") == 1)
        {
            star1.SetActive(true);
        }
        if (PlayerPrefs.GetInt("Star2Awarderd") == 1)
        {
            star2.SetActive(true);
        }
        if (PlayerPrefs.GetInt("Star3Awarderd") == 1)
        {
            star3.SetActive(true);
        }
        if (PlayerPrefs.GetInt("Star4Awarderd") == 1)
        {
            star4.SetActive(true);
        }
        if (PlayerPrefs.GetInt("Star5Awarderd") == 1)
        {
            star5.SetActive(true);
        }
    }

    public void LoadScene(string sceneName)
    {
        PlayerPrefs.SetInt("Hidden", 0);
        PlayerPrefs.SetInt("Retry", 0);
        PlayerPrefs.SetInt("DayToLoad", 1);
        PlayerPrefs.SetInt("HiJohn", 0);
        PlayerPrefs.SetInt("Smiley", 0);

        PlayerPrefs.SetInt("Star1", 1);
        PlayerPrefs.SetInt("Star2", 1);
        PlayerPrefs.SetInt("Star3", 1);
        PlayerPrefs.SetInt("Star4", 1);
        PlayerPrefs.SetInt("Star5", 1);

        PlayButtonSound();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        PlayButtonSound();
        Application.Quit();
    }

    public void ToggleCredits()
    {
        credits.SetActive(!credits.activeSelf);
        PlayButtonSound();
    }

    private void PlayButtonSound()
    {
        buttonAudioSource.PlayOneShot(buttonSound);
    }
}
