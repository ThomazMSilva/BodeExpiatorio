using UnityEngine;

public class Corda : MonoBehaviour
{
    private JogadorReference player;
    private bool jumpButtonDown;

    private void OnEnable()
    {
        Entrada.Instance.OnJumpButtonDown += JumpPressed;
    }

    private void JumpPressed() => jumpButtonDown = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent<JogadorReference>(out player);
            player.SetPlayerClimbing(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //que condicional bizarra
        if (other.CompareTag("Player") && !player.Movimento.IsClimbing && (jumpButtonDown || Entrada.Instance.VerticalInput != 0))
        {
            player.SetPlayerClimbing(true);
            jumpButtonDown = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.SetPlayerClimbing(false);
        }
    }
}
