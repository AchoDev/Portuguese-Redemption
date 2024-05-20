using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] float boostForce = 100f;

    [SerializeField] float rotationAmount = 2f;
    [SerializeField] float rotationMultiplier = 0.1f;

    

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            animator.SetTrigger("fall");
            // rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.right * 20 / rb.velocity.x, ForceMode2D.Impulse);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            // rb.freezeRotation = true;
        }
    }
    

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

        if(Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.right * boostForce, ForceMode2D.Impulse);
            animator.SetTrigger("boost");
        }

    }

    // public void Boost() {
    //     rb.AddForce(transform.forward * liftForce, ForceMode2D.Impulse);
    // }

    void FixedUpdate() 
    {
        float gravity = -9.81f;

        float rotation = Mathf.Clamp(rb.velocity.y * rotationAmount - 90, -180, 0);
        
        rotation += rotationMultiplier * flightDirection;
        
        float rotationPercentage = (rotation + 1 + 180) / 180;

        float xForce = Mathf.Max(0, -rb.velocity.y * speedForce * rotationPercentage);
        float yForce = liftForce * (rb.velocity.x * horizontalSpeedMultiplier) * (1 - rotationPercentage);

        // if(flightDirection == 1)
        // {
        //     yForce *= diveMultiplier;
        //     xForce = -rb.velocity.x * diveDrag;
        // } else if(flightDirection == -1)
        // {
        //     yForce *= diveMultiplier;
        //     xForce = 0;
        // }



        yForce += gravity * 1.3f;

        Debug.Log($"xForce: {xForce}, yForce: {yForce}, rotation: {rotation}, rotationPercentage: {rotationPercentage}");

        rb.AddForce(new Vector2(xForce, yForce));

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(rb.velocity.y * rotationAmount - 90, -180, 0));
    }
}
