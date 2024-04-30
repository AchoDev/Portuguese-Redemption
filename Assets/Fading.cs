using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fading : MonoBehaviour
{

    [SerializeField]
    float defaultDuration = 0.5f;

    Image image;

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
            yield return new WaitForSeconds(timeBetweenFrames);
        }
    }
}
