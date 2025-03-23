using UnityEngine;

public class Book : MonoBehaviour
{
    public BookScriptableObject bookData; // Assign this in the Unity Inspector

    public void ReadBook()
    {
        if (bookData != null)
        {
            DialogueManager.Instance.StartCoroutine(DialogueManager.Instance.ReadBook(bookData));
        }
        else
        {
            Debug.LogWarning("Book data is missing on " + gameObject.name);
        }
    }
}