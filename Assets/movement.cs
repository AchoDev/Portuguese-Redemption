using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

    Animator animator;
    Rigidbody2D rb;

    [SerializeField] float speed = 10.0f;

    float moveDirection = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = Input.GetAxis("Horizontal");
    }

    void FixedUpdate() 
    {
        rb.velocity = new Vector2(moveDirection, rb.velocity.y) * speed;

        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            animator.SetBool("running", true);
            transform.rotation = moveDirection > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        } else 
        {
            animator.SetBool("running", false);
        }
    }
}
