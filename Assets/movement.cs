using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

    Animator animator;
    Rigidbody2D rb;

    [SerializeField] float speed;
    [SerializeField] float sprintSpeed;

    bool sprinting = false;

    float moveDirection = 0;
    float currentRotation = 0;
    
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

        sprinting = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("sprinting", sprinting);
    }

    void FixedUpdate() 
    {
        rb.velocity = new Vector2(moveDirection * speed * (sprinting ? sprintSpeed : 1), rb.velocity.y);

        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            Quaternion.Euler(0, currentRotation, 0), 
            0.15f
        );

        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            currentRotation = moveDirection > 0 ? 0 : 180;
            animator.SetBool("running", true);
        } else 
        {
            animator.SetBool("running", false);
        }
    }
}
