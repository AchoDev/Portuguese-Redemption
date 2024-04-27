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
    }


    public void StartDialogue(int index)
    {
        if(dialogues == null || dialogues[index] == null) return;
        dialogues[index].speak();
    }

    void Reset() 
    {
        dialogues = GetComponentsInChildren<speaker>();
        Debug.Log("Reset dialogue cutscene manager");
    }
}