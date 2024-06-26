using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Hardware;
using UnityEditor.Rendering;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class FlightController : MonoBehaviour
{

    Animator animator;
    Rigidbody2D rb;
    int flightDirection = 0;

    
    [SerializeField] Transform startPoint;
    [Space(10)]
    [SerializeField] float liftForce = 100f;
    [SerializeField] float speedForce = 100f;
    [SerializeField] float boostForce = 100f;

    [SerializeField] float rotationAmount = 2f;
    [SerializeField] float upliftMultiplier = 2f;
    [SerializeField] float upliftDrag = 2f;

    [SerializeField] Vector2 jumpForce = new Vector2(1, 10);

    bool gameStarted = false;
    bool diving = false;
    bool dead = false;
    Vector3 rotationVel = Vector3.zero;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground" && !dead)
        {
            animator.SetTrigger("fall");
            // rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.right * 20 / rb.velocity.x, ForceMode2D.Impulse);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            // rb.freezeRotation = true;
            dead = true;
            // GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }
    }
    

    // Start is called before the first frame update
    
    public void StartInitialDive()
    {
        diving = true;
    }

    public void InspectWingsuit()
    {
        animator.SetBool("checkingWingsuit", true);
    }

    public void StopInspectingWingsuit()
    {
        animator.SetBool("checkingWingsuit", false);
    }

    public void StartGame() 
    {
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        animator.SetTrigger("startGame");
        transform.position = startPoint.position;
    }

    public void WingsuitDeployed()
    {
        animator.SetBool("gameStarted", true);
        // rb.AddForce(new Vector2(1, 1) * 10);
    }

    public void Jump()
    {
        gameStarted = true;
        rb.simulated = true;
        GetComponent<CircleCollider2D>().enabled = true;
        rb.AddForce(jumpForce, ForceMode2D.Force);
    }

    public void StartDiving()
    {
        diving = true;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // rb.AddForce(10 * Vector2.right, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        flightDirection = -(int)Input.GetAxisRaw("Vertical");
        animator.SetInteger("flightDirection", flightDirection);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("space");
        
            if(diving) {
                Debug.Log("stop diving");
                diving = false;
            } else {
                rb.AddForce(Vector2.right * boostForce, ForceMode2D.Impulse);
            }

        }

    }

    // public void Boost() {
    //     rb.AddForce(transform.forward * liftForce, ForceMode2D.Impulse);
    // }

    float xForceFunction(float x)
    {
        // return -2 * (x * x) + 0.5f;
        return (float)Math.Pow(System.Math.E, -Math.Pow(5 * x, 2));
    }

    void applyForces() 
    {
        float gravity = -9.81f;

        float rotation = Mathf.Clamp(rb.velocity.y * rotationAmount - 90, -180, 0);        
        float rotationPercentage = (rotation + 1 + 180) / 180;

        // float xForce = -rb.velocity.y * speedForce * xForceFunction(rotationPercentage - 0.5f);
        // float yForce = liftForce * (rb.velocity.x * horizontalSpeedMultiplier) * (1 - rotationPercentage);

        float xForce = -rb.velocity.y * speedForce;
        float yForce = liftForce * rb.velocity.x;

        if(flightDirection == -1 || dead || diving)
        {
            yForce = 0;
            xForce = 0;
        } else if(flightDirection == 1) {
            Debug.Log("flyin up");
            yForce += upliftMultiplier * rb.velocity.x;
            float newXForce = -upliftDrag * Mathf.Max(0, rb.velocity.y);
            if(newXForce > 0) {
                xForce = newXForce;
            }


            if(rb.velocity.y > 0 && yForce < 0.1) {
                animator.SetTrigger("flip");
            }
        }

 
        yForce += gravity * 1.3f;

        // Debug.Log($"xForce: {xForce}, yForce: {yForce}, rotation: {rotation}, rotationPercentage: {rotationPercentage}, xForceFunction: {xForceFunction(rotationPercentage - 0.5f)}");

        rb.AddForce(new Vector2(xForce, yForce));
    }

    void FixedUpdate() 
    {
        if(gameStarted) {
            applyForces();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, Mathf.Clamp(rb.velocity.y * rotationAmount - 90, -180, 0)), 0.1f);
        }
    }
}
