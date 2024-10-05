using UnityEngine;

public class PlataformaBrutal : MonoBehaviour
{
    [SerializeField] private float damage;
    private Jogador _player;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<Jogador>(out _player)) return;

        _player.ApplyDamageEffect(damage, this.name);
    }
}
