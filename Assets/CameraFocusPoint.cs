using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFocusPoint : MonoBehaviour
{
    CinemachineBrain brain;
    CinemachineVirtualCamera mainCamera;
    CinemachineVirtualCamera tempCamera;

    [SerializeField] bool moveCamera = false;
    [SerializeField] float movementAmount = 1f;
    Vector3 movementVelocity = Vector3.zero;

    bool focused = false;

    GameObject player;

    public Transform focusPoint;
    Vector3 focusPosition;
    [Range(0.1f, 5)] public float cameraSpeed = 1f;
    [Range(0.1f, 5)]public float ortho = 2.5f;
    public CinemachineBlendDefinition.Style transitionType = CinemachineBlendDefinition.Style.EaseInOut;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        brain = GameObject.FindWithTag("MainCamera").GetComponent<CinemachineBrain>();
        mainCamera = GameObject.FindGameObjectWithTag("CMVcam").GetComponent<CinemachineVirtualCamera>();
        
        tempCamera = new GameObject("focus-point CMVcam")
            .AddComponent<CinemachineVirtualCamera>();
        tempCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        tempCamera.transform.position = focusPosition;
        tempCamera.transform.position = new Vector3(
            tempCamera.transform.position.x, 
            tempCamera.transform.position.y, 
            mainCamera.transform.position.z
        );

        tempCamera.m_Lens.OrthographicSize = ortho;
    
        if(moveCamera && focused) 
        {
            Vector3 target = focusPoint.transform.position;
            target.x -= (target.x / player.transform.position.x - 1) * movementAmount;

            Debug.Log(target);

            focusPosition = Vector3.SmoothDamp(tempCamera.transform.position, target, ref movementVelocity, 0.55f);
        }
    }

    public void Focus() 
    {
        Focus(focusPoint.position);
    }

    public void Focus(Vector3 position) 
    {
        focusPosition = position;
        focused = true;

        brain.m_DefaultBlend.m_Time = cameraSpeed;
        brain.m_DefaultBlend.m_Style = transitionType;

        tempCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
    }

    public void Unfocus() 
    {
        focused = false;
        tempCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }
}
