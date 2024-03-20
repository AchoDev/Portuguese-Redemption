using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

enum People 
{  
    [EnumMember(Value = "Leo")]
    Leo,
}

[System.Serializable]
class Message {
    public People name;
    [TextArea(3, 10)]
    public string dialogue;
    public string emotion = "neutral";
}

[RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(CameraFocusPoint))]
public class speaker : MonoBehaviour
{
    // Start is called before the first frame update

    [TextArea(3, 10)]
    [SerializeField] new string name;
    [SerializeField] Message[] dialogue;
    [SerializeField, Range(0, 0.1f)] float timeBetweenLetters = 0.025f;


    BoxCollider2D trigger;
    CameraFocusPoint cameraFocusPoint;

    TextMeshProUGUI textBox;
    TextMeshProUGUI nameBox;
    Animator animator;
    movement playerMovement;
    GameObject eIndicator;

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
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<movement>();
        eIndicator = player.transform.Find("indicator").gameObject;

        cameraFocusPoint = GetComponent<CameraFocusPoint>();
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
        currentIndex = 0;

        playerMovement.allowMovement = false;

        
        cameraFocusPoint.Focus(
            (transform.position + playerMovement.transform.position) / 2
        );

        StartCoroutine(finishAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        if(talking) textBox.text = currentText;
        eIndicator.SetActive(playerInTrigger && !talking);
        
        if(!(playerInTrigger && Input.GetKeyDown(KeyCode.E))) return;
        
        
        if(!talking) {
            initiateTalk();
            return;
        } 
        
        string text = dialogue[currentLine].dialogue;
        if(currentIndex < text.Length) // skip to end of line
        {
            currentIndex = text.Length - 1;
            currentText = text;
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
                cameraFocusPoint.Unfocus();
            } else 
            {
                StartCoroutine(talk());
            }
        }
    }

    void setNewImage()
    {
        GameObject parent = animator.transform.Find(dialogue[currentLine].name.ToString()).gameObject;
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(false);
        }

        parent.transform.Find(dialogue[currentLine].emotion).gameObject.SetActive(true);
    }

    IEnumerator talk()
    {
        currentIndex = 0;
        int line = currentLine;
        while(currentIndex < dialogue[currentLine].dialogue.Length && currentLine == line)
        {
            currentIndex++;
            currentText = dialogue[currentLine].dialogue.Substring(0, currentIndex);
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

        cameraFocusPoint = GetComponent<CameraFocusPoint>();
    }
}
