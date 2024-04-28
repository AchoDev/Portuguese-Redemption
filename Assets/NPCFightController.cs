using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
    bool attacking = false;
    bool dead = false;
    int moveDirection;

    TextMeshProUGUI debugText;

    float health = 100;
    bool falling = false;

    Vector3 originalSize;
    Vector3 turnVelocity;

    [HideInInspector] public float idleProbability = 0.33f;
    [HideInInspector] public float attackProbability = 0.33f;
    [HideInInspector] public float defendProbability = 0.33f;

    [SerializeField] float speed = 1.5f;
    [SerializeField] float deathDuration = 5f;
    [SerializeField] UnityEvent onDeath;

    DoubleSlider healthBar;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.Find("enemy health")?.GetComponent<DoubleSlider>();
        originalSize = transform.localScale;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        
        debugText = GameObject.Find("debug info")?.GetComponent<TextMeshProUGUI>();
        
        StartCoroutine(IterateStates());
    }

    // Update is called once per frame
    void Update()
    {

        healthBar.value = health;

        if(debugText != null) {
            debugText.text = "State: " + currentState + "\n" + "Attacking: " + attacking;
        }

        if(attacking || dead) {
            moveDirection = 0;
            animator.SetInteger("moveDirection", 0);
            return;
        };

        animator.SetInteger("moveDirection", moveDirection * (player.transform.position.x < transform.position.x ? -1 : 1));
        rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);

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
            if(dead) yield break;
            if(attacking) {
                moveDirection = 0;
                yield return null;
                continue;
            }
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
                        yield return new WaitForSeconds(0.1f);
                        animator.SetTrigger("punch");

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

    IEnumerator Idle() 
    {
        while(true)
        {
            if(dead) yield break;
            moveDirection = 0;
            yield return null;
        }
    }

    IEnumerator Defend() 
    {
        while(true)
        {
            if(dead) yield break;
            int playerDirection = (int)Mathf.Sign(player.transform.position.x - transform.position.x);

            if(Vector2.Distance(transform.position, player.transform.position) < 4.5f)
            {
                moveDirection = -playerDirection;
            } else 
            {
                moveDirection = 0;
            }


            yield return null;
        }
    }

    IEnumerator IterateStates() 
    {
        while(true)
        {
            yield return StartCoroutine(HoldState());
        }
    }

    IEnumerator HoldState() 
    {
        float random = Random.value;
        int id;

        if(random < idleProbability) id = 0;
        else if(random < idleProbability + attackProbability) id = 1;
        else id = 2;

        float time;

        if(attacking) {
            while(attacking) {
                yield return new WaitForSeconds(0.1f);
            }
        }

        if(currentCoroutine != null) {
            StopCoroutine(currentCoroutine);
        }

        switch (id)
        {
            case 0:
                time = Random.Range(100, 1000);

                currentState = State.Idle;
                currentCoroutine = Idle();
                StartCoroutine(currentCoroutine);
                break;
            case 1:

                time = Random.Range(100, 2500);
                currentState = State.Attack;
                currentCoroutine = Attack();
                StartCoroutine(currentCoroutine);
                break;
            case 2:

                time = Random.Range(500, 2000);
                currentState = State.Defend;
                currentCoroutine = Defend();
                StartCoroutine(currentCoroutine);
                break;

            default:
                time = Random.Range(1000, 3000);
                break;
        }

        yield return new WaitForSeconds(time / 1000f);
    }

    public void takeDamage(float damage) 
    {

        if(dead) return;

        float value = Random.value;
        float damageTaken = damage;

        
        if(value < 0.2) {
            damageTaken *= 2;
            animator.SetTrigger("critical");
        } else if(!falling) {
            animator.SetTrigger("hit");
        }
        

        health = Mathf.Clamp(health - damageTaken, 0, 100);
        if(health == 0) {
            animator.SetTrigger("die");
            dead = true;
            StartCoroutine(endDeath());
        }
        falling = true;


    }

    public IEnumerator endDeath() 
    {
        yield return new WaitForSeconds(deathDuration);
        onDeath.Invoke();
    }

    public void endFalling() 
    {
        falling = false;
    }
}
