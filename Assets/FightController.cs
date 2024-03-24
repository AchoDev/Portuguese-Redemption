using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FightController : MonoBehaviour
{
    
    [SerializeField] float speed = 5;

    Rigidbody2D rb;
    Animator animator;
    [SerializeField] GameObject enemy;

    int movementDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movementDirection = (int)Input.GetAxisRaw("Horizontal");
        
        if(transform.position.x < enemy.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            animator.SetInteger("moveDirection", movementDirection);
        } else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            animator.SetInteger("moveDirection", -movementDirection);
        }

        if(movementDirection == 0)
        {
            animator.SetInteger("moveDirection", 0);
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("frontPunch");
        }

        if(Input.GetKey(KeyCode.S))
        {
            animator.SetBool("crouch", true);
            movementDirection = 0;
        } else
        {
            animator.SetBool("crouch", false);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(movementDirection * speed, rb.velocity.y);
    }
}
