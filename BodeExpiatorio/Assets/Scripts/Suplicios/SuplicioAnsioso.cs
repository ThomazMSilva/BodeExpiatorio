using System.Collections;
using UnityEngine;

public class SuplicioAnsioso : MonoBehaviour
{
    [SerializeField] private float interval = 2.5f;
    [SerializeField] private float damagePerSecond = 1;
    [SerializeField] private float force = 45;
    [SerializeField, Range(0, 1)] private float kneelingForceMultiplier = 0.5f;

    [SerializeField] private bool isIntervalActive = true;
    [SerializeField] private bool isAttracting;
    private bool _isKneeling;
    private float currentForceMultiplier;
    private Jogador _player;

    private void OnEnable()
    {
        Entrada.Instance.OnKneelButtonDown += SetKneelingTrue;
        Entrada.Instance.OnKneelButtonUp += SetKneelingFalse;
        StartCoroutine(Attract());
    }
   
    private void OnDisable()
    {
        Entrada.Instance.OnKneelButtonDown -= SetKneelingTrue;
        Entrada.Instance.OnKneelButtonUp -= SetKneelingFalse;
        StopCoroutine(Attract());
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isAttracting || !other.TryGetComponent<Jogador>(out _player)) return;

        currentForceMultiplier = _isKneeling ? force * kneelingForceMultiplier : force;
        _player.ApplyDamageEffect
        (
            damagePerSecond * Time.fixedDeltaTime, 
            -transform.right * currentForceMultiplier, 
            .01f,
            this.name,
            ForceMode.Acceleration
        );
    }

    private IEnumerator Attract()
    {
        while (true)
        {
            if (!isIntervalActive) yield return null;
            isAttracting = !isAttracting;
            yield return new WaitForSeconds(interval);
        }
    }

    private void SetKneelingTrue() => _isKneeling = true;
    private void SetKneelingFalse() => _isKneeling = false;
}
