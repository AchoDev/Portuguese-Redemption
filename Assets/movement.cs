using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

    Animator animator;
    Rigidbody2D rb;

    GameObject scooter;

    [SerializeField] float speed;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float driveMultiplier;

    bool sprinting = false;
    bool driving = false;
    public bool allowMovement = true;

    float moveDirection = 0;
    float currentRotation = 0;
    Vector3 rotationVelocity = Vector3.zero;
    float normalXSize;
    float currentXSize;

    SoundManager soundManager;

    public void setMovement(bool value) 
    {
        allowMovement = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        scooter = GameObject.FindWithTag("Scooter");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        normalXSize = transform.localScale.x;
        currentXSize = normalXSize;

        soundManager = GameObject.FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {

        // float newMoveDirection = Input.GetAxis("Horizontal");

        // if(newMoveDirection != moveDirection && driving) {
        //     soundManager.Play("scooter");
        // }

        moveDirection = Input.GetAxis("Horizontal");

        if(!driving)
        {
            sprinting = Input.GetKey(KeyCode.LeftShift);
            animator.SetBool("sprinting", sprinting);
        }

        if(Input.GetKeyDown("e") && driving)
        {
            StartCoroutine(stopDriving());
        }
    }

    void FixedUpdate() 
    {
        transform.localScale = Vector3.SmoothDamp(
            transform.localScale, 
            new Vector3(currentXSize, transform.localScale.y, transform.localScale.z), 
            ref rotationVelocity,
            0.1f
        );
        
        if(!allowMovement) moveDirection = 0;

        rb.velocity = new Vector2(moveDirection * speed * (sprinting && !driving ? sprintMultiplier : 1) * (driving ? driveMultiplier : 1), rb.velocity.y);


        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            currentXSize = moveDirection > 0 ? normalXSize : -normalXSize;
            animator.SetBool("running", true);

        } else 
        {
            animator.SetBool("running", false);
        }
    }

    public void startDriving()
    {
        StartCoroutine(drive());
    }

    IEnumerator stopDriving()
    {
        animator.SetTrigger("exit");
        driving = false;
        yield return new WaitForSeconds(0.4f);
        scooter.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);

        Vector3 scooterSize = scooter.transform.localScale;
        float scooterX = transform.localScale.x > 0 ? Mathf.Abs(scooterSize.x) : -Mathf.Abs(scooterSize.x);
        scooter.transform.localScale = new Vector3(scooterX, scooterSize.y, scooterSize.z);

        scooter.gameObject.SetActive(true);
    }

    IEnumerator drive()
    {
        animator.SetTrigger("drive");
        yield return new WaitForEndOfFrame();
        driving = true;
    }

    public void playRunningSound() 
    {
        soundManager.Play("step");
    }
}
