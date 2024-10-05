using UnityEngine;

namespace Assets.Scripts.Suplicios
{
    public class Arame : MonoBehaviour
    {
        [SerializeField] private bool willLookRight;
        private Jogador _player;
        [SerializeField] private float damage = 4f;
        [SerializeField] private float forceAmountX = 10f;
        [SerializeField] private float forceAmountY = 1.5f;
        [SerializeField] private float stunTime = .5f;


        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<Jogador>(out _player)) return;
            
            _player.SetPlayerWired(true, willLookRight);
            _player.SetWiredForceAndRagdoll(forceAmountX, forceAmountY, stunTime);
            
            _player.ApplyDamageEffect(damage, this.name);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            _player.SetPlayerWired(false, willLookRight);
            
            _player.ApplyDamageEffect(damage, this.name);
        }
    }
}