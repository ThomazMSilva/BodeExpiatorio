using Cinemachine;
using System.Collections;
using TreeEditor;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public class CinemachineShake : MonoBehaviour
    {
        public static CinemachineShake instance;

        private CinemachineVirtualCamera cam;
        private CinemachineBasicMultiChannelPerlin perlin;

        VidaJogador vida;

        [SerializeField] private float minShakeTime = 0.45f;
        [SerializeField] private float maxShakeTime = 1f;
        [SerializeField] private float minShakeIntensity = 5;
        [SerializeField] private float maxShakeIntensity = 25;

        [Space(8f)]

        [Tooltip("Literalmente pausa o jogo por esse tanto de segundos quando leva dano.")]
        [SerializeField] bool canHitStop = true;
        [SerializeField, Range(0,0.09f)] private float minHitStopTime = 0.035f;
        [SerializeField, Range(0,0.09f)] private float maxHitStopTime = 0.07f;

        private float baseHealth;

        private bool isTimeFrozen;

        // Use this for initialization
        private void Awake()
        {
            instance = this;
            cam = GetComponent<CinemachineVirtualCamera>(); 
            perlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            vida = FindAnyObjectByType<VidaJogador>();
            baseHealth = vida.BaseHealth;
        }

        private void OnEnable() => vida.OnHealthChanged += ShakeOnDamage;
        private void OnDisable() => vida.OnHealthChanged -= ShakeOnDamage;

        public void ShakeCamera(float intensity, float time)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeTimer(intensity, time));
        }


        IEnumerator ShakeTimer(float intensity, float time)
        {
            perlin.m_AmplitudeGain = intensity;
            perlin.m_FrequencyGain = intensity;
            float currentTime = time;
            while (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                perlin.m_AmplitudeGain = Mathf.Lerp(0, intensity, currentTime / time);
                yield return null;
            }
            perlin.m_AmplitudeGain = 0;
        }

        IEnumerator HitStop(float time)
        {
            var orgTime = Time.timeScale;
            Time.timeScale = 0;
         
            isTimeFrozen = true;

            yield return new WaitForSecondsRealtime(time);
            Time.timeScale = orgTime;
            
            isTimeFrozen = false;
        }

        public void ShakeOnDamage(object sender, float oldHealth, float newHealth)
        {
            if (newHealth >= oldHealth) return;

            float damageDone = oldHealth - newHealth;
            float prctRelativeToMax = Mathf.Clamp01(damageDone / baseHealth);

            float damageSeverity = Mathf.Lerp(minShakeIntensity, maxShakeIntensity, prctRelativeToMax);
            float shakeTime = Mathf.Lerp(minShakeTime, maxShakeTime, prctRelativeToMax);
            
            ShakeCamera(damageSeverity, shakeTime);

            if (!canHitStop) return;

            float hitStopTime = Mathf.Lerp(minHitStopTime, maxHitStopTime, prctRelativeToMax);

            if (!isTimeFrozen)
                StartCoroutine(HitStop(hitStopTime));
            
        }
    }
}