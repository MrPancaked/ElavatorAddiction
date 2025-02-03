using System.Collections;
using System.Collections.Generic; // Added for List
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
        if (Inputs.Instance.interaction.WasPressedThisFrame())
        {
            if (isRunning && !skipLine)
            {
                skipLine = true;
            }
        }
    }

    #endregion

    #region Text Updating

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

    IEnumerator TypeLine(string line, float textSpeed)
    {
        foreach (char c in line.ToCharArray())
        {
            if (skipLine)
            {
                monologueText.text = line;
                skipLine = false;
                yield break;
            }
            monologueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator ClearLine(float textSpeed)
    {
        while (monologueText.text.Length > 0)
        {
            if (skipLine)
            {
                monologueText.text = "";
                skipLine = false;
                yield break;
            }
            monologueText.text = monologueText.text.Substring(0, monologueText.text.Length - 1);
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator HandleLine(string line, float textSpeed, float delayAfterLine)
    {
        yield return StartCoroutine(TypeLine(line, textSpeed));
        if (skipLine)
        {
            skipLine = false;
            yield break;
        }
        yield return new WaitForSeconds(delayAfterLine);
        yield return StartCoroutine(ClearLine(0.005f));
    }

    #endregion

    #region Monologues

    IEnumerator IntroMonologue()
    {
        OpenWindow();

        List<(string line, float textSpeed, float delayAfterLine)> dialogue = new List<(string line, float textSpeed, float delayAfterLine)>
        {
            ("Where am I?", 0.05f, 1.5f),
            //("//No, please don't go!//", 0.02f, 0.9f),
            //("//Wake up… please…//", 0.01f, 0.9f),
            //("Why is she calling for me?", 0.06f, 0.9f),
            //("Am I...", 0.03f, 0.8f),
            //("In a coma?", 0.12f, 0.4f),
            ("...", 0.3f, 0.3f)
        };

        foreach (var (line, textSpeed, delayAfterLine) in dialogue)
        {
            yield return StartCoroutine(HandleLine(line, textSpeed, delayAfterLine));
            if (skipLine)
            {
                skipLine = false;
                yield break;
            }
        }

        CloseWindow();
    }

    public IEnumerator ElevatorMonologue()
    {
        OpenWindow();
        randomPhraseIndex = Random.Range(0, randomPhrases.Length);

        yield return StartCoroutine(HandleLine(randomPhrases[randomPhraseIndex], 0.02f, 2f));

        CloseWindow();
    }

    public IEnumerator ReadBook()
    {
        if (isRunning)
        {
            yield break; // Exit the coroutine if already running
        }

        OpenWindow();

        yield return StartCoroutine(HandleLine("<02.02.1965>", 0.05f, 0.8f));
        if (skipLine)
        {
            skipLine = false;
            yield break;
        }
        yield return StartCoroutine(HandleLine("The day I lost my parents", 0.03f, 0.6f));

        CloseWindow();
    }

    #endregion
}