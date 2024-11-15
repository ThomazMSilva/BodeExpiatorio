using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public List<DialogueBehaviour> dialogueOrderList = new List<DialogueBehaviour>();
    private int currentIndex = 0;

    public void StartDialogueOrder()
    {
        if (dialogueOrderList.Count > 0)
        {
            dialogueOrderList[currentIndex].gameObject.SetActive(true);
        }
    }

    public void NextDialogue()
    {
        if (currentIndex < dialogueOrderList.Count - 1)
        {
            dialogueOrderList[currentIndex].gameObject.SetActive(false);
            currentIndex++;
            dialogueOrderList[currentIndex].gameObject.SetActive(true);
        }
    }
}
