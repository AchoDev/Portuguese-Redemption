using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider2D))]
public class OnTriggerEnter : MonoBehaviour
{

    [SerializeField] string tag = "Player";

    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == tag)
        {
            onTriggerEnter.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == tag)
        {
            onTriggerExit.Invoke();
        }
    }

    void Reset() {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
}
