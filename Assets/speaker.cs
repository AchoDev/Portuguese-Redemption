using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

enum People 
{  
    LeoSuit,
    BickNuterus,
    SemranEmadi,
    AA,
    MohawkMan,
    LeonVladimirovich,
    LeoTshirt,
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
    [SerializeField, Tooltip("Sets animation parameter 'talking' to true")] bool setAnimatorTalking = false;
    [SerializeField, Range(0, 100f)] float timeBetweenLetters = 25;
    float timeBetweenLettersOriginal;
    [SerializeField] UnityEvent onDialogueFinish;

    [Space(10)]
    [Header("Camera")]
    [SerializeField] bool focusCamera = true;
    [SerializeField, Range(0.1f, 5f)] float cameraSpeed = 1f;
    [SerializeField, Range(0.1f, 5)] float ortho = 2.5f;

    public bool talkByInteracting = true;

    [Space(10)]
    [Header("Dialogue")]
    [Tooltip(@"
        <wait [milliseconds]> wait for milliseconds
        <skip> skip to end of line
        <next> go to next line
        <play [name]> play sound
        <speed [milliseconds]> change text speed
        <rspeed> reset text speed to original
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
    List<int[]> commands = new List<int[]>();

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
        timeBetweenLettersOriginal = timeBetweenLetters;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!talkByInteracting) return;
        if(other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(!talkByInteracting) return;
        if(other.CompareTag("Player"))
        {
            playerInTrigger = false;
            eIndicator.SetActive(false);
        }
    }

    public void initiateTalk()
    {
        talking = true;
        canvasAnimator.SetBool("speaking", true);
        nameBox.text = name;

        currentLine = 0;
        currentIndex = 0;

        playerMovement.allowMovement = false;

        if(setAnimatorTalking) {
            initiator.GetComponent<Animator>().SetBool("talking", true);
            // animator.SetBool("talking", true);
        }

        if(focusCamera) 
        {
            cameraFocusPoint.Focus(
                (transform.position + playerMovement.transform.position) / 2
            );
        }

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
        if(talking)  {

            string formattedText = currentText;
            int buffer = 0;

            foreach(int[] command in commands)
            {
                try {
                    formattedText = formattedText.Remove(command[0] - buffer, command[1] - command[0]);
                    buffer += command[1] - command[0];
                } catch (Exception e) {
                    Debug.LogWarning("Error removing command: " + e.Message);
                }
            }

            textBox.text = formattedText;
        }

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
            currentText += text.Substring(currentIndex);
            currentIndex = text.Length - 1;
        } else // go to next line
        {
            currentLine++;
            if(currentLine >= dialogue.Length) // stop dialogue after last line
            {
                talking = false;
                currentLine = 0;
                canvasAnimator.SetBool("speaking", false);
                // playerMovement.allowMovement = true;
                cameraFocusPoint.Unfocus();

                StartCoroutine(waitBeforeEnd(dialogue[dialogue.Length - 1].name.ToString()));

                if(onDialogueFinish != null) onDialogueFinish.Invoke();

                if(setAnimatorTalking) {
                    initiator.GetComponent<Animator>().SetBool("talking", false);
                    // animator.SetBool("talking", false);
                }

                timeBetweenLetters = timeBetweenLettersOriginal;
            } else 
            {
                startNextLine();
            }
        }
    }

    void startNextLine()
    {
        commands.Clear();
        disableCurrentImages();
        setNewImage();
        StartCoroutine(talk());
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
            if(dialogue[currentLine].dialogue[currentIndex] == '<')
            {
                int startIndex = currentIndex;
                string command = "";
                Func<char> currentCommandChar = () => dialogue[currentLine].dialogue[currentIndex];
                currentIndex++;

                while(currentCommandChar() != '>')
                {
                    command += currentCommandChar();
                    currentIndex++;
                }

                commands.Add(new int[] {startIndex, currentIndex + 1});
                yield return StartCoroutine(evaluateCommand(command));
            }

            currentText = dialogue[currentLine].dialogue.Substring(0, currentIndex + 1);
            currentIndex++;
            yield return new WaitForSeconds(timeBetweenLetters / 1000f);
        }
    }

    IEnumerator evaluateCommand(string command) 
    {
        string[] parts = command.Split(' ');
        switch(parts[0])
        {
            case "wait":
                int milliseconds = int.Parse(parts[1]);
                yield return new WaitForSeconds(milliseconds / 1000f);
                break;
            case "skip":
                currentIndex = dialogue[currentLine].dialogue.Length - 1;
                break;
            case "next":
                currentLine++;
                startNextLine();
                break;
            case "play":
                break;
            case "speed":
                timeBetweenLetters = float.Parse(parts[1]);
                break;
            case "rspeed":
                timeBetweenLetters = timeBetweenLettersOriginal;
                break;
            case "image":
                string currentDialogue = dialogue[currentLine].dialogue;
                dialogue[currentLine].dialogue = currentDialogue.Insert(currentIndex + 1, $"<sprite=0>");
                currentIndex += 10;
                break;
            default:
                Debug.LogWarning("Unknown command: " + parts[0]);
                break;
        }

        yield return null;
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
