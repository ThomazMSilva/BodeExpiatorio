using UnityEngine;

namespace Assets.Scripts.Itens
{
    public class ItemCura : MonoBehaviour
    {
        Jogador _player;
        private void Start()
        {
            _player = FindAnyObjectByType<Jogador>();
            _player.Vida.OnPlayerDeath += () => gameObject.SetActive(true);
        }

        [SerializeField] private float cureAmount;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            _player.ApplyCure(cureAmount);

            gameObject.SetActive(false);
        }


    }
}