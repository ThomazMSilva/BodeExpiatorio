using UnityEngine;

public class MovimentoCamera : MonoBehaviour
{
    [SerializeField] private Transform jogadorTransform;
    [SerializeField] private float tempoSuavizacao = 0.3F;
    private Vector3 velocidade = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 alvo = new(jogadorTransform.position.x, jogadorTransform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, alvo, ref velocidade, tempoSuavizacao);
    }
}
