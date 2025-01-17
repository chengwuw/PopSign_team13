﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using InitScriptName;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AnimationManager : MonoBehaviour
{
    public bool PlayOnEnable = true;
    public int PRACTICE_LEVEL_INTERVAL = 5;
    private static bool isIndian = true; // Global boolean variable
    System.Collections.Generic.Dictionary<string, string> parameters;

    void OnEnable()
    {
        if (PlayOnEnable)
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[0]);
        }
    }

    void OnDisable()
    {

    }

    public void OnFinished()
    {

    }

    public void PlaySoundButton()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
    }

    public IEnumerator Close()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void CloseMenu()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (gameObject.name == "MenuComplete" || gameObject.name == "MenuGameOver")
        {
            LogPlayTime();
            SceneManager.LoadScene("map");
        }
        else if (gameObject.name == "MenuInGamePause")
        {
            GamePlay.Instance.GameStatus = GameState.Playing;
        }
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[1]);
        gameObject.SetActive(false);
    }

    private void LogPlayTime()
    {
        string playDates;
        string theDate = System.DateTime.Now.ToString("yyyyMMdd");
        float timePlayed = Time.time;

        if (PlayerPrefs.HasKey(theDate))
        {
            float timeAlreadyPlayed = PlayerPrefs.GetFloat(theDate);
            timePlayed += timeAlreadyPlayed;
        }

        PlayerPrefs.SetFloat(theDate, timePlayed);

        if (PlayerPrefs.HasKey("PlayDates"))
        {
            playDates = PlayerPrefs.GetString("PlayDates");

            if (!playDates.Contains(theDate))
                playDates += "," + theDate;
        }
        else
        {
            playDates = theDate;
        }

        PlayerPrefs.SetString("PlayDates", playDates);
    }

    public void Play()
    {
		isIndian = PlayerPrefs.GetInt("UseIndianLevels", 0) == 1; // Default is false (0)
        Debug.Log(gameObject.name);
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (gameObject.name == "MenuGameOver")
        {
            LogPlayTime();
            CustomizeLevelManager.switchOff();
            SceneManager.LoadScene("map");
            VideoManager.resetVideoManager();

            if (mainscript.Instance.stars > 0)
            {
                GameObject.Find("Canvas").transform.Find("CongratsModal").gameObject.SetActive(true);
                PlayerPrefs.SetInt("CongratsModalShown", 1);
            }
            else
            {
                ShowGameOver();
            }
            if (PlayerPrefs.GetInt("AllLevelsCleared", 0) == 1 && PlayerPrefs.GetInt("CongratsModalShown", 0) == 0)
            {
                GameObject.Find("Canvas").transform.Find("CongratsModal").gameObject.SetActive(true);
                PlayerPrefs.SetInt("CongratsModalShown", 1);
                PlayerPrefs.Save();
            }
        }
        else if (gameObject.name == "PracticeScreen" || gameObject.name == "MenuInGamePause" || gameObject.name == "Settings")
        {
            if (gameObject.name == "MenuInGamePause" && CustomizeLevelManager.Instance.tryingToCustomize)
            {
                CustomizeLevelManager.reset();
                SceneManager.LoadScene("wordlist");
            }
            else
            {
                if (isIndian)
                {
                    SceneManager.LoadScene("game1");
                }
                else
                {
                    SceneManager.LoadScene("game");
                }
                VideoManager.resetVideoManager();
            }
        }
        else if (gameObject.name == "NextLevel")
        {
            if (CustomizeLevelManager.Instance.tryingToCustomize)
            {
                CustomizeLevelManager.reset();
                SceneManager.LoadScene("wordlist");
            }
            else
            {
                PlayerPrefs.SetInt("OpenLevel", PlayerPrefs.GetInt("OpenLevel") + 1);
                PlayerPrefs.Save();

                int randomizeLevels = PlayerPrefs.GetInt("RandomizeLevel", 0);

                if (PlayerPrefs.GetInt("OpenLevel") % PRACTICE_LEVEL_INTERVAL == 0 || randomizeLevels == 1)
                {
                    VideoManager.resetVideoManager();

                    if (isIndian)
                    {
                        SceneManager.LoadScene("practice1");
                    }
                    else
                    {
                        SceneManager.LoadScene("practice");
                    }

                    VideoManager.resetVideoManager();

                }
                else
                {
                    if (isIndian)
                    {
                        SceneManager.LoadScene("game1");
                    }
                    else
                    {
                        SceneManager.LoadScene("game");
                    }
                    VideoManager.resetVideoManager();
                }
            }
        }
        else if (gameObject.name == "TryAgain")
        {
            if (CustomizeLevelManager.Instance.tryingToCustomize)
            {
                CustomizeLevelManager.startCustomizedLevel();
            }
            else
            {
                PlayerPrefs.SetInt("OpenLevel", PlayerPrefs.GetInt("OpenLevel"));
                PlayerPrefs.Save();
                if (isIndian)
                {
                    SceneManager.LoadScene("game1");
                }
                else
                {
                    SceneManager.LoadScene("game");
                }
                VideoManager.resetVideoManager();
            }
        }
        else if (gameObject.name == "PlayMain")
        {
            CustomizeLevelManager.switchOff();
            SceneManager.LoadScene("map");
        }
    }

    public void ShowSettings()
    {
        SceneManager.LoadScene("settings");
    }

    public void ReturnToMap()
    {
        SceneManager.LoadScene("map");
    }

    public void Next()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        CloseMenu();
    }

    void ShowGameOver()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.gameOver);
        GameObject.Find("Canvas").transform.Find("MenuGameOver").gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ShowWordList()
    {
        if (PlayerPrefs.GetInt("ReviewModalShown", 0) == 0)
        {
            PlayerPrefs.SetInt("ReviewModalShown", 1);
            GameObject.Find("Canvas").transform.Find("ReviewModal").gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("wordlist");
        }
    }

    public void CloseReview()
    {
        if (CustomizeLevelManager.Instance.tryingToCustomize)
        {
            CustomizeLevelManager.switchOff();
        }
        SceneManager.LoadScene("map");
    }

    public void CloseTutorial()
    {
        SceneManager.LoadScene("map");
    }

    public void ClosePracticeScreen()
    {
        SceneManager.LoadScene("map");
    }

    public void PauseGame(GameObject menuSettings)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (!menuSettings.activeSelf) menuSettings.SetActive(true);
        else menuSettings.SetActive(false);
        GamePlay.Instance.GameStatus = GameState.BlockedGame;
    }

    public void CloseCongrats()
    {
        GameObject.Find("Canvas").transform.Find("CongratsModal").gameObject.SetActive(false);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("settings");
    }

    public void Quit()
    {
        if (CustomizeLevelManager.Instance.tryingToCustomize)
        {
            CustomizeLevelManager.switchOff();
        }
        if (SceneManager.GetActiveScene().name == "game1" || SceneManager.GetActiveScene().name == "game")
            SceneManager.LoadScene("map");
        else
            Application.Quit();
    }
}
