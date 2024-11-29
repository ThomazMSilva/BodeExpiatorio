using UnityEngine;

public class SuplicioBrutalColisor : MonoBehaviour
{
    private bool isPlayerInside;
    private JogadorReference player;
    [SerializeField] private int groundLayer = 3;
    [SerializeField] private bool trueDamageActive = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            player = other.gameObject.GetComponent<JogadorReference>();
        }

        if (other.gameObject.layer == groundLayer && isPlayerInside)
            player.InstaKill("Estrela-da-Manhã", trueDamageActive);
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")) isPlayerInside = false;
    }
}
