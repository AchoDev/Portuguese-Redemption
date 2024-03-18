using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class speaker : MonoBehaviour
{
    // Start is called before the first frame update

    BoxCollider2D trigger;

    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Reset()
    {
        trigger = GetComponent<BoxCollider2D>();
        trigger.isTrigger = true;
    }
}
