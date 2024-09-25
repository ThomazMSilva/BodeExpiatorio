using UnityEngine;

public class Venceu : MonoBehaviour
{
    [SerializeField] private GameObject telaVitoria;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            telaVitoria.SetActive(true);
        }
    }
}
