using UnityEngine;

namespace Assets.Scripts.Itens
{
    public class ItemCura : MonoBehaviour
    {
        [SerializeField] private float cureAmount;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<Jogador>(out Jogador _player)) return;

            _player.ApplyCure(cureAmount);

            gameObject.SetActive(false);
        }
    }
}