using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum CharacterExpression { Happy, Neutral, Upset }

[System.Serializable]
public class Dialogue
{
    [TextArea] public string text;
    public float typingDelaySpeed = 0.1f;
    public bool hasBackground;
    public CharacterExpression characterMood;
}

public class DialogueBehaviour : MonoBehaviour
{
    public List<Dialogue> dialogueList = new();
    public UnityEvent OnTextFinished;
    private TextMeshProUGUI textMeshPro;
    private UnityEngine.UI.Image characterImage;

    private int currentIndex = 0;
    private bool isTextFinished = false;

    private void Awake()
    {
        CanvasReference reference = FindObjectOfType<CanvasReference>();
        textMeshPro = reference?.tmp;
        characterImage = reference?.image;
    }

    private void OnEnable()
    {
        currentIndex = 0;
        DisplayCurrentDialogue();
    }

    public void DisplayCurrentDialogue()
    {
        if (currentIndex < dialogueList.Count)
        {
            StartCoroutine(TypeText(dialogueList[currentIndex].text, dialogueList[currentIndex].typingDelaySpeed));
            // Atualize a imagem conforme a expressão
            // Exemplo: UpdateCharacterImage(dialogueList[currentIndex].characterMood);
        }
        else
        {
            OnTextFinished?.Invoke();
        }
    }

    private IEnumerator TypeText(string text, float typingInterval)
    {
        isTextFinished = false;
        textMeshPro.text = "";
        WaitForSeconds interval = new WaitForSeconds(typingInterval);

        foreach (char c in text)
        {
            textMeshPro.text += c;
            yield return interval;
        }

        isTextFinished = true;
    }

    public void NextDialogue()
    {
        if (isTextFinished)
        {
            currentIndex++;
            DisplayCurrentDialogue();
        }
    }
}
