using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSlider : MonoBehaviour
{

    [SerializeField] RectTransform body;
    [SerializeField] RectTransform preBody;
    RectTransform background;

    [SerializeField, Range(0, 5)] float borderWidth = 10;

    [SerializeField, Range(0, 100)] float _value = 100;
    float preValue = 100;

    [SerializeField, Range(0.1f, 0.5f)] float smoothDuration = 0.25f;
    [SerializeField, Range(5f, 50)] float longSmoothVelocity = 1f;
    [SerializeField, Range(0.1f, 1)] float pause = 0.5f;


    public float value {
        get {
            return _value;
        }
        set {
            StartCoroutine(MoveTo(value));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        background = GetComponent<RectTransform>();
        preValue = _value;
    }

    // Update is called once per frame
    void Update()
    {
        body.sizeDelta = new Vector2((background.sizeDelta.x - borderWidth * 2) * value / 100, background.sizeDelta.y - borderWidth * 2);
        body.anchoredPosition = new Vector2(-(background.sizeDelta.x / 2) + borderWidth + body.sizeDelta.x / 2, body.anchoredPosition.y);

        preBody.sizeDelta = new Vector2((background.sizeDelta.x - borderWidth * 2) * preValue / 100, background.sizeDelta.y - borderWidth * 2);
        preBody.anchoredPosition = new Vector2(-(background.sizeDelta.x / 2) + borderWidth + preBody.sizeDelta.x / 2, preBody.anchoredPosition.y);
    }

    IEnumerator MoveTo(float value) {
        float startValue = this.value;
        float endValue = value;

        float t = 0;

        while (_value - endValue > 0.01f || _value - endValue < -0.01f) {

            t += Time.deltaTime / smoothDuration;

            _value = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }

        yield return new WaitForSeconds(pause);

        t = 0;

        float longSmoothDuration = Mathf.Abs(endValue - preValue) / longSmoothVelocity;

        while(preValue - endValue > 0.01f || preValue - endValue < -0.01f) {

            t += Time.deltaTime / longSmoothDuration;

            preValue = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }
    }
}
