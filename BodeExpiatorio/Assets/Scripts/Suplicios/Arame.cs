using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Suplicios
{
    public class Arame : MonoBehaviour
    {
        [SerializeField] private bool willLookRight;
        private Jogador _player;
        [SerializeField] private float damage = 4f;
        [SerializeField] private float forceAmountX = 10f;
        [SerializeField] private float forceAmountY = 1.5f;
        [SerializeField] private float stunTime = .5f;

        [SerializeField] private Material damageMaterial;
        private Renderer[] rend;
        private Material[] originalMaterial;
        private Coroutine changeColorCoroutine;

        private void Start()
        {
            rend = GetComponentsInChildren<Renderer>();
            originalMaterial = new Material[rend.Length];
            for (int i = 0; i < rend.Length; i++)
            {
                originalMaterial[i] = rend[i].material;
            }
        }

        private IEnumerator ChangeColor()
        {
            foreach (var renderer in rend)
            {
                renderer.material = damageMaterial;
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime * 3);

            for (int i = 0; i < rend.Length; i++)
            {
                rend[i].material = originalMaterial[i];
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<Jogador>(out _player)) return;
            
            _player.SetPlayerWired(true, willLookRight);
            _player.SetWiredForceAndRagdoll(forceAmountX, forceAmountY, stunTime);

            if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
            changeColorCoroutine = StartCoroutine(ChangeColor());
            
            _player.ApplyDamageEffect(damage, this.name);

            AudioManager.Instance.PlayerOneShot(FMODEvents.Instance.ThornCollided, transform.position);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            _player.SetPlayerWired(false, willLookRight);

            if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
            changeColorCoroutine = StartCoroutine(ChangeColor());

            _player.ApplyDamageEffect(damage, this.name);
        }
    }
}