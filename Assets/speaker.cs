using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEditor.Search;
using UnityEditor.UI;
using UnityEngine;

enum People 
{  
    LeoSuit,
    BickNuterus,
}

enum Emotion {
    neutral,
    annoyed,
    sad,
    angry,
    happy,
}

[System.Serializable]
class Message {
    public People name;
    [TextArea(3, 10)]
    public string dialogue;
    public Emotion emotion = Emotion.neutral;
}

[RequireComponent(typeof(BoxCollider2D))]
public class speaker : MonoBehaviour
{
    // Start is called before the first frame update

    [Space(10)]
    [Header("General")]
    [SerializeField] bool turnTowardsPlayer = false;
    [SerializeField] bool setAnimatorTalking = false;
    [SerializeField, Range(0, 0.1f)] float timeBetweenLetters = 0.025f;
    
    [Space(10)]
    [Header("Camera")]
    [SerializeField] bool focusCamera = true;
    [SerializeField, Range(0.1f, 5f)] float cameraSpeed = 1f;
    [SerializeField, Range(0.1f, 5)] float ortho = 2.5f;

    [Space(10)]
    [Header("Dialogue")]
    [SerializeField] Message[] dialogue;


    BoxCollider2D trigger;
    CameraFocusPoint cameraFocusPoint;

    TextMeshProUGUI textBox;
    TextMeshProUGUI nameBox;
    Animator animator;
    movement playerMovement;
    GameObject eIndicator;

    Vector3 turnVelocity = Vector3.zero;
    float originalSize;

    bool playerInTrigger = false;
    bool talking = false;

    int currentLine = 0;
    int currentIndex = 0;
    string currentText = "";

    string SplitCamelCase(string source) {
        return Regex.Split(source, @"(?<!^)(?=[A-Z])").Aggregate((a, b) => a + " " + b);
    }

    void Start()
    {
        animator = GameObject.Find("Canvas/dialogue").GetComponent<Animator>();
        textBox = GameObject.Find("Canvas/dialogue/content").GetComponent<TextMeshProUGUI>();
        nameBox = GameObject.Find("Canvas/dialogue/name").GetComponent<TextMeshProUGUI>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<movement>();
        eIndicator = player.transform.Find("indicator").gameObject;

        cameraFocusPoint = gameObject.AddComponent<CameraFocusPoint>();
        cameraFocusPoint.cameraSpeed = cameraSpeed;
        cameraFocusPoint.ortho = ortho;

        originalSize = transform.localScale.x;
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
            eIndicator.SetActive(false);
        }
    }

    void initiateTalk()
    {
        talking = true;
        currentText = "";
        animator.SetBool("speaking", true);
        nameBox.text = name;

        currentLine = 0;
        currentIndex = 0;

        playerMovement.allowMovement = false;

        if(setAnimatorTalking) {
            GetComponent<Animator>().SetBool("talking", true);
        }

        cameraFocusPoint.Focus(
            (transform.position + playerMovement.transform.position) / 2
        );

        setNewImage();
        StartCoroutine(finishAnimation());
    }

    void turnAround() 
    {
        if(!turnTowardsPlayer) return;
        if(talking) {
            float newSize = playerMovement.transform.position.x - transform.position.x > 0 ? originalSize : -originalSize;

            transform.localScale = Vector3.SmoothDamp(
                transform.localScale, 
                new Vector3(newSize, transform.localScale.y, transform.localScale.z), 
                ref turnVelocity,
                0.1f
            );
        } else {
            transform.localScale = Vector3.SmoothDamp(
                transform.localScale, 
                new Vector3(originalSize, transform.localScale.y, transform.localScale.z), 
                ref turnVelocity,
                0.1f
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(talking) textBox.text = currentText;

        if(playerInTrigger && !talking) eIndicator.SetActive(true);

        turnAround();

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(!talking)
            {
                if(playerInTrigger && playerMovement.allowMovement) {
                    initiateTalk();
                }
                return;
            }
        } else {
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
                // playerMovement.allowMovement = true;
                cameraFocusPoint.Unfocus();

                StartCoroutine(waitBeforeEnd(dialogue[dialogue.Length - 1].name.ToString()));

                if(setAnimatorTalking) {
                    GetComponent<Animator>().SetBool("talking", false);
                }
            } else 
            {
                disableCurrentImages();
                setNewImage();
                StartCoroutine(talk());
            }
        }
    }

    void disableCurrentImages()
    {
        GameObject parent = animator.transform.Find($"Image/{dialogue[currentLine - 1].name.ToString()}").gameObject;
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(false);
        }   
    }

    void setNewImage()
    {
        Message message = dialogue[currentLine];

        nameBox.text = SplitCamelCase(message.name.ToString());

        GameObject parent = animator.transform.Find($"Image/{message.name.ToString()}").gameObject;

        string searchingFor = message.emotion.ToString();

        Debug.Log(parent);

        if(parent.transform.Find(searchingFor) == null) {
            Debug.LogWarning("No image found for emotion: " + searchingFor + " in " + message.name.ToString() + "!");
            return;
        }

        parent.transform.Find(searchingFor).gameObject.SetActive(true);
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
    }

    IEnumerator waitBeforeEnd(string lastSpeaker)
    {

        yield return new WaitForSeconds(0.4f);
        playerMovement.allowMovement = true;
        GameObject parent = animator.transform.Find($"Image/{lastSpeaker}").gameObject;
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(false);
        }

    }

    void Reset()
    {
        trigger = GetComponent<BoxCollider2D>();
        trigger.isTrigger = true;
    }
}
