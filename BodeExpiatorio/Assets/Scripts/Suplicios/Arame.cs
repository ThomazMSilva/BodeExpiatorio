using UnityEngine;

namespace Assets.Scripts.Suplicios
{
    public class Arame : MonoBehaviour
    {
        [SerializeField] private bool willLookRight;
        private Jogador _player;
        private Vector3 sideForce;
        [SerializeField] private float damage = 2f;
        [SerializeField, Range(0,1)] private float yVector = .75f;
        [SerializeField] private float forceAmount = 10f;
        [SerializeField] private float stunTime = .5f;


        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<Jogador>(out _player)) return;

            _player.SetPlayerWired(true, willLookRight);
            _player.ApplyDamageEffect(damage);

            _player.Movimento.OnPlayerJumpInput += WallPush;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            if(_player != null) _player.Movimento.OnPlayerJumpInput -= WallPush;
        }

        private void WallPush()
        {
            _player.SetPlayerWired(false, willLookRight);

            sideForce.Set(willLookRight ? 1f : -1f, yVector, 0f);
            sideForce *= forceAmount;

            _player.ApplyDamageEffect(damage, sideForce, stunTime);
        }
    }
}