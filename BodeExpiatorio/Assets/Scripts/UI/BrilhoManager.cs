using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BrilhoManager : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Volume exposureVolume;
    private VolumeProfile exposureProfile;
    private ColorAdjustments exposure;
    [SerializeField] private float minimumIntensity = 0.01f;
    [SerializeField] private float maximumIntensity = 4f;
    [SerializeField] private string brightnessKey = "brightness";

    private void Start()
    {

        if (exposureVolume == null || exposureVolume.profile == null)
        {
            Debug.LogError("Exposure volume or its profile is not assigned.");
            return;
        }

        exposureProfile = exposureVolume.profile;

        if (!exposureProfile.TryGet(out exposure))
        {
            Debug.LogError("ColorAdjustments override is missing from the volume profile.");
            return;
        }
        //else { Debug.Log("Tem ColorAdjustments"); }

        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        brightnessSlider.value = PlayerPrefs.GetFloat(brightnessKey);
        //SetBrightness(Mathf.Lerp(minimumIntensity, maximumIntensity, brightnessSlider.value));
    }

    public void SetBrightness(float intensity)
    {
        PlayerPrefs.SetFloat(brightnessKey, intensity);
        float interpolatedIntensity = Mathf.Lerp(minimumIntensity, maximumIntensity, intensity);

        exposure.postExposure.Override(interpolatedIntensity);
    }
}
