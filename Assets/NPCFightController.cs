using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCFightController : MonoBehaviour
{

    enum State 
    {
        Idle,
        Attack,
        Defend,
    }

    State currentState = State.Idle;
    GameObject player;
    Animator animator;
    Rigidbody2D rb;
    IEnumerator currentCoroutine;
    [SerializeField]bool attacking = false;
    int moveDirection;

    Vector3 originalSize;
    Vector3 turnVelocity;

    // Start is called before the first frame update
    void Start()
    {
        originalSize = transform.localScale;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        StartCoroutine(IterateStates());

        currentCoroutine = Attack();
        StartCoroutine(currentCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        if(attacking) return;

        animator.SetInteger("moveDirection", moveDirection * (player.transform.position.x < transform.position.x ? -1 : 1));
        rb.velocity = new Vector2(moveDirection * 1.5f, rb.velocity.y);

        Vector3 turnSize;
        if(player.transform.position.x < transform.position.x)
        {
            turnSize = new Vector3(-originalSize.x, originalSize.y, originalSize.z);
        } else 
        {
            turnSize = new Vector3(originalSize.x, originalSize.y, originalSize.z);
        }
        
        transform.localScale = Vector3.SmoothDamp(
            transform.localScale, 
            turnSize, 
            ref turnVelocity, 
            0.1f
        );
    }

    IEnumerator Attack() 
    {
        while(true)
        {
            if(attacking) continue;
            if(Vector2.Distance(transform.position, player.transform.position) < 1.5f)
            {
                attacking = true;
                int attackId = Random.Range(0, 3);
                switch(attackId)
                {
                    case 0:
                        moveDirection = 0;
                        break;
                    case 1:
                        moveDirection = 1;
                        break;
                    case 2:
                        moveDirection = 1;
                        break;
                    case 3:
                        moveDirection = -1;
                        break;
                }
                yield return new WaitForSeconds(0.1f);
                animator.SetTrigger("punch");
                moveDirection = 0;
                yield return new WaitForSeconds(1f);
                attacking = false;

            } else 
            {
                moveDirection = (int)Mathf.Sign(player.transform.position.x - transform.position.x);
                yield return null;
            }   
        }
        
    }

    IEnumerator IterateStates() 
    {
        while(true)
        {
            yield return StartCoroutine(HoldState());

            Debug.Log("State: " + currentState);
        }
    }

    IEnumerator HoldState() 
    {
        int id = Random.Range(0, 3);
        float time = Random.Range(5, 15);

        switch (id) 
        {
            case 0:
                currentState = State.Idle;
                break;
            case 1:
                currentState = State.Attack;
                if(currentCoroutine != null) {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = Attack();
                StartCoroutine(currentCoroutine);
                break;
            case 2:
                currentState = State.Defend;
                break;
        }

        yield return new WaitForSeconds(time);
    }
}
