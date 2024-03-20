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
    public bool allowMovement = true;

    float moveDirection = 0;
    float currentRotation = 0;
    Vector3 rotationVelocity = Vector3.zero;
    float normalXSize;
    float currentXSize;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        normalXSize = transform.localScale.x;
        currentXSize = normalXSize;
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
        transform.localScale = Vector3.SmoothDamp(
            transform.localScale, 
            new Vector3(currentXSize, transform.localScale.y, transform.localScale.z), 
            ref rotationVelocity,
            0.1f
        );
        
        if(!allowMovement) return;

        rb.velocity = new Vector2(moveDirection * speed * (sprinting ? sprintSpeed : 1), rb.velocity.y);


        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            currentXSize = moveDirection > 0 ? normalXSize : -normalXSize;
            animator.SetBool("running", true);
        } else 
        {
            animator.SetBool("running", false);
        }
    }
}
