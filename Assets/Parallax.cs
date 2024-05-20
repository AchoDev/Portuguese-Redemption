using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    float length, startpos;
    public GameObject cam;
    public float parallaxEffect;

    float startY, startOrtho;
    Vector3 startSize;

    void Awake() 
    {
        startOrtho = Camera.main.orthographicSize;
    }

    void Start()
    {
        startSize = transform.localScale;
        startY = cam.transform.position.y;
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        length = GetComponent<SpriteRenderer>().bounds.size.x * transform.localScale.x;

        transform.position = new Vector3(startpos + dist, cam.transform.position.y, transform.position.z);
        transform.localScale = startSize * (Camera.main.orthographicSize / startOrtho);


        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;        
    }
}
