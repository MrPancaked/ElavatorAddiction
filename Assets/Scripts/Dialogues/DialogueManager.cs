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

    [HideInInspector]
    public bool isOpen;
    private int randomPhraseIndex;
    private bool skipLine;
    private bool lastLine;
    private bool isRunning;
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
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Start()
    {
        StartCoroutine(IntroMonologue());
    }

    void Update()
    {
        if (Inputs.Instance.interaction.WasPressedThisFrame() && isOpen)
        {
            if (lastLine && !isRunning)
            {
                CloseWindow();
            }
            else
            {
                skipLine = true;
            }
        }
    }

    #endregion

    #region Windows Updating

    public void OpenWindow()
    {
        ClearText();
        UISounds.Instance.WindowOpenSound();
        isOpen = true;
        dialogueAnimator.SetTrigger("Open");
    }

    public void CloseWindow()
    {
        lastLine = false;
        isOpen = false;
        UISounds.Instance.WindowCloseSound();
        dialogueAnimator.SetTrigger("Close");
    }

    public void ClearText()
    {
        monologueText.text = "";
    }

    #endregion

    #region Text Updating

    IEnumerator HandleLine(string line, float textSpeed)
    {
        isRunning = true;
        yield return StartCoroutine(TypeLine(line, textSpeed));
        isRunning = false;

        yield return StartCoroutine(WaitForInput());

        if (!lastLine)
        {
            yield return StartCoroutine(ClearLine(0.005f));
        }
    }

    IEnumerator TypeLine(string line, float textSpeed)
    {
        monologueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            if (skipLine)
            {
                monologueText.text = line;
                skipLine = false;
                break;
            }
            monologueText.text += c;
            UISounds.Instance.DialogueSound();
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator WaitForInput()
    {
        skipLine = false;

        while (!skipLine)
        {
            yield return null;
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
            UISounds.Instance.DialogueSound();
            monologueText.text = monologueText.text.Substring(0, monologueText.text.Length - 1);
            yield return new WaitForSeconds(textSpeed);
        }
    }

    #endregion

    #region Monologues

    IEnumerator IntroMonologue()
    {
        if (isOpen)
        {
            yield break;
        }

        OpenWindow();

        List<(string line, float textSpeed)> dialogue = new List<(string line, float textSpeed)>
        {
            ("Where am I?", 0.04f),
            ("...", 0.2f)
        };

        for (int i = 0; i < dialogue.Count; i++)
        {
            lastLine = (i == dialogue.Count - 1);
            yield return StartCoroutine(HandleLine(dialogue[i].line, dialogue[i].textSpeed));
        }
    }

    // Modified ElevatorMonologue to take the dialogue list as an argument
    public IEnumerator ElevatorMonologue(string[] dialogues)
    {
        if (isOpen)
        {
            yield break;
        }

        OpenWindow();

        randomPhraseIndex = Random.Range(0, dialogues.Length);
        lastLine = true;
        yield return StartCoroutine(HandleLine(dialogues[randomPhraseIndex], 0.04f));
    }

    public IEnumerator ReadBook()
    {
        if (isOpen)
        {
            yield break;
        }

        OpenWindow();

        List<(string line, float textSpeed)> dialogue = new List<(string line, float textSpeed)>
        {
            ("02.02.1965", 0.05f),
            ("The day I lost my parents", 0.03f)
        };

        for (int i = 0; i < dialogue.Count; i++)
        {
            lastLine = (i == dialogue.Count - 1);
            yield return StartCoroutine(HandleLine(dialogue[i].line, dialogue[i].textSpeed));
        }
    }

    #endregion
}