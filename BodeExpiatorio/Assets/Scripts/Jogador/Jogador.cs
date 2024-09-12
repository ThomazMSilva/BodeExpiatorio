using UnityEngine;

[RequireComponent(typeof(MovimentoJogador),typeof(VidaJogador))]
public class Jogador : MonoBehaviour
{
    private MovimentoJogador movimento;
    private VidaJogador vida;

    public VidaJogador Vida { get { return vida; } private set { vida = value; } }
    public MovimentoJogador Movimento { get { return movimento; } private set { movimento = value; } }

    private void Awake()
    {
        movimento = GetComponent<MovimentoJogador>();
        vida = GetComponent<VidaJogador>();
    }
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
