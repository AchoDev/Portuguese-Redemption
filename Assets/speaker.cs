using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class speaker : MonoBehaviour
{
    // Start is called before the first frame update

    [TextArea(3, 10)]
    [SerializeField] new string name;
    [SerializeField] string[] dialogue;
    [SerializeField, Range(0, 0.1f)] float timeBetweenLetters = 0.025f;


    BoxCollider2D trigger;
    TextMeshProUGUI textBox;
    TextMeshProUGUI nameBox;
    Animator animator;
    movement playerMovement;

    bool playerInTrigger = false;
    bool animationFinished = false;
    bool talking = false;
    int currentLine = 0;
    int currentIndex = 0;
    string currentText = "";

    void Start()
    {
        animator = GameObject.Find("Canvas/dialogue").GetComponent<Animator>();
        textBox = GameObject.Find("Canvas/dialogue/content").GetComponent<TextMeshProUGUI>();
        nameBox = GameObject.Find("Canvas/dialogue/name").GetComponent<TextMeshProUGUI>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<movement>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger");
            playerInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger");
            playerInTrigger = false;
        }
    }

    void initiateTalk()
    {
        talking = true;
        animationFinished = false;
        currentText = "";
        animator.SetBool("speaking", true);
        nameBox.text = name;
        currentLine = 0;
        playerMovement.allowMovement = false;
        StartCoroutine(finishAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        if(talking) textBox.text = currentText;
        
        if(!(playerInTrigger && Input.GetKeyDown(KeyCode.E))) return;
        
        
        if(!talking) {
            initiateTalk();
            return;
        } 
        
        if(currentIndex < dialogue[currentLine].Length) // skip to end of line
        {
            currentIndex = dialogue[currentLine].Length;
            currentText = dialogue[currentLine];
        } else // go to next line
        {
            currentLine++;
            if(currentLine >= dialogue.Length) // stop dialogue after last line
            {
                talking = false;
                currentLine = 0;
                currentText = "";
                animator.SetBool("speaking", false);
                playerMovement.allowMovement = true;
            } else 
            {
                StartCoroutine(talk());
            }
        }

        
    }

    IEnumerator talk()
    {
        currentIndex = 0;
        int line = currentLine;
        while(currentIndex < dialogue[line].Length && currentLine == line)
        {
            currentIndex++;
            currentText = dialogue[currentLine].Substring(0, currentIndex);
            yield return new WaitForSeconds(timeBetweenLetters);
        }
    }

    IEnumerator finishAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(talk());
        animationFinished = true;
    }

    void Reset()
    {
        trigger = GetComponent<BoxCollider2D>();
        trigger.isTrigger = true;
    }
}
