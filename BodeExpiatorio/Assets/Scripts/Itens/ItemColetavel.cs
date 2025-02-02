﻿using DG.Tweening;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Itens
{
    public class ItemColetavel : MonoBehaviour
    {

        JogadorReference _player;
        Vector3 originalPosition;
        Tween hoverTween;
        private bool isCollectable = true;
        [SerializeField] private bool activeWhileInside;
        [SerializeField] private bool keepActiveOnCollection;
        [SerializeField] private bool recollectableOnPlayerDeath = true;
        [SerializeField] private bool recollectableOnTimer;
        [SerializeField] private float reactivationTimer = 20f;
        [field: SerializeField] private EventReference interactionAudioEventReference;
        public UnityEvent OnCollected;
        public UnityEvent OnLeftTrigger;


        private void Start()
        {
            if (recollectableOnPlayerDeath)
            {
                _player = FindAnyObjectByType<JogadorReference>();
                _player.Vida.OnPlayerDeath += Reactivate;
            }
            originalPosition = transform.position;
            Hover();
        }

        private void Reactivate()
        {
            isCollectable = true;
            gameObject.SetActive(true);
        }

        private void Collect()
        {
            OnCollected?.Invoke();

            if(!interactionAudioEventReference.IsNull) 
                AudioManager.Instance.PlayerOneShot(interactionAudioEventReference, transform.position);

            if (!keepActiveOnCollection)
                isCollectable = false;

            if(recollectableOnTimer)
                StartCoroutine(StartTimer());

            if(!keepActiveOnCollection)
                gameObject.SetActive(false);
        }

        private IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(reactivationTimer);
            Reactivate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            Collect();
        }
        private void OnTriggerExit(Collider other)
        {
            if (!activeWhileInside || !other.gameObject.CompareTag("Player")) return;

            OnLeftTrigger?.Invoke();
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