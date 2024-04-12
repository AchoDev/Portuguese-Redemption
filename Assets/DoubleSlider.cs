using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSlider : MonoBehaviour
{

    [SerializeField] RectTransform body;
    [SerializeField] RectTransform preBody;
    [SerializeField] RectTransform background;

    [SerializeField] float borderWidth = 10;

    float maxSize;

    public float value = 100;

    // Start is called before the first frame update
    void Start()
    {
        maxSize = background.sizeDelta.x - borderWidth * 2;
        body.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
