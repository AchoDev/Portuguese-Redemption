using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadeTransition : MonoBehaviour
{
    [SerializeField] UnityEvent OnFadeInComplete;
    [SerializeField] Fading transitioner;

    public void InitiateTransition() {
        StartCoroutine(Transition());
    }

    IEnumerator Transition() {
        yield return StartCoroutine(transitioner.FadeIn());
        OnFadeInComplete.Invoke();
        yield return StartCoroutine(transitioner.FadeOut());
    }
}
