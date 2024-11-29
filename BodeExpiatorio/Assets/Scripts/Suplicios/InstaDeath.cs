using UnityEngine;

public class InstaDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.TryGetComponent<JogadorReference>(out JogadorReference player))
            player.InstaKill("O Abismo");
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.TryGetComponent<JogadorReference>(out JogadorReference player))
            player.InstaKill("O Abismo");
    }
}
