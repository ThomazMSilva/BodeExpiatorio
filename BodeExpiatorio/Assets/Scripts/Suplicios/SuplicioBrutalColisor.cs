using UnityEngine;

public class SuplicioBrutalColisor : MonoBehaviour
{
    private bool isPlayerInside;
    private Jogador player;
    [SerializeField] int groundLayer = 3;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            player = other.gameObject.GetComponent<Jogador>();
        }

        if (other.gameObject.layer == groundLayer && isPlayerInside)
            player.InstaKill(this.name, false);
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")) isPlayerInside = false;
    }
}
