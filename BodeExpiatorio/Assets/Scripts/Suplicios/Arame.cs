using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Suplicios
{
    public class Arame : MonoBehaviour
    {
        [SerializeField] private bool willLookRight;
        private Jogador player;
        private Vector3 sideForce;

        [SerializeField] private float damage = 2f;
        [SerializeField, Range(0,1)] private float yVector = .75f;
        [SerializeField] private float forceAmount = 10f;
        [SerializeField] private float stunTime = .5f;
        [SerializeField] private bool hasPlayer;


        private void OnTriggerEnter(Collider other)
        {
            GameObject go = other.gameObject;

            if (!go.CompareTag("Player")) return;

            hasPlayer = true;
            player = go.GetComponent<Jogador>();

            player.SetPlayerWired(true, willLookRight);
            player.ApplyDamageEffect(damage);

            player.Movimento.OnPlayerJumpInput += WallPush;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            hasPlayer = false;
            player.Movimento.OnPlayerJumpInput -= WallPush;
        }

        private void WallPush()
        {
            player.SetPlayerWired(false, willLookRight);

            sideForce.Set(willLookRight ? 1f : -1f, yVector, 0f);
            sideForce *= forceAmount;

            player.ApplyDamageEffect(damage, sideForce, stunTime);
        }
    }
}