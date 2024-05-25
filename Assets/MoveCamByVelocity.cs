using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using GD.MinMaxSlider;

public class MoveCamByVelocity : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    Rigidbody2D rb;
    
    float velocity;

    [SerializeField, MinMaxSlider(0, 10)] Vector2 minMaxOrthoSize = new Vector2(1, 5);
    [SerializeField] float orthoSizeMultiplier = 1;

    void Start()
    {   
        cam = GameObject.FindGameObjectWithTag("CMVcam").GetComponent<CinemachineVirtualCamera>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float newOrtho = Mathf.Clamp(minMaxOrthoSize.x + rb.velocity.magnitude * orthoSizeMultiplier, minMaxOrthoSize.x, minMaxOrthoSize.y);
        
        
        cam.m_Lens.OrthographicSize = Mathf.SmoothDamp(cam.m_Lens.OrthographicSize, newOrtho, ref velocity, 0.5f);
    }
}
