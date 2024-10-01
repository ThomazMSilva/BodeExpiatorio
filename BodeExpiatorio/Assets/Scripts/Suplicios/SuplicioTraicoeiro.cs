using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Suplicios
{
    public class SuplicioTraicoeiro : MonoBehaviour
    {
        [SerializeField] private bool isShooting;
        private bool isPlayerLookingRight;
        private Vector3 playerDirection = Vector3.zero;
        private Vector3 transformDirection = Vector3.zero;

        [SerializeField] private float distanceCheckInterval = .1f;
        private const float minMovementToCheck = 0.0015f;
        private Vector3 lastPlayerPosition;
        private WaitForSeconds waitForDistanceCheck;
        
        private MovimentoJogador _player;
        private Transform playerTransform;

        private void Awake()
        {
            _player = FindAnyObjectByType<MovimentoJogador>();
            playerTransform = _player.transform;
            playerDirection = new(1, 0, 0);
            transformDirection = transform.right;
            waitForDistanceCheck = new WaitForSeconds(distanceCheckInterval);
        }

        private void OnEnable()
        {
            _player.OnPlayerTurned += ChangePlayerDirection;
            StartCoroutine(UpdateShootingOnMovement());
        }

        private void OnDisable()
        {
            _player.OnPlayerTurned -= ChangePlayerDirection;
            StopCoroutine(UpdateShootingOnMovement());
        }

        private void ChangePlayerDirection(bool boolean)
        {
            isPlayerLookingRight = boolean;
            playerDirection.x = isPlayerLookingRight ? 1 : -1;
        }

        private void ChangeShootingState()
        {
            isShooting =
                playerTransform.position.x - transform.position.x > 0
                ? Vector3.Dot(transformDirection, playerDirection) > 0
                : Vector3.Dot(transformDirection, -playerDirection) > 0;
        }

        private bool HasPlayerMoved()
        {
            float playerMovement = (playerTransform.position - lastPlayerPosition).sqrMagnitude;

            if (playerMovement > minMovementToCheck)
            {
                lastPlayerPosition = playerTransform.position;
                return true;
            }
            return false;
        }

        private IEnumerator UpdateShootingOnMovement()
        {
            while (true)
            {
                if (HasPlayerMoved()) ChangeShootingState();

                yield return waitForDistanceCheck;
            }
        }
    }
}