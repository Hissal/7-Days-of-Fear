using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private Image backGround;
    [SerializeField] private Sprite bg1;
    [SerializeField] private Sprite bg2;
    [SerializeField] private Sprite bg3;

    private bool bg2AsActive = false;
    private bool bg3AsActive = false;

    [SerializeField] private AudioSource bgNoiseSource;
    [SerializeField] private AudioClip bgNoise1;
    [SerializeField] private AudioClip bgNoise2;

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

        int randomChance = Random.Range(0, 100);
        if (randomChance < 1)
        {
            bg3AsActive = true;
            backGround.sprite = bg3;
            bgNoiseSource.clip = bgNoise2;
            bgNoiseSource.Play();
        }
        else if (randomChance < 5)
        {
            bg2AsActive = true;
            backGround.sprite = bg2;
            StartCoroutine(SecondTickerBg2Active());
        }
        else
        {
            StartCoroutine(SecondTickerBg1Active());
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

    IEnumerator SecondTickerBg1Active()
    {
        while (!bg2AsActive && !bg3AsActive)
        {
            int randomChance = Random.Range(0, 300);
            if (randomChance < 1)
            {
                bgNoiseSource.clip = bgNoise2;
                bgNoiseSource.Play();

                backGround.sprite = bg3;
                yield return new WaitForSeconds(0.25f);
                randomChance = Random.Range(0, 100);
                if (randomChance < 5)
                {
                    bg3AsActive = true;
                    yield break;
                }
                else
                {
                    bgNoiseSource.clip = bgNoise1;
                    bgNoiseSource.Play();
                    backGround.sprite = bg1;
                }
            }
            else if (randomChance < 5)
            {
                backGround.sprite = bg2;
                yield return new WaitForSeconds(0.25f);
                backGround.sprite = bg1;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator SecondTickerBg2Active()
    {
        while (!bg3AsActive)
        {
            int randomChance = Random.Range(0, 300);

            if (randomChance < 1)
            {
                bgNoiseSource.clip = bgNoise2;
                bgNoiseSource.Play();

                backGround.sprite = bg3;
                yield return new WaitForSeconds(0.25f);
                randomChance = Random.Range(0, 100);
                if (randomChance < 5)
                {
                    bg3AsActive = true;
                    yield break;
                }
                else
                {
                    bgNoiseSource.clip = bgNoise1;
                    bgNoiseSource.Play();
                    backGround.sprite = bg2;
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
