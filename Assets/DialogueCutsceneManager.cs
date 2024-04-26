using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueCutsceneManager : MonoBehaviour
{
    speaker[] dialogues;


    void Start()
    {
        dialogues = GetComponentsInChildren<speaker>();


        Debug.Log(dialogues);
        Debug.Log(dialogues[0]);
        Debug.Log("fjdklasöfsdjkalöfdjasklöfdjkslöajdfsklöafjsdklöa");
    }


    public void StartDialogue(int index)
    {
        Debug.Log(dialogues.Length);
        Debug.Log(index);
        dialogues[index].speak();
    }

    void Reset() 
    {
        dialogues = GetComponentsInChildren<speaker>();
        Debug.Log("Reset dialogue cutscene manager");
    }
}