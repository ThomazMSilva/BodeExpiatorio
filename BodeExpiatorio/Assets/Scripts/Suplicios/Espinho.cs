using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Espinho : MonoBehaviour
{
    [Header("Configurações do Trampolim")]
    [SerializeField] private float launchForce = 10f;  
    [SerializeField] private int damageAmount = 1;
    [Tooltip("O tempo que o Jogador fica sem controlar o input horizontal quando colide. Essencial pra armadilhas horizontais.")]
    [SerializeField] private float timeStunned = .3f;

    [Tooltip("Se a direcao do impacto da colisao vai afetar a forca de lancamento.")]
    [SerializeField] private bool isDirectionRelevant = false;
    [Tooltip("Se a velocidade de impacto da colisao vai afetar a forca de lancamento.")]
    [SerializeField] private bool isVelocityRelevant = false;
    [SerializeField, Range(1f,3f)] private float minVelMultiplier = 1f;
    [SerializeField, Range(1f, 10f)] private float maxVelMultiplier = 3f;
    [SerializeField] private float terminalVelocity = 30f;


    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<Jogador>(out Jogador _player)) return;

        Vector3 relativeVel = collision.relativeVelocity;
        float colDotToUp = Vector3.Dot(relativeVel, -transform.up); //Produto vetorial do quão de frente foi a colisão

        if (colDotToUp <= 0) return;

        float velPrctRelativeToTerminal = Mathf.Clamp01(colDotToUp / terminalVelocity);//Quao proxima a velocidade de colisao com a velocidade maxima do jogador
        float forceMultiplier = Mathf.Lerp(minVelMultiplier, maxVelMultiplier, velPrctRelativeToTerminal);

        float colDirMultiplier = Mathf.Clamp01(colDotToUp);
        if(!isDirectionRelevant) colDirMultiplier = colDirMultiplier > 0 ? 1 : 0; //So checa se a colisao veio de frente, ou nao, sem gradual

        float forceRelativeToUp = launchForce * colDirMultiplier;
        float finalForce = !isVelocityRelevant ? forceRelativeToUp : forceRelativeToUp * forceMultiplier;
        float damage = !isVelocityRelevant ? damageAmount : damageAmount + (damageAmount * velPrctRelativeToTerminal);

        //Debug.Log($"velPrct: {velPrctRelativeToTerminal} forceMultiplier: {finalForce}");

        _player.ApplyDamageEffect(damage, finalForce * transform.up, timeStunned);
    }

}
