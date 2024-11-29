using UnityEngine;

public class PlataformaBrutal : MonoBehaviour
{
    [SerializeField] private float damage;
    private JogadorReference _player;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<JogadorReference>(out _player)) return;

        _player.ApplyDamageEffect(damage, "Brasas", 10);
    }
}
