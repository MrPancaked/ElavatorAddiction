using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    #region Variables

    public Animator dialogueAnimator;
    public GameObject monologueWindow;
    public TextMeshProUGUI monologueText;
    public string[] randomPhrases;

    [HideInInspector]
    public bool isRunning;
    private int randomPhraseIndex;
    private bool skipLine; // Flag to indicate a skip request
    private static DialogueManager instance;
    public static DialogueManager Instance { get { return instance; } }

    #endregion

    #region Unity Methods

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
    }

    public void Start()
    {
        StartCoroutine(IntroMonologue());
    }

    void Update()
    {
        if (Inputs.Instance.interaction.WasPressedThisFrame() && isRunning)
        {
            skipLine = true;
        }
    }

    #endregion

    #region Windows Updating

    public void OpenWindow()
    {
        ClearText();
        isRunning = true;
        dialogueAnimator.SetTrigger("Open");
    }

    public void CloseWindow()
    {
        isRunning = false;
        dialogueAnimator.SetTrigger("Close");
    }

    public void ClearText()
    {
        monologueText.text = ""; // Clear the text for the next line.
    }

    #endregion

    #region Text Updating

    IEnumerator HandleLine(string line, float textSpeed)  //Removed the delayAfterLine parameter and adapted to be only one line.
    {
        yield return StartCoroutine(TypeLine(line, textSpeed));
        yield return StartCoroutine(WaitForInput()); // Wait for input to continue
        yield return StartCoroutine(ClearLine(0.005f));
    }

    IEnumerator TypeLine(string line, float textSpeed)
    {
        foreach (char c in line.ToCharArray())
        {
            if (skipLine)
            {
                monologueText.text = line;
                skipLine = false; // Reset skipLine
                yield break;
            }
            monologueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator WaitForInput() // New coroutine to wait for the 'E' key press
    {
        skipLine = false; //reset skipLine

        while (!skipLine)
        {
            yield return null; // Wait until skipLine is true
        }
    }

    IEnumerator ClearLine(float textSpeed)
    {
        while (monologueText.text.Length > 0)
        {
            if (skipLine)
            {
                monologueText.text = "";
                skipLine = false;  // Reset skipLine
                yield break;
            }
            monologueText.text = monologueText.text.Substring(0, monologueText.text.Length - 1);
            yield return new WaitForSeconds(textSpeed);
        }
    }

    #endregion

    #region Monologues

    IEnumerator IntroMonologue()
    {
        if (isRunning)
        {
            yield break;
        }

        OpenWindow();

        List<(string line, float textSpeed)> dialogue = new List<(string line, float textSpeed)>
        {
            ("Where am I?", 0.01f),
            //("//No, please don't go!//", 0.02f),
            //("//Wake up… please…//", 0.01f),
            //("Why is she calling for me?", 0.06f),
            //("Am I...", 0.03f),
            //("In a coma?", 0.12f),
            ("...", 0.2f)
        };

        foreach (var (line, textSpeed) in dialogue)
        {
            yield return StartCoroutine(HandleLine(line, textSpeed)); //Removed the delayAfterLine parameter
        }

        CloseWindow();
    }

    public IEnumerator ElevatorMonologue()
    {
        if (isRunning)
        {
            yield break;
        }

        OpenWindow();

        randomPhraseIndex = Random.Range(0, randomPhrases.Length);
        yield return StartCoroutine(HandleLine(randomPhrases[randomPhraseIndex], 0.01f));

        CloseWindow();
    }

    public IEnumerator ReadBook()
    {
        if (isRunning)
        {
            yield break;
        }

        OpenWindow();

        yield return StartCoroutine(HandleLine("02.02.1965", 0.05f));
        yield return StartCoroutine(HandleLine("The day I lost my parents", 0.03f));

        CloseWindow();
    }

    #endregion
}