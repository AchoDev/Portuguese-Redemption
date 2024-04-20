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
    public float cameraSpeed;
    public float ortho;

    // SetAnimatorBool
    public Animator animator;
    public string triggerName;

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
                cameraFocusPoint.Focus(focusPoint.position);
                break;
            case CutsceneStepType.SetAnimatorTrigger:
                animator.SetTrigger(triggerName );
                break;
        }

        if(!isBlocking) {
            
        }
        yield return null;

    }
}

[Serializable]
public enum CutsceneStepType
{
    CameraFocusPoint,
    CameraMove,
    SetAnimatorTrigger,
    SetAnimatorAndMove,
    StartDialogue
}


public class Cutscene : MonoBehaviour
{
    
    [SerializeField] List<CutsceneStep> steps = new List<CutsceneStep>();

    public void AddStep(CutsceneStepType type)
    {
        steps.Add(new CutsceneStep(type));
    }

    public void Play()
    {
        StartCoroutine(PlayCoroutine());
    }

    IEnumerator PlayCoroutine()
    {
        foreach(CutsceneStep step in steps)
        {
            yield return StartCoroutine(step.act());
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
                default:
                    GUILayout.BeginVertical("window");
                    GUILayout.Label($"Step called '{currentStep}' does not have defined editor layout :(");
                    break;
            }

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

        serializedObject.ApplyModifiedProperties();
    }
}      