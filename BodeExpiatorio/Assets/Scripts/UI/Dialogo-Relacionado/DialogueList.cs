using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI.Dialogo_Relacionado
{
    public class DialogueList : MonoBehaviour
    {
        [SerializeField] private GameObject act1to2;
        [SerializeField] private GameObject act2to3;
        [SerializeField] private GameObject act3to4;
        
        public void ActivateCurrentDialogue()
        {
            int i = GameManager.Instance.currentRoom.sceneIndex;

            switch (i)
            {
                case 1:
                    act1to2.SetActive(true);
                    break;

                case 2: 
                case 3:
                    act2to3.SetActive(true);
                    break;

                case 5:
                    act3to4.SetActive(true);
                    break;
                default:
                    Debug.LogError("Entered confession from a scene different from the last level of an act.");
                    break;
            }
        }
    }
}