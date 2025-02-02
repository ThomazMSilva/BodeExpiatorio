﻿using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Suplicios
{
    public class SuplicioTraicoeiro : MonoBehaviour
    {
        private bool isLookingRight;
        private bool isLookingUp;
        [SerializeField] private bool isLookingVertical;
        [SerializeField] private float verticalShooterHorizontalRange = 10;
        private bool isPlayerLookingRight;
        private Vector3 playerDirection = Vector3.zero;
        private Vector3 transformDirection = Vector3.zero;

        private MovimentoJogador _player;
        private Transform playerTransform;
        
        [Tooltip("Só funciona mudar fora do Play")]
        [SerializeField] private float playerCheckRate = .1f;
        private WaitForSeconds playerCheckInterval;

        [Tooltip("Só funciona mudar fora do Play")]
        [SerializeField] private float shootingRate = 1.5f;
        private WaitForSeconds shootingInterval;
        [SerializeField] private float sporeSpeed = 10f;
        [SerializeField] private Transform sporeDispenser;
        [SerializeField] private GameObject sporePrefab;

        private void Awake()
        {
            _player = FindAnyObjectByType<MovimentoJogador>();
            playerTransform = _player.transform;
            playerDirection = new(1, 0, 0);
            transformDirection = transform.right;
            isLookingRight = transformDirection.x > 0;
            isLookingVertical = transformDirection.y != 0;
            playerCheckInterval = new WaitForSeconds(playerCheckRate);
            shootingInterval = new WaitForSeconds(shootingRate);
        }

        private void OnEnable()
        {
            _player.OnPlayerTurned += ChangePlayerDirection;
            StartCoroutine(CheckAndShoot());
        }

        private void OnDisable()
        {
            _player.OnPlayerTurned -= ChangePlayerDirection;
            StopCoroutine(CheckAndShoot());
        }

        private void ChangePlayerDirection(bool boolean)
        {
            isPlayerLookingRight = boolean;
            playerDirection.x = isPlayerLookingRight ? 1 : -1;
        }

        private bool CanShoot()
        {
            bool isPlayerOnFront = !isLookingVertical
                                    ? isLookingRight
                                        ? playerTransform.position.x > transform.position.x
                                        : playerTransform.position.x < transform.position.x
                                    : transformDirection.y > 0
                                        ? playerTransform.position.y > transform.position.y
                                        : playerTransform.position.y < transform.position.y;


            bool isPlayerFacingAway = !isLookingVertical
                                        ? isLookingRight
                                            ? playerDirection.x > 0
                                            : playerDirection.x < 0
                                        : playerTransform.position.x > transform.position.x - verticalShooterHorizontalRange
                                          && playerTransform.position.x < transform.position.x + verticalShooterHorizontalRange;
            
            return isPlayerOnFront && isPlayerFacingAway;
        }

        private IEnumerator CheckAndShoot()
        {
            while (true)
            {
                if (CanShoot())
                {
                    GameObject go = PoolManager.Instance.InstantiateFromPool(sporePrefab, sporeDispenser.position, transform.rotation);
                    Rigidbody sporeRB = go.GetComponent<Rigidbody>();
                    sporeRB.velocity = Vector3.zero;
                    sporeRB.AddForce(transformDirection * sporeSpeed, ForceMode.Impulse);

                    AudioManager.Instance.PlayerOneShot(FMODEvents.Instance.TreacheryShot, sporeDispenser.position);

                    yield return shootingInterval;
                }
                yield return playerCheckInterval;
            }
        }
    }
}