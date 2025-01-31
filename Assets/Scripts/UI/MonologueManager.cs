using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class MonologueManager : MonoBehaviour
{
    [SerializeField] private GameObject monologueWindow;
    [SerializeField] private TextMeshProUGUI monologueText;
    private bool firstInteraction = true;
    private bool isRunning;

    [SerializeField] private string[] randomPhrases;
    private int randomPhraseIndex;
    
    private static MonologueManager instance;
    public static MonologueManager Instance { get { return instance; } }


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
        StartCoroutine(BeginingMonologue(3f));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ElevatorMonologue"))
        {
            if (firstInteraction)
            {
                StartCoroutine(ElevatorDialogue(4f));
            }
            else
            {
                StartCoroutine(RepetetivePhrase(2f));
            }
        }
    }

    public void UseBook()
    {
        StartCoroutine(ReadBook(3f));
    }

    IEnumerator BeginingMonologue(float seconds)
    {
        isRunning = true;
        PopUp();
        monologueText.text = "What's this? Where am I? This elevator looks familiar...";
        yield return new WaitForSeconds(seconds);
        WindowClear();
        isRunning = false;
    }

    IEnumerator ElevatorDialogue(float seconds)
    {
        PopUp();
        monologueText.text = "Female voice: Why isn’t he waking up?";
        yield return new WaitForSeconds(seconds);
        monologueText.text = "Male voice: His vitals are dropping... He’s slipping away...";
        yield return new WaitForSeconds(seconds);
        monologueText.text = "Wait... are they talking about me? Am I still in a coma? Is this all just a dream?";
        yield return new WaitForSeconds(seconds);
        WindowClear();
        firstInteraction = false;
    }

    IEnumerator RepetetivePhrase(float seconds)
    {
        randomPhraseIndex = Random.Range(0,4);
        PopUp();
        monologueText.text = randomPhrases[randomPhraseIndex];
        yield return new WaitForSeconds(seconds);
        WindowClear();
    }

    IEnumerator ReadBook(float seconds)
    {
        PopUp();
        monologueText.text = "Book: 02.02.1965 The ritual went succefully.";
        yield return new WaitForSeconds(seconds);
        monologueText.text = "I remember that day...";
        yield return new WaitForSeconds(seconds);
        monologueText.text = "the day my parents died.";
        yield return new WaitForSeconds(seconds);
        WindowClear();
    }

    public void PopUp()
    {
        monologueWindow.SetActive(true);
    }

    public void WindowClear()
    {
        monologueWindow.SetActive(false);
    }
}
