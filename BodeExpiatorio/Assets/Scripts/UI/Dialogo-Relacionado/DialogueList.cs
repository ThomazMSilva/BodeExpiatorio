using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Dialogo_Relacionado
{
    public class DialogueList : MonoBehaviour
    {
        [SerializeField] private UINavigationManager navigationManager;
        [SerializeField] private GameObject act1to2;
        [SerializeField] private GameObject act2to3;
        [SerializeField] private GameObject act3to4;
        [SerializeField] private GameObject defaultDialogue;
        
        public void ActivateCurrentDialogue()
        {
            int i = GameManager.Instance.currentRoom.sceneIndex;

            switch (i)
            {
                case 1:
                    navigationManager.OpenPanel(act1to2);
                    /*act1to2.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(act1to2);*/
                    break;

                case 2: 
                case 3:
                    navigationManager.OpenPanel(act2to3);
                    //EventSystem.current.SetSelectedGameObject(act2to3);
                    break;

                case 5:
                    navigationManager.OpenPanel(act3to4);
                    /*act3to4.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(act3to4);*/
                    break;
                default:
                    Debug.LogError("Entered confession from a scene different from the last level of an act.");
                    navigationManager.OpenPanel(defaultDialogue);
                    break;
            }
        }
    }
}