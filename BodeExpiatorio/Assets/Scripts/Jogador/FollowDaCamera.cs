using UnityEngine;

public class FollowDaCamera : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 kneltPositon;
    [SerializeField, Range(-10, 1)] private float kneltY = -5f;
    private MovimentoJogador player;

    private void Start()
    {
        originalPosition = transform.localPosition;
        kneltPositon = originalPosition;
        kneltPositon.y += kneltY;

        player = FindAnyObjectByType<MovimentoJogador>();
        player.OnPlayerKnelt += Player_OnPlayerKnelt;
    }

    private void Player_OnPlayerKnelt(bool boolean) => transform.localPosition = boolean ? kneltPositon : originalPosition;
}
