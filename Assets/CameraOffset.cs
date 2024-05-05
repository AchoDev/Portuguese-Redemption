using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class CameraOffset : MonoBehaviour
{

    CinemachineVirtualCamera mainCamera;
    CinemachineVirtualCamera tempCamera;
    CinemachineFramingTransposer tempTransposer; 
    GameObject player;


    bool focused = false;

    [SerializeField] float offsetX;
    [SerializeField] float offsetY;

    [Header("Cam conginer settings")]
    [SerializeField] Collider2D boundingBox;
    [SerializeField, Range(0, 5)] float damping = 0.5f;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player") 
        {
            Focus();
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == "Player") 
        {
            Unfocus();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        mainCamera = GameObject.FindWithTag("CMVcam").GetComponent<CinemachineVirtualCamera>();
        player = GameObject.FindWithTag("Player");

        tempCamera = new GameObject("offset CMVcam")
            .AddComponent<CinemachineVirtualCamera>();
        tempCamera.gameObject.SetActive(false);
        tempCamera.tag = "TempCMVcam";

        tempTransposer = tempCamera.AddCinemachineComponent<CinemachineFramingTransposer>();

        if(boundingBox != null) 
        {
            CinemachineConfiner2D tempConfiner = tempCamera.gameObject.AddComponent<CinemachineConfiner2D>();
            tempConfiner.m_BoundingShape2D = boundingBox;
            tempConfiner.m_Damping = damping;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(player.activeSelf == false) {
            player = GameObject.FindWithTag("Player");
        }

        if(!CameraFocusPoint.allowFocus) {
            Unfocus();
        }

        if(focused) 
        {
            mainCamera.gameObject.SetActive(false);
            tempCamera.gameObject.SetActive(true);

            tempCamera.m_Follow = player.transform;
            tempCamera.m_Lens.OrthographicSize = mainCamera.m_Lens.OrthographicSize;
            tempCamera.m_Lens.NearClipPlane = mainCamera.m_Lens.NearClipPlane;
            

            // change offset of camera
            tempTransposer.m_TrackedObjectOffset = new Vector2(offsetX, offsetY);
        }
    }

    public void Focus() 
    {   
        focused = true;

        if(!mainCamera.gameObject.activeSelf) 
        {
            // fina all cams with temp tag
            GameObject[] tempCams = GameObject.FindGameObjectsWithTag("TempCMVcam");
            foreach(GameObject cam in tempCams) 
            {
                cam.SetActive(false);
            }
        }
    }

    public void Unfocus() 
    {
        focused = false;

        mainCamera.gameObject.SetActive(true);
        tempCamera.gameObject.SetActive(false);
    }

    void Reset()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }
}
