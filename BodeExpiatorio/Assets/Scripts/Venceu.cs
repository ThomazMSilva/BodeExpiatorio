using UnityEngine;

public class Venceu : MonoBehaviour
{
    [SerializeField] private GameObject telaVitoria;
    [SerializeField] private Confessionario confessionario;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            telaVitoria.SetActive(true);
            Debug.Log(confessionario.name);
            confessionario.DisplayTime();
        }
    }
}
