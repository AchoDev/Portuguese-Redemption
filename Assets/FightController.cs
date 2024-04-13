using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FightController : MonoBehaviour
{
    
    [SerializeField] float speed = 5;

    float health = 100;

    Rigidbody2D rb;
    Animator animator;
    GameObject enemy;

    int movementDirection;
    bool attacking = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemy = GameObject.FindWithTag("Enemy");
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

        if(attacking) 
        {
            movementDirection = 0;
            animator.SetInteger("moveDirection", 0);
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("frontPunch");
            attacking = true;
        }

        if(!attacking) 
        {
            if(Input.GetKey(KeyCode.S))
            {
                animator.SetBool("crouch", true);
                movementDirection = 0;
            } else
            {
                animator.SetBool("crouch", false);
            }
        }
    }
    
    public void setAttackingFalse()
    {
        attacking = false;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(movementDirection * speed, rb.velocity.y);
    }

    public void damageEnemy(float damage)
    {
        enemy.GetComponent<NPCFightController>().takeDamage(damage);
    }
}
