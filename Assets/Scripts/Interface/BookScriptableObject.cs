using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBook", menuName = "Dialogue/Book")]
public class BookScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class BookLine
    {
        [TextArea(2, 5)]
        public string text;
        public float textSpeed = 0.04f; // Default text speed
    }

    public List<BookLine> bookLines;
}