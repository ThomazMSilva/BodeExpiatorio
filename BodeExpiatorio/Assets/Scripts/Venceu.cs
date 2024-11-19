using UnityEngine;

public class Venceu : MonoBehaviour
{
    //[SerializeField] private GameObject telaVitoria;
    //[SerializeField] private Confessionario confessionario;

    [SerializeField] private bool isLastFromAct;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(!isLastFromAct)
                GameManager.Instance.LoadNextRoom();
            else
                GameManager.Instance.LoadConfessionBooth();
        }
    }
}