using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class CameraFocusPoint : MonoBehaviour
{
    [SerializeField]
    public static bool allowFocus = true;

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

    // public void ForceStart() {
    //     Start();
    // }

    // Start is called before the first frame update
    void Awake()
    {

        player = GameObject.FindWithTag("Player");

        brain = GameObject.FindWithTag("MainCamera").GetComponent<CinemachineBrain>();
        mainCamera = GameObject.FindGameObjectWithTag("CMVcam").GetComponent<CinemachineVirtualCamera>();
        
        tempCamera = new GameObject("focus-point CMVcam")
            .AddComponent<CinemachineVirtualCamera>();
        tempCamera.gameObject.SetActive(false);
        tempCamera.tag = "TempCMVcam";
    }

    void Update()
    {
        if(player != null && player.activeSelf == false) {
            player = GameObject.FindWithTag("Player");
        }

        if(!allowFocus) {
            Unfocus();
        }

        if(focused) 
        {
            tempCamera.transform.position = focusPosition;
            tempCamera.transform.position = new Vector3(
                tempCamera.transform.position.x, 
                tempCamera.transform.position.y, 
                mainCamera.transform.position.z
            );

            tempCamera.m_Lens.OrthographicSize = ortho;
        
            mainCamera.gameObject.SetActive(false);
            if(moveCamera) 
            {
                Vector3 target = focusPoint.transform.position;
                target.x -= (target.x / player.transform.position.x - 1) * movementAmount;

                // Debug.Log(target);

                focusPosition = Vector3.SmoothDamp(tempCamera.transform.position, target, ref movementVelocity, 0.55f);
            } else {
                if(focusPoint != null) {
                    focusPosition = focusPoint.position;
                }
            }
        }

    }

    public void Focus() 
    {
        Focus(focusPoint.position);
    }

    public void Focus(Vector3 position) 
    {

        if(!allowFocus) return;

        focusPosition = position;
        focused = true;

        brain.m_DefaultBlend.m_Time = cameraSpeed;
        brain.m_DefaultBlend.m_Style = transitionType;


        // Debug.Log(mainCamera);
        if(!mainCamera.gameObject.activeSelf) 
        {
            // fina all cams with temp tag
            GameObject[] tempCams = GameObject.FindGameObjectsWithTag("TempCMVcam");
            foreach(GameObject cam in tempCams) 
            {
                cam.SetActive(false);
            }
        }

        tempCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
    }

    public void Unfocus() 
    {
        focused = false;
        if(tempCamera == null) return;
        tempCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }

    public void disableFocus() 
    {
        allowFocus = false;
    }

    public void enableFocus() 
    {
        allowFocus = true;
    }
}

[CustomEditor(typeof(CameraFocusPoint))]
public class CameraFocusPointEditor : Editor
{
    CameraFocusPoint focusPoint;

    public override void OnInspectorGUI()
    {

        if(!CameraFocusPoint.allowFocus) 
        {
            EditorGUILayout.HelpBox("Focus is disabled", MessageType.Warning);
        }

        DrawDefaultInspector();

        focusPoint = (CameraFocusPoint)target;

        // GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Focus")) 
        {
            focusPoint.Focus();
        }

        if(GUILayout.Button("Unfocus")) 
        {
            focusPoint.Unfocus();
        }

        GUILayout.EndHorizontal();

        if(!CameraFocusPoint.allowFocus) 
        {
            if(GUILayout.Button("Allow focus")) 
            {
                focusPoint.enableFocus();
            }
        } else {
            if(GUILayout.Button("Disallow focus completely")) 
            {
                focusPoint.disableFocus();
            }
        }
    }
}
