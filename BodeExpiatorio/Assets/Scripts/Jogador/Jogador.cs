using UnityEngine;

[RequireComponent(typeof(MovimentoJogador),typeof(VidaJogador))]
public class Jogador : MonoBehaviour
{
    [SerializeField] MovimentoJogador movimento;
    [SerializeField] VidaJogador vida;

    public void ApplyDamageEffect(float damageAmount)
    {
        vida.DamageHealth(damageAmount);
    }

    public void ApplyDamageEffect(float damageAmount, Vector3 force, ForceMode forceMode = ForceMode.Impulse)
    {
        vida.DamageHealth(damageAmount);
        movimento.ApplyForce(force, forceMode);
        movimento.Ragdoll();
    }    

}
