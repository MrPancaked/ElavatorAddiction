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
    [HideInInspector]
    public bool isRunning;
    private bool skipLine;
    private bool lastLine;

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

    //void FixedUpdate()
    //{
    //    if (skipLine)
    //    {
    //        skipLine = false;
    //    }
    //}
    
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
        skipLine = false; // Reset to ensure proper typing
        isRunning = true;
        
        yield return StartCoroutine(TypeLine(line, textSpeed)); // Type out the line
        isRunning = false;
        
        yield return StartCoroutine(WaitForInput()); // Wait for player input before continuing
    }


    IEnumerator TypeLine(string line, float textSpeed)
    {
        monologueText.text = ""; // Ensure it's fully cleared before typing
        skipLine = false; // Reset this to prevent any immediate skips

        foreach (char c in line.ToCharArray())
        {
            if (skipLine)
            {
                monologueText.text = line; // Instantly display full text if skipping
                skipLine = false;
                break;
            }

            monologueText.text += c; // Append each letter
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

    public IEnumerator ElevatorMonologue(string[] dialogues)
    {
        if (isOpen)
        {
            yield break;
        }

        OpenWindow();

        int randomPhraseIndex = Random.Range(0, dialogues.Length);
        lastLine = true;
        yield return StartCoroutine(HandleLine(dialogues[randomPhraseIndex], 0.04f));
    }

    public IEnumerator ReadBook(BookScriptableObject book)
    {
        if (isOpen || book == null)
        {
            yield break;
        }

        OpenWindow();

        for (int i = 0; i < book.bookLines.Count; i++)
        {
            lastLine = (i == book.bookLines.Count - 1);
            yield return StartCoroutine(HandleLine(book.bookLines[i].text, book.bookLines[i].textSpeed));
        }
    }

    #endregion
}
