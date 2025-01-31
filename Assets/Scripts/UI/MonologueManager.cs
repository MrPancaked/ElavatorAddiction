using System.Collections;
using UnityEngine;
using TMPro;

public class MonologueManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject monologueWindow;
    [SerializeField] private TextMeshProUGUI monologueText;
    private bool firstInteraction = true;
    private bool isRunning;
    public Animator dialogueAnimator;
    public Collider firstInteractionCollider;

    [SerializeField] private string[] randomPhrases;
    private int randomPhraseIndex;

    private static MonologueManager instance;
    public static MonologueManager Instance { get { return instance; } }

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
        StartCoroutine(SpawnText());
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isRunning) return;
        if (firstInteraction)
        {
            StartCoroutine(ElevatorDialogue());
        }
        else
        {
            StartCoroutine(RepetetivePhrase());
        }
    }

    #endregion

    #region Text Updating

    public void OpenWindow()
    {
        ClearText();
        dialogueAnimator.SetTrigger("Open");
    }

    public void CloseWindow()
    {
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
            monologueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator ClearLine(float textSpeed)
    {
        while (monologueText.text.Length > 0)
        {
            monologueText.text = monologueText.text.Substring(0, monologueText.text.Length - 1);
            yield return new WaitForSeconds(textSpeed);
        }
    }

    #endregion

    #region Elevator

    IEnumerator SpawnText()
    {
        OpenWindow();
        isRunning = true;
        yield return StartCoroutine(TypeLine("Where am I?", 0.05f));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(ClearLine(0.03f));
        isRunning = false;
        CloseWindow();
    }

    IEnumerator ElevatorDialogue()
    {
        OpenWindow();
        firstInteractionCollider.enabled = false;
        isRunning = true;

        //yield return StartCoroutine(TypeLine("///Why aren't you waking up?!///", 0.02f));
        //yield return new WaitForSeconds(2f);
        //ClearText();
        //yield return StartCoroutine(TypeLine("///Please… don’t leave me!///"));
        //yield return new WaitForSeconds(1f);
        //ClearText();
        //yield return StartCoroutine(TypeLine("///He’s slipping away…///"));
        //yield return new WaitForSeconds(0.5f);
        //ClearText();
        yield return StartCoroutine(TypeLine("No, please don't go!", 0.02f));
        yield return new WaitForSeconds(1.2f);
        yield return StartCoroutine(ClearLine(0.005f));
        //yield return StartCoroutine(TypeLine("///Somebody do something!///", 0.02f));
        //yield return new WaitForSeconds(0.4f);
        //ClearText();
        //yield return StartCoroutine(TypeLine("///I can’t lose him…///"));
        //yield return new WaitForSeconds(0.3f);
        //ClearText();
        yield return StartCoroutine(TypeLine("Wake up… please…", 0.01f));
        yield return new WaitForSeconds(1.2f);
        yield return StartCoroutine(ClearLine(0.005f));
        //yield return StartCoroutine(TypeLine("///Stay with me! Don’t give up!///"));
        //yield return new WaitForSeconds(0.2f);
        //ClearText();
        yield return StartCoroutine(TypeLine("Stay a little longer..", 0.01f));
        yield return new WaitForSeconds(1.2f);
        yield return StartCoroutine(ClearLine(0.005f));
        //yield return StartCoroutine(TypeLine("///Don’t you dare give up!///"));
        yield return new WaitForSeconds(0.5f);
        //ClearText();
        yield return StartCoroutine(TypeLine("Why is she calling for me?", 0.08f));
        yield return new WaitForSeconds(1.2f);
        yield return StartCoroutine(ClearLine(0.005f));

        yield return StartCoroutine(TypeLine("Am I...", 0.06f));
        yield return new WaitForSeconds(0.7f);
        yield return StartCoroutine(ClearLine(0.005f));

        yield return StartCoroutine(TypeLine("In a coma?", 0.15f));
        yield return new WaitForSeconds(0.4f);
        yield return StartCoroutine(ClearLine(0.005f));

        yield return StartCoroutine(TypeLine("...", 0.5f));
        yield return new WaitForSeconds(0.7f);
        yield return StartCoroutine(ClearLine(0.005f));

        firstInteraction = false;
        isRunning = false;
        CloseWindow();
    }

    IEnumerator RepetetivePhrase()
    {
        OpenWindow();
        isRunning = true;

        randomPhraseIndex = Random.Range(0, randomPhrases.Length);

        yield return StartCoroutine(TypeLine(randomPhrases[randomPhraseIndex], 0.02f));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(ClearLine(0.005f));

        isRunning = false;
        CloseWindow();
    }

    #endregion

    #region Book

    public void UseBook()
    {
        if (isRunning) return;
        StartCoroutine(ReadBook());
    }

    IEnumerator ReadBook()
    {
        OpenWindow();
        isRunning = true;

        yield return StartCoroutine(TypeLine("<02.02.1965>", 0.07f));
        yield return new WaitForSeconds(1.2f);
        yield return StartCoroutine(ClearLine(0.01f));
        yield return StartCoroutine(TypeLine("The day I lost my parents", 0.07f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ClearLine(0.01f));

        isRunning = false;
        CloseWindow();
    }

    #endregion
}