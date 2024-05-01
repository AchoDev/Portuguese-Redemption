using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour
{
    [SerializeField] UnityEvent onInteracted;

    bool isInteractable = false;
    GameObject indicator;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            isInteractable = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            isInteractable = false;
            indicator.SetActive(false);
        }
    }
    
    void Start()
    {
        indicator = GameObject.FindWithTag("Player").transform.Find("indicator").gameObject;
    }

    void Update()
    {
        if(!isInteractable) return;
        indicator.SetActive(true);

        if(Input.GetKeyDown("e"))
        {
            onInteracted.Invoke();
        }
    }

    void Reset()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }

    // void OnDrawGizmos()
    // {
    //     BoxCollider2D collider = GetComponent<BoxCollider2D>();
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(transform.position, collider.size);
    // }
}
