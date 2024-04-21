using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraFocusPoint)), RequireComponent(typeof(BoxCollider2D))]
public class CameraFocusPointTrigger : MonoBehaviour
{
    CameraFocusPoint cameraFocus;

    // Start is called before the first frame update
    void Start()
    {
        cameraFocus = GetComponent<CameraFocusPoint>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraFocus.Focus();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(cameraFocus != null) {
                cameraFocus.Unfocus();
            }
        }
    }
    
    void Reset() 
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
}
