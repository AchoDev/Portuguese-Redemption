using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

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
    IEnumerator currentCoroutine;
    bool attacking = false;
    int moveDirection;

    Vector3 originalSize;
    Vector3 turnVelocity;

    // Start is called before the first frame update
    void Start()
    {
        originalSize = transform.localScale;
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        StartCoroutine(IterateStates());

        currentCoroutine = Attack();
        StartCoroutine(currentCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetInteger("moveDirection", moveDirection);
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
                        animator.SetTrigger("punch");
                        yield return new WaitForSeconds(0.1f);
                        break;
                    case 3:
                        moveDirection = -1;
                        break;
                }
                yield return new WaitForSeconds(0.25f);
                animator.SetTrigger("punch");
            } else 
            {
                moveDirection = (int)Mathf.Sign(player.transform.position.x - transform.position.x);
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
        float time = Random.Range(1, 15);

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
