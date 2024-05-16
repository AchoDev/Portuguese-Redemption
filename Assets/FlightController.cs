using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class FlightController : MonoBehaviour
{

    Animator animator;
    Rigidbody2D rb;
    int flightDirection = 0;

    [SerializeField] float liftForce = 100f;
    [SerializeField] float speedForce = 100f;
    [SerializeField] float diveMultiplier = 4f;
    [SerializeField] float diveDrag = 0.3f; 
    [SerializeField, Range(0, 0.1f)] float horizontalSpeedMultiplier = 4f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        rb.AddForce(10 * Vector2.right, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        flightDirection = -(int)Input.GetAxisRaw("Vertical");
        animator.SetInteger("flightDirection", flightDirection);
    }

    void FixedUpdate() 
    {
        float gravity = -9.81f;

        float xForce = Mathf.Max(0, -rb.velocity.y * speedForce);
        float yForce = liftForce * (flightDirection + 1) * (rb.velocity.x * horizontalSpeedMultiplier);

        if(flightDirection == 1)
        {
            yForce *= diveMultiplier;
            xForce = -rb.velocity.x * diveDrag;
        } else if(flightDirection == -1)
        {
            yForce *= diveMultiplier;
            xForce = 0;
        }

        yForce += gravity * 1.3f;

        Debug.Log($"xForce: {xForce}, yForce: {yForce}");

        rb.AddForce(new Vector2(xForce, yForce));
    }
}
