using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public MessageBoxScript MessageBox;

    private PlayerController playerController;
    public bool deathStarted;

    private bool oxygenTutorial;
    private bool heatTutorial;

    void Start()
    {
        oxygenTutorial = false;
        heatTutorial = false;
        deathStarted = false;
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        if (PlayerPrefs.GetInt("Tutorial01") != 1)
        {
            StartCoroutine(Tutorial01());
        }
        else if (PlayerPrefs.GetInt("Tutorial02") != 1)
        {
            StartCoroutine(Tutorial02());
        }
        else if (PlayerPrefs.GetInt("Tutorial03") != 1)
        {
            StartCoroutine(Tutorial03());
        }
    }

    void Update()
    {
        if (!deathStarted && playerController.IsDead)
        {
            deathStarted = true;

            if (PlayerPrefs.GetInt("Tutorial01") != 1)
            {
                if (playerController.Oxygen <= 0f)
                {
                    StartCoroutine(Tutorial01Death());
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            else if (PlayerPrefs.GetInt("Tutorial02") != 1)
            {
                if (playerController.Heat >= playerController.MaxHeat)
                {
                    StartCoroutine(Tutorial02Death());
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            else if (PlayerPrefs.GetInt("Tutorial03") != 1)
            {
                if (heatTutorial && oxygenTutorial)
                {
                    StartCoroutine(Tutorial03Death());
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }

        if (PlayerPrefs.GetInt("Tutorial01") == 1 && PlayerPrefs.GetInt("Tutorial02") == 1)
        {
            if (playerController.Oxygen / playerController.MaxOxygen < .45f && !oxygenTutorial)
            {
                oxygenTutorial = true;
                StartCoroutine(OxygenTutorial());
            }

            if (playerController.Heat / playerController.MaxHeat > .55f && !heatTutorial)
            {
                heatTutorial = true;
                StartCoroutine(HeatTutorial());
            }
        }
    }

    private IEnumerator Tutorial01()
    {
        yield return new WaitForSeconds(3f);
        
        List<string> m = new List<string>();
        m.Add("[Use the LEFT mouse button to drive]");
        StartCoroutine(MessageBox.ShowMessages(m));
        
        yield return new WaitForSeconds(3f);
        
        m.Clear();
        m.Add("\"Hello, can you hear me?\"");
        m.Add("\"(Is this thing on...?)\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        
        yield return new WaitForSeconds(3f);
        
        m.Clear();
        m.Add("\"You're piloting the DiveMaster 2000. My most expensive invention yet!\"");
        m.Add("\"Use it to dive DEEPER to the center of the planet.\"");
        m.Add("\"If we can be the first expedition crew to reach the core, I'll be rich!\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        
        yield return new WaitForSeconds(1f);
        
        m.Clear();
        m.Add("\"...\"");
        m.Add("\"Sorry, I mean *WE'LL* be rich.\"");
        StartCoroutine(MessageBox.ShowMessages(m));
    }
    
    private IEnumerator Tutorial01Death()
    {
        yield return new WaitForSeconds(2f);
        
        List<string> m = new List<string>();
        m.Add("\"Did...\"");
        m.Add("\"Did you just run out of air?\"");
        m.Add("\"Seriously?!\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        
        yield return new WaitForSeconds(3f);
        
        m.Clear();
        m.Add("\"Alright, abort mission.\"");
        m.Add("\"I guess it's back to the drawing board...\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
        
        // Finish Tutorial
        PlayerPrefs.SetInt("Tutorial01", 1);

        SceneManager.LoadScene("MainMenu");
    }
    
    private IEnumerator Tutorial02()
    {
        yield return new WaitForSeconds(3f);
        
        List<string> m = new List<string>();
        m.Add("[Use the LEFT mouse button to drive]");
        m.Add("[Your drill is active whenever you're moving.]");
        StartCoroutine(MessageBox.ShowMessages(m));
        
        yield return new WaitForSeconds(3f);
        
        m.Clear();
        m.Add("\"Ok, new plan.\"");
        m.Add("\"This is the Drill-Bot-XTreme.\"");
        m.Add("\"Try to not scratch the paint...\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        
        yield return new WaitForSeconds(3f);
        
        m.Clear();
        m.Add("\"See that? Your oxygen is fine!\"");
        m.Add("\"Forget diving, we're going to drill out way even DEEPER into the planet!\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        
        yield return new WaitForSeconds(3f);
        
        m.Clear();
        m.Add("\"Hey, watch that temperature gauge.\"");
        m.Add("\"You're starting to overheat!\"");
        StartCoroutine(MessageBox.ShowMessages(m));
    }
    
    private IEnumerator Tutorial02Death()
    {
        yield return new WaitForSeconds(2f);
        
        List<string> m = new List<string>();
        m.Add("\"Are you KIDDING me?!\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        
        yield return new WaitForSeconds(2f);
        
        m.Clear();
        m.Add("\"Do you know how much these things cost?!\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
        
        m.Clear();
        m.Add("\"(Deep breaths. In and out.)\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
        
        m.Clear();
        m.Add("\"Ok, let's regroup. I've got one more idea.\"");
        m.Add("\"It's just crazy enough to work...\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
        
        // Finish Tutorial
        PlayerPrefs.SetInt("Tutorial02", 1);

        SceneManager.LoadScene("MainMenu");
    }
    
    private IEnumerator Tutorial03()
    {
        yield return new WaitForSeconds(3f);
        
        List<string> m = new List<string>();
        m.Add("[Use the LEFT mouse button to drive]");
        m.Add("[Your drill is now active underwater.]");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(3f);
        
        
        m.Clear();
        m.Add("\"I spent every last penny I had developing this new vehicle.\"");
        m.Add("\"I call it the...\"");
        m.Add("\"Dive & Dig!\"");
        m.Add("\"This baby can drill through solid rock, and navigate underwater.");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
        
        m.Clear();
        m.Add("\"Now we can go DEEPER *AND* DEEPER.\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
        
        m.Clear();
        m.Add("\"(Get it?)\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
    }
    
    private IEnumerator OxygenTutorial()
    {
        List<string> m = new List<string>();
        m.Add("\"You're running low on oxygen!\"");
        m.Add("\"Quick, drill through the RIGHT WALL!\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
    }
    
    private IEnumerator HeatTutorial()
    {
        List<string> m = new List<string>();
        m.Add("\"You're about to overheat!\"");
        m.Add("\"Quick, drill through the LEFT WALL!\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
    }
    
    private IEnumerator Tutorial03Death()
    {
        yield return new WaitForSeconds(2f);
        
        List<string> m = new List<string>();
        m.Add("\"I can't believe that WORKED!\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        
        yield return new WaitForSeconds(2f);
        
        m.Clear();
        m.Add("\"Well, it *WAS* working. Until you totaled it.\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
        
        m.Clear();
        m.Add("\"Head on back. We'll fix her up and you can try again!\"");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
        
        m.Clear();
        m.Add("[Tutorial complete.]");
        m.Add("[You can now play the game in Arcade mode.]");
        m.Add("[Try to get a new high score!]");
        StartCoroutine(MessageBox.ShowMessages(m));
        yield return new WaitForSeconds(2f);
        
        // Finish Tutorial
        PlayerPrefs.SetInt("Tutorial03", 1);

        SceneManager.LoadScene("MainMenu");
    }
}
