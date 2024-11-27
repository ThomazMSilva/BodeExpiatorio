using UnityEngine;
using UnityEngine.UI;

public class BrilhoManager : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Light directionalLight;
    [SerializeField] private float minimumIntensity = 0.01f;
    [SerializeField] private float maximumIntensity = 1f;
    [SerializeField] private string brightnessKey = "brightness";

    private void Start()
    {
        brightnessSlider.value = PlayerPrefs.GetFloat(brightnessKey);
        SetBrightness(Mathf.Lerp(minimumIntensity, maximumIntensity, brightnessSlider.value));
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }

    public void SetBrightness(float intensity)
    {
        PlayerPrefs.SetFloat(brightnessKey, intensity);
        Debug.Log($"setting {directionalLight.gameObject.name}'s intensity from {directionalLight.intensity} to {Mathf.Lerp(minimumIntensity, maximumIntensity, intensity)}");
        directionalLight.intensity = Mathf.Lerp(minimumIntensity, maximumIntensity, intensity);
    }
}
