using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

enum People 
{  
    LeoSuit,
    BickNuterus,
    SemranEmadi,
    AA,
}

enum Emotion {
    neutral,
    annoyed,
    sad,
    angry,
    happy,
    embarrassed,
    funny,
    sus,
    kawaii
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
    [SerializeField] GameObject initiator = null;
    [SerializeField] bool turnTowardsPlayer = false;
    [SerializeField] bool setAnimatorTalking = false;
    [SerializeField, Range(0, 0.1f)] float timeBetweenLetters = 0.025f;
    [SerializeField] UnityEvent onDialogueFinish;
    
    [Space(10)]
    [Header("Camera")]
    [SerializeField] bool focusCamera = true;
    [SerializeField, Range(0.1f, 5f)] float cameraSpeed = 1f;
    [SerializeField, Range(0.1f, 5)] float ortho = 2.5f;

    [Space(10)]
    [Header("Dialogue")]
    [Tooltip(@"
        <wait [milliseconds]> wait for milliseconds
        <skip> skip to end of line
        <next> go to next line
        <play [name]> play sound
        <speed [milliseconds]> change text speed
    ")]
    [SerializeField] Message[] dialogue;


    BoxCollider2D trigger;
    CameraFocusPoint cameraFocusPoint;

    TextMeshProUGUI textBox;
    TextMeshProUGUI nameBox;
    Animator canvasAnimator;
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
        canvasAnimator = GameObject.Find("Canvas/dialogue").GetComponent<Animator>();
        textBox = GameObject.Find("Canvas/dialogue/content").GetComponent<TextMeshProUGUI>();
        nameBox = GameObject.Find("Canvas/dialogue/name").GetComponent<TextMeshProUGUI>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<movement>();
        eIndicator = player.transform.Find("indicator").gameObject;

        cameraFocusPoint = gameObject.AddComponent<CameraFocusPoint>();
        cameraFocusPoint.cameraSpeed = cameraSpeed;
        cameraFocusPoint.ortho = ortho;

        if(initiator == null) initiator = gameObject;
        
        originalSize = initiator.transform.localScale.x;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInTrigger = false;
            eIndicator.SetActive(false);
        }
    }

    void initiateTalk()
    {
        talking = true;
        currentText = "";
        canvasAnimator.SetBool("speaking", true);
        nameBox.text = name;

        currentLine = 0;
        currentIndex = 0;

        playerMovement.allowMovement = false;

        if(setAnimatorTalking) {
            initiator.GetComponent<Animator>().SetBool("talking", true);
            // animator.SetBool("talking", true);
        }


        cameraFocusPoint.Focus(
            (transform.position + playerMovement.transform.position) / 2
        );

        setNewImage();
        StartCoroutine(finishAnimation());
    }

    void turnAround() 
    {
        Transform t = initiator.transform;
        if(talking) {
            float newSize = playerMovement.transform.position.x - t.position.x > 0 ? originalSize : -originalSize;

            initiator.transform.localScale = Vector3.SmoothDamp(
                t.localScale,
                new Vector3(newSize, t.localScale.y, t.localScale.z), 
                ref turnVelocity,
                0.1f
            );
        } else {
            initiator.transform.localScale = Vector3.SmoothDamp(
                t.localScale, 
                new Vector3(originalSize, t.localScale.y, t.localScale.z), 
                ref turnVelocity,
                0.1f
            );
        }
    }

    public void speak()
    {
        initiateTalk();
    }

    // Update is called once per frame
    void Update()
    {
        if(talking) textBox.text = currentText;

        if(playerInTrigger && !talking) eIndicator.SetActive(true);

        if(turnTowardsPlayer) turnAround();

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
                canvasAnimator.SetBool("speaking", false);
                // playerMovement.allowMovement = true;
                cameraFocusPoint.Unfocus();

                StartCoroutine(waitBeforeEnd(dialogue[dialogue.Length - 1].name.ToString()));

                if(onDialogueFinish != null) onDialogueFinish.Invoke();

                if(setAnimatorTalking) {
                    initiator.GetComponent<Animator>().SetBool("talking", false);
                    // animator.SetBool("talking", false);
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
        GameObject parent = canvasAnimator.transform.Find($"Image/{dialogue[currentLine - 1].name.ToString()}").gameObject;
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(false);
        }   
    }

    void setNewImage()
    {
        Message message = dialogue[currentLine];

        nameBox.text = SplitCamelCase(message.name.ToString());

        GameObject parent = canvasAnimator.transform.Find($"Image/{message.name.ToString()}").gameObject;

        string searchingFor = message.emotion.ToString();

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
        GameObject parent = canvasAnimator.transform.Find($"Image/{lastSpeaker}").gameObject;
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
