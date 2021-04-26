using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBoxScript : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public float TimeBetweenMessages = 0.5f;
    public GameObject UIGroup;

    private bool messageBoxActive = false;

    public void Update()
    {
        if (messageBoxActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                messageBoxActive = false;
            }
        }
    }

    public IEnumerator ShowMessages(List<string> messages)
    {
        Time.timeScale = 0f;

        foreach (string msg in messages)
        {
            // Show Message Box
            messageBoxActive = true;
            ShowMessageBox(msg);
            
            // Wait for click
            while (messageBoxActive)
            {
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }
            
            // Hide Message Box
            HideMessageBox();
            
            // Pause a little bit
            yield return new WaitForSecondsRealtime(TimeBetweenMessages);
        }
        
        Time.timeScale = 1f;
    }

    public void ShowMessageBox(string text)
    {
        UIGroup.SetActive(true);
        tmp.text = text;
    }

    public void HideMessageBox()
    {
        UIGroup.SetActive(false);
        tmp.text = "";
    }
}
