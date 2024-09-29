using UnityEngine;

public class Succ : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 1;
    [SerializeField] private float force = 45;
    [SerializeField, Range(0,1)] private float kneelingForceMultiplier = 0.5f;
    private bool _isKneeling;
    private float currentForceMultiplier;
    private Jogador _player;

    private void OnEnable()
    {
        Entrada.Instance.OnKneelButtonDown += SetKneelingTrue;
        Entrada.Instance.OnKneelButtonUp += SetKneelingFalse;
    }
   
    private void OnDisable()
    {
        Entrada.Instance.OnKneelButtonDown -= SetKneelingTrue;
        Entrada.Instance.OnKneelButtonUp -= SetKneelingFalse;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent<Jogador>(out _player)) return;

        currentForceMultiplier = _isKneeling ? force * kneelingForceMultiplier : force;
        _player.ApplyDamageEffect
        (
            damagePerSecond * Time.fixedDeltaTime, 
            -transform.right * currentForceMultiplier, 
            .01f, 
            ForceMode.Acceleration
        );
    }

    private void SetKneelingTrue() => _isKneeling = true;
    private void SetKneelingFalse() => _isKneeling = false;
}
