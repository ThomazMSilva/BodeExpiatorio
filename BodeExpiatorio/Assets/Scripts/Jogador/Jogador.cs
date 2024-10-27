using UnityEngine;

[RequireComponent(typeof(MovimentoJogador),typeof(VidaJogador))]
public class Jogador : MonoBehaviour
{
    [SerializeField] private MovimentoJogador movement;
    [SerializeField] private VidaJogador life;
    [SerializeField] private ContagemRegressivaVidaJogador countdown;

    [Header("Buffs")]

    public bool ignoreLethalDamage = false;
    public bool ignoreFirstDamage = false;
    public bool creepingDamage = false;
    public bool reducedDamage = false;
    public bool doubleFervor = false;
    public bool countdownPause = false;

    public VidaJogador Vida { get => life;  private set => life = value; }
    public MovimentoJogador Movimento { get => movement; private set => movement = value; }

    public void ApplyDamageEffect(float damageAmount, object sender) => life.DamageHealth(damageAmount, sender);

    public void InstaKill(object sender, bool trueDamage = true) => life.DamageHealth(sender, trueDamage);

    public void ApplyDamageEffect(float damageAmount, Vector3 force, float stunSeconds, object sender, ForceMode forceMode = ForceMode.Impulse)
    {
        life.DamageHealth(damageAmount, sender);
        movement.ApplyForce(force, forceMode);
        movement.Ragdoll(stunSeconds);
        //Debug.Log($"Dano: {damageAmount} Forca: {force} Stun: {stunSeconds}sec");
    }

    public void ApplyCure() => life.CureHealth(life.BaseHealth);

    public void ApplyCure(float cureAmount) => life.CureHealth(cureAmount);

    public void SetPosition(Vector3 position) => movement.EnterWarp(position);

    public void StopCreepingDamage() => life.StopCreepingDamage();

    public void SetBuffs()
    {
        life.isIgnoreLethalDamageActive = ignoreLethalDamage;
        life.isIgnoreFirstDamageActive = ignoreFirstDamage;
        life.isCreepingDamageActive = creepingDamage;
        life.isReducedDamageActive = reducedDamage;
        countdown.isDoubleFervorActive = doubleFervor;
        countdown.isCountDownPauseActive = countdownPause;
    }

    public void ActivateBuff(BuffType buff)
    {
        ignoreLethalDamage = false;
        ignoreFirstDamage = false;
        creepingDamage = false;
        reducedDamage = false;
        doubleFervor = false;
        countdownPause = false;

        if(buff != BuffType.CreepingDamage && Vida.isCreepingDamageActive) 
        {
            Vida.isCreepingDamageActive = false;
            Vida.StopCreepingDamage();
            Vida.DamageHealth(Vida.CurrentHealth - Vida.CurrentFrontHealth, this.name); 
        }


        switch (buff)
        {
            case BuffType.IgnoreLethalDamage:
                ignoreLethalDamage = true;
                break;
            case BuffType.IgnoreFirstDamage:
                ignoreFirstDamage = true;
                break;
            case BuffType.CreepingDamage:
                creepingDamage = true;
                break;
            case BuffType.ReducedDamage:
                reducedDamage = true;
                break;
            case BuffType.DoubleFervor:
                doubleFervor = true;
                break;
            case BuffType.CountDownPause:
                countdownPause = true;
                break;
            default: 
                Debug.Log("Buff invalido: " + buff); 
                break;
        }

        SetBuffs();
    }

    public void SetPlayerWired(bool wiredState, bool shouldLookRight) => movement.SetWiredState(wiredState, shouldLookRight);

    public void SetPlayerClimbing(bool climbing) => Movimento.SetPlayerClimbing(climbing);
    public void SetWiredForceAndRagdoll(float wiredForceX, float wiredForceY, float ragdollTime) => movement.SetWiredForce(wiredForceX, wiredForceY, ragdollTime);
}

[System.Serializable]
public enum BuffType
{
    IgnoreLethalDamage,
    IgnoreFirstDamage,
    CreepingDamage,
    ReducedDamage,
    DoubleFervor,
    CountDownPause
}
