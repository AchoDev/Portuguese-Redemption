using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fading : MonoBehaviour
{

    [SerializeField]
    float defaultDuration = 0.5f;

    Image image;

    [SerializeField] List<Image> subImages;
    [SerializeField] List<TextMeshProUGUI> subTexts;

    float frameCount = 100f;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void FadeInAsync()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeOutAsync()
    {
        StartCoroutine(FadeOut());
    }

    public void FadeInOutAsync()
    {
        StartCoroutine(FadeInOut());
    }

    public IEnumerator FadeInOut()
    {
        yield return StartCoroutine(InitiateFadeIn(defaultDuration));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(InitiateFadeOut(defaultDuration));
    }

    public IEnumerator FadeOut() 
    {
        yield return StartCoroutine(InitiateFadeOut(defaultDuration));
    }

    public IEnumerator FadeOut(float duration)
    {
        yield return StartCoroutine(InitiateFadeOut(duration));
    }

    public IEnumerator FadeIn() 
    {
        yield return StartCoroutine(InitiateFadeIn(defaultDuration));
    }

    public IEnumerator FadeIn(float duration)
    {
        yield return StartCoroutine(InitiateFadeIn(duration));
    }

    IEnumerator InitiateFadeIn(float duration)
    {
        float timeBetweenFrames = duration / frameCount;

        for (float f = 0f; f <= 1; f += 1 / frameCount)
        {
            Color c = image.color;
            c.a = f;
            image.color = c;

            foreach (Image subImage in subImages)
            {
                Color subC = subImage.color;
                subC.a = f;
                subImage.color = subC;
            }

            foreach (TextMeshProUGUI subText in subTexts)
            {
                Color subC = subText.color;
                subC.a = f;
                subText.color = subC;
            }

            yield return new WaitForSeconds(timeBetweenFrames);
        }
    }

    IEnumerator InitiateFadeOut(float duration)
    {
        float timeBetweenFrames = duration / frameCount;

        for (float f = 1f; f >= 0; f -= 1 / frameCount)
        {
            Color c = image.color;
            c.a = f;
            image.color = c;
         
            foreach (Image subImage in subImages)
            {
                Color subC = subImage.color;
                subC.a = f;
                subImage.color = subC;
            }

            foreach (TextMeshProUGUI subText in subTexts)
            {
                Color subC = subText.color;
                subC.a = f;
                subText.color = subC;
            }

            yield return new WaitForSeconds(timeBetweenFrames);
        }
    }
}
