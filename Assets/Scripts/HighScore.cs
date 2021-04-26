using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (PlayerPrefs.HasKey("highscore"))
        {
            tmp.text = $"{PlayerPrefs.GetInt("highscore")} m";
        }
    }
}
