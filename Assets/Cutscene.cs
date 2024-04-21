using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[Serializable]
public class CutsceneStep
{

    public float duration;
    public bool isBlocking = true;
    public CutsceneStepType type;

    // CameraFocusPoint
    CameraFocusPoint cameraFocusPoint;
    public Transform focusPoint;
    public float cameraSpeed = 1;
    public float ortho = 10;

    // SetAnimatorBool
    public Animator animator;
    public string triggerName;

    // SetGameobjectActive
    public GameObject setActiveTarget;
    public bool active = true;

    // MoveGameobject
    public Vector3 originalPosition;
    public Transform moveTarget;
    public Vector3 moveTargetDelta;
    public float speed = 1;

    // StartDialogue
    public speaker dialogue;


    public CutsceneStep(CutsceneStepType type)
    {
        this.type = type;
    }

    public void SetCameraFocusPoint(CameraFocusPoint target) {
        cameraFocusPoint = target;
    }

    public IEnumerator act() {
        switch(type) {
            case CutsceneStepType.CameraFocusPoint:

                cameraFocusPoint.cameraSpeed = cameraSpeed;
                cameraFocusPoint.ortho = ortho;
                cameraFocusPoint.focusPoint = focusPoint;
                cameraFocusPoint.Focus();

                if(isBlocking) {
                    yield return new WaitForSeconds(cameraSpeed / 1000);
                } else {
                    yield return new WaitForSeconds(duration / 1000);
                }

                break;
            case CutsceneStepType.SetAnimatorTrigger:
                animator.SetTrigger(triggerName );
                break;
            case CutsceneStepType.SetGameobjectActive:
                setActiveTarget.SetActive(active);
                break;
            case CutsceneStepType.MoveGameobject:
                originalPosition = new Vector3(moveTarget.position.x, moveTarget.position.y, moveTarget.position.z);
                while(Vector3.Distance(moveTarget.position, originalPosition + moveTargetDelta) > 0.1f) {
                    moveTarget.position = Vector3.MoveTowards(moveTarget.position, originalPosition + moveTargetDelta, speed * Time.deltaTime);
                    yield return null;
                }
                break;
            case CutsceneStepType.Wait:
                yield return new WaitForSeconds(duration / 1000);
                break;
            case CutsceneStepType.StartDialogue:
                dialogue.talkByInteracting = false;
                dialogue.initiateTalk();
                break;
        }

        yield return null;
    }

    public string DebugInformation() {
        switch(type) {
            case CutsceneStepType.CameraFocusPoint:
                return @$"
                Focus camera on '{focusPoint.name}'";
                
            case CutsceneStepType.SetAnimatorTrigger:
                return $"Set animator trigger '{triggerName}' on {animator.name}";
            case CutsceneStepType.SetGameobjectActive:
                return $"Set {setActiveTarget.name} active to {active}";
            case CutsceneStepType.MoveGameobject:
                return  @$"
                Move {moveTarget.name} by {moveTargetDelta} at speed {speed}
                Current position: {moveTarget.position}
                Target position: {originalPosition + moveTargetDelta}
                Remaning Distance: {Vector3.Distance(moveTarget.position, originalPosition + moveTargetDelta)}";
            default:
                return $"No debug information available for step type '{type}' :(";
        }
    }
}

[Serializable]
public enum CutsceneStepType
{
    CameraFocusPoint,
    SetAnimatorTrigger,
    StartDialogue,
    SetGameobjectActive,
    MoveGameobject,
    Wait,
}


public class Cutscene : MonoBehaviour
{
    
    [SerializeField] List<CutsceneStep> steps = new List<CutsceneStep>();
    [HideInInspector] public bool playing;
    [HideInInspector] public int currentIndex;
    [HideInInspector] public CutsceneStep currentStep;


    CameraFocusPoint cameraFocusPoint;

    public void AddStep(CutsceneStepType type)
    {
        CutsceneStep step = new CutsceneStep(type);
        step.SetCameraFocusPoint(cameraFocusPoint);
        steps.Add(step);
    }

    public void Play()
    {
        if(playing) return;
        SetSteps();
        StartCoroutine(PlayCoroutine());
        playing = true;
    }

    IEnumerator PlayCoroutine()
    {
        foreach(CutsceneStep step in steps)
        {
            currentStep = step;
            yield return StartCoroutine(step.act());
            currentIndex++;
        }
    }

    void Start()
    {
        
        // cameraFocusPoint.ForceStart();
    }

    public void ForceReset() {
        Reset();
    }

    void Reset() {
        // create chidl obj
        if(GameObject.Find("Scripts") != null) {
            DestroyImmediate(GameObject.Find("Scripts"));
        }

        GameObject obj = new GameObject("Scripts");
        cameraFocusPoint = obj.AddComponent<CameraFocusPoint>();
        obj.transform.parent = transform;
    }

    void SetSteps() {
        cameraFocusPoint = GetComponentInChildren<CameraFocusPoint>();
        foreach(CutsceneStep step in steps) {
            Debug.Log(cameraFocusPoint);
            Debug.Log("THIS IS CAMERA FOCUS POITN");
            step.SetCameraFocusPoint(cameraFocusPoint);
        }
    }
}

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{

    CutsceneStepType selectedStep = CutsceneStepType.CameraFocusPoint;

    
    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();

        Cutscene cutscene = (Cutscene)target;

        if(GUILayout.Button("Reset"))
        {
            cutscene.ForceReset();
        }

        SerializedProperty steps = serializedObject.FindProperty("steps");

        EditorGUILayout.LabelField("Cutscene steps", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Number of steps: {steps.arraySize}");

        for(int i = 0; i < steps.arraySize; i++)
        {
            SerializedProperty currentStep = steps.GetArrayElementAtIndex(i);
            CutsceneStepType stepType = (CutsceneStepType)currentStep.FindPropertyRelative("type").enumValueIndex;

            GUILayout.BeginVertical($"{stepType.ToString()} (Step {i})", "window");
            
            SerializedProperty isBlocking = currentStep.FindPropertyRelative("isBlocking");
            isBlocking.boolValue = EditorGUILayout.Toggle("Is blocking", isBlocking.boolValue);

            if(!isBlocking.boolValue)
            {
                SerializedProperty duration = currentStep.FindPropertyRelative("duration");
                duration.floatValue = EditorGUILayout.Slider("Duration (ms)", currentStep.FindPropertyRelative("duration").floatValue, 0, 5000);
            }

            switch(stepType)
            {
                case CutsceneStepType.CameraFocusPoint:
                    SerializedProperty focusPoint = currentStep.FindPropertyRelative("focusPoint");
                    SerializedProperty ortho = currentStep.FindPropertyRelative("ortho");
                    SerializedProperty cameraSpeed = currentStep.FindPropertyRelative("cameraSpeed");
                    focusPoint.boxedValue = EditorGUILayout.ObjectField("Focus point", (Transform)focusPoint.boxedValue, typeof(Transform), true) as Transform;
                    ortho.floatValue = EditorGUILayout.FloatField("Ortho", ortho.floatValue);
                    cameraSpeed.floatValue = EditorGUILayout.FloatField("Camera speed", cameraSpeed.floatValue);
                    
                    break;
                // case "CameraMoveStep":
                //     // CameraMoveStep cameraMoveStep = (CameraMoveStep)step;
                //     break;
                case CutsceneStepType.SetAnimatorTrigger:
                    SerializedProperty animator = currentStep.FindPropertyRelative("animator");
                    SerializedProperty triggerName = currentStep.FindPropertyRelative("triggerName");
                    animator.objectReferenceValue = EditorGUILayout.ObjectField("Animator", animator.objectReferenceValue, typeof(Animator), true) as Animator;
                    triggerName.stringValue = EditorGUILayout.TextField("Trigger name", triggerName.stringValue);
                    break;
                // case "SetAnimatorAndMoveStep":
                //     // SetAnimatorAndMoveStep setAnimatorAndMoveStep = (SetAnimatorAndMoveStep)step;
                //     break;
                // case "StartDialogueStep":
                //     // StartDialogueStep startDialogueStep = (StartDialogueStep)step;
                //     break;

                case CutsceneStepType.SetGameobjectActive:
                    SerializedProperty target = currentStep.FindPropertyRelative("setActiveTarget");
                    SerializedProperty active = currentStep.FindPropertyRelative("active");
                    target.objectReferenceValue = EditorGUILayout.ObjectField("Target", target.objectReferenceValue, typeof(GameObject), true) as GameObject;
                    active.boolValue = EditorGUILayout.Toggle("Active", active.boolValue);
                    break;

                case CutsceneStepType.MoveGameobject:
                    SerializedProperty moveTarget = currentStep.FindPropertyRelative("moveTarget");
                    SerializedProperty moveTargetDelta = currentStep.FindPropertyRelative("moveTargetDelta");
                    SerializedProperty speed = currentStep.FindPropertyRelative("speed");
                    moveTarget.objectReferenceValue = EditorGUILayout.ObjectField("Move target", moveTarget.objectReferenceValue, typeof(Transform), true) as Transform;
                    moveTargetDelta.vector3Value = EditorGUILayout.Vector3Field("Move target delta", moveTargetDelta.vector3Value);
                    speed.floatValue = EditorGUILayout.FloatField("Speed", speed.floatValue);
                    break;

                default:
                    // GUILayout.BeginVertical("window");
                    GUILayout.Label($"Step called '{stepType}' does not have defined editor layout :(");
                    break;
            }


            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Move up"))
            {
                steps.MoveArrayElement(i, i - 1);
            }
            if(GUILayout.Button("Move down"))
            {
                steps.MoveArrayElement(i, i + 1);
            }
            GUILayout.EndHorizontal();

            if(GUILayout.Button("Remove"))
            {
                steps.DeleteArrayElementAtIndex(i);
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);
        }

        selectedStep = (CutsceneStepType)EditorGUILayout.EnumPopup("Select step", selectedStep);
        if(GUILayout.Button("Add step"))
        {
            cutscene.AddStep(selectedStep);
        }

        GUILayout.Space(10);
        if(GUILayout.Button("Play"))
        {
            cutscene.Play();
        }

        if(cutscene.playing) {
            GUILayout.Label("Playing...");
            GUILayout.Label($"Current index: {cutscene.currentIndex}");
            GUILayout.Label($"Current step: {cutscene.currentStep.type}");
            GUILayout.Label($"{cutscene.currentStep.DebugInformation()}");
        }

        serializedObject.ApplyModifiedProperties();
    }
}      