using UnityEngine;

[RequireComponent(typeof(MovimentoJogador),typeof(VidaJogador))]
public class Jogador : MonoBehaviour
{
    [SerializeField] private MovimentoJogador movimento;
    [SerializeField ]private VidaJogador vida;

    public VidaJogador Vida { get { return vida; } private set { vida = value; } }
    public MovimentoJogador Movimento { get { return movimento; } private set { movimento = value; } }

    public static Jogador Instance;

    private void Awake()
    {
        /*movimento = GetComponent<MovimentoJogador>();
        vida = GetComponent<VidaJogador>();*/
        Instance = this;
    }
    public void ApplyDamageEffect(float damageAmount) => vida.DamageHealth(damageAmount);

    public void ApplyDamageEffect(float damageAmount, Vector3 force, float stunSeconds, ForceMode forceMode = ForceMode.Impulse)
    {
        vida.DamageHealth(damageAmount);
        movimento.ApplyForce(force, forceMode);
        movimento.Ragdoll(stunSeconds);
        //Debug.Log($"Dano: {damageAmount} Forca: {force} Stun: {stunSeconds}sec");
    }

    public void SetPosition(Vector3 position)
    {
        movimento.EnterWarp(position);        
    }

    public void ApplyCure() => vida.CureHealth(vida.BaseHealth);

    public void ApplyCure(float cureAmount) => vida.CureHealth(cureAmount);

    public void SetPlayerWired(bool wiredState, bool shouldLookRight)
    {
        movimento.SetWiredState(wiredState, shouldLookRight);
    }
}
