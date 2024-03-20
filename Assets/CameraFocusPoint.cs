using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFocusPoint : MonoBehaviour
{
    CinemachineBrain brain;
    CinemachineVirtualCamera mainCamera;
    CinemachineVirtualCamera tempCamera;

    public Transform focusPoint;
    [Range(0.1f, 5)] public float cameraSpeed = 1f;
    [Range(0, 5)]public float ortho = 2.5f;
    public CinemachineBlendDefinition.Style transitionType = CinemachineBlendDefinition.Style.EaseInOut;

    // Start is called before the first frame update
    void Start()
    {
        brain = GameObject.FindWithTag("MainCamera").GetComponent<CinemachineBrain>();
        mainCamera = GameObject.FindGameObjectWithTag("CMVcam").GetComponent<CinemachineVirtualCamera>();
        
        tempCamera = new GameObject("focus-point CMVcam")
            .AddComponent<CinemachineVirtualCamera>();
        tempCamera.gameObject.SetActive(false);
    }

    public void Focus() 
    {
        Focus(focusPoint.position);
    }

    public void Focus(Vector3 position) 
    {
        tempCamera.transform.position = position;
        tempCamera.transform.position = new Vector3(
            tempCamera.transform.position.x, 
            tempCamera.transform.position.y, 
            mainCamera.transform.position.z
        );

        tempCamera.m_Lens.OrthographicSize = ortho;
        brain.m_DefaultBlend.m_Time = cameraSpeed;
        brain.m_DefaultBlend.m_Style = transitionType;

        tempCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
    }

    public void Unfocus() 
    {
        tempCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }
}
