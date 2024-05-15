using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class FlightController : MonoBehaviour
{

    Animator animator;
    int flightDirection = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        flightDirection = (int)Input.GetAxisRaw("Vertical");
        animator.SetInteger("flightDirection", flightDirection);
    }

    void FixedUpdate() 
    {
        transform.position += new Vector3(0.1f, flightDirection * 0.1f, 0);
    }
}
