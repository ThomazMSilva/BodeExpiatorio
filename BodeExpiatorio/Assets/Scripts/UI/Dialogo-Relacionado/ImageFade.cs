using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageFade : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float fadeTime = 1f;

    private Color originalColor;

    private void Awake()
    {
        originalColor = image.color;
    }

    private void OnEnable()
    {
        StartCoroutine(FadeIn());
    }

    public void DisableImage()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float timeMultiplier = 1 / fadeTime;
        Color currentColor = originalColor;
        currentColor.a = 0;
        image.color = currentColor;

        while (image.color.a < originalColor.a)
        {
            currentColor.a += Time.deltaTime * timeMultiplier;
            image.color = currentColor;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float timeMultiplier = 1 / fadeTime;
        Color currentColor = originalColor;
        while (image.color.a > 0)
        {
            currentColor.a -= Time.deltaTime * timeMultiplier;
            image.color = currentColor;
            yield return null;
        }
        image.gameObject.SetActive(false);
    }
}
