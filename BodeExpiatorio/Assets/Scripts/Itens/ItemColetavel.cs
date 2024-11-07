using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Itens
{
    public class ItemColetavel : MonoBehaviour
    {

        Jogador _player;
        Vector3 originalPosition;
        Tween hoverTween;
        public UnityEvent OnCollected;

        private void Start()
        {
            _player = FindAnyObjectByType<Jogador>();
            _player.Vida.OnPlayerDeath += () => gameObject.SetActive(true);
            originalPosition = transform.position;
            Hover();
        }

        [SerializeField] private float cureAmount;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            OnCollected?.Invoke();

            gameObject.SetActive(false);
        }

        private void Hover()
        {
            hoverTween = transform.DOMoveY(originalPosition.y + .1f, 1)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDestroy() => hoverTween?.Kill();
    }

}