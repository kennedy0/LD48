using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject PlayWater;
    public GameObject PlayLand;
    
    private void Start()
    {
        if (PlayerPrefs.GetInt("Tutorial01") != 1)
        {
            PlayLand.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Tutorial02") != 1)
        {
            PlayWater.SetActive(false);
        }
    }

    public void StartDive()
    {
        // Tutorial 01
        if (PlayerPrefs.GetInt("Tutorial01") != 1)
        {
            SceneManager.LoadScene("TutorialWater");
        }
        else if (PlayerPrefs.GetInt("Tutorial03") != 1)
        {
            SceneManager.LoadScene("TutorialWater2");
        }
        else
        {
            // Default behavior
            SceneManager.LoadScene("WaterLevel");
        }
    }
    
    public void StartDig()
    {
        // Tutorial 02
        if (PlayerPrefs.GetInt("Tutorial02") != 1)
        {
            SceneManager.LoadScene("TutorialLand");
        }
        else if (PlayerPrefs.GetInt("Tutorial03") != 1)
        {
            SceneManager.LoadScene("TutorialLand2");
        }
        else
        {
            // Default behavior
            SceneManager.LoadScene("LandLevel");
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void ClearGameData()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
