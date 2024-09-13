using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Espinho : MonoBehaviour
{
    [Header("Configurações do Trampolim")]
    [SerializeField] private float launchForce = 10f;  
    [SerializeField] private int damageAmount = 1;
    [Tooltip("O tempo que o Jogador fica sem controlar o input horizontal quando colide. Essencial pra armadilhas horizontais.")]
    [SerializeField] private float timeStunned = .3f;
    
    [Tooltip("Se a velocidade de colisao vai afetar a forca de lancamento")]
    [SerializeField] private bool isVelocityRelevant;
    [SerializeField, Range(1f,3f)] private float minVelMultiplier = 1f;
    [SerializeField, Range(1f, 3f)] private float maxVelMultiplier = 3f;
    [SerializeField] private float terminalVelocity = 30f;


    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        
        if (!go.CompareTag("Player")) return;
        
        if (!go.TryGetComponent<Jogador>(out Jogador player)) return;

        Vector3 relativeVel = collision.relativeVelocity;
        float colVelToUp = Vector3.Dot(relativeVel, -transform.up); //Produto vetorial do quão de frente foi a colisão

        if (colVelToUp <= 0) return;

        float velPrctRelativeToTerminal = Mathf.Clamp01(colVelToUp / terminalVelocity);//Quao proxima a velocidade de colisao com a velocidade maxima do jogador
        float forceMultiplier = Mathf.Lerp(minVelMultiplier, maxVelMultiplier, velPrctRelativeToTerminal);
        
        float forceRelativeToUp = launchForce * Mathf.Clamp01(colVelToUp);
        float finalForce = !isVelocityRelevant ? forceRelativeToUp : forceRelativeToUp * forceMultiplier;
        float damage = !isVelocityRelevant ? damageAmount : damageAmount + (damageAmount * velPrctRelativeToTerminal);

        //Debug.Log($"velPrct: {velPrctRelativeToTerminal} forceMultiplier: {finalForce}");

        player.ApplyDamageEffect(damage, finalForce * transform.up, timeStunned);
    }

}
