

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{

    int selectedStep = 0;
    string[] stepOptions = new string[] {"CameraFocusPoint", "CameraMove", "SetAnimatorBool", "SetAnimatorAndMove", "StartDialogue"};

    public override void OnInspectorGUI()
    {
        Cutscene cutscene = (Cutscene)target;
        SerializedObject serializedObject = new SerializedObject(cutscene);
        DrawDefaultInspector();

        SerializedProperty steps = serializedObject.FindProperty("steps");
        for(int i = 0; i < steps.arraySize; i++)
        {
            CutsceneStep step = steps.GetArrayElementAtIndex(i).managedReferenceValue as CutsceneStep;

            switch(step.GetType().ToString())
            {
                case "CameraFocusPointStep":
                    CameraFocusPointStep cameraFocusPointStep = (CameraFocusPointStep)step;
                    GUILayout.BeginVertical($"Camera Focus Point Step {i++}", "window");
                    cameraFocusPointStep.focusPoint = EditorGUILayout.ObjectField("Focus point", cameraFocusPointStep.focusPoint, typeof(Transform), true) as Transform;
                    cameraFocusPointStep.cameraFocusPoint = EditorGUILayout.ObjectField("Camera focus point", cameraFocusPointStep.cameraFocusPoint, typeof(CameraFocusPoint), true) as CameraFocusPoint;
                    GUILayout.EndVertical();
                    break;
                // case "CameraMoveStep":
                //     // CameraMoveStep cameraMoveStep = (CameraMoveStep)step;
                //     break;
                // case "SetAnimatorBoolStep":
                //     // SetAnimatorBoolStep setAnimatorBoolStep = (SetAnimatorBoolStep)step;
                //     break;
                // case "SetAnimatorAndMoveStep":
                //     // SetAnimatorAndMoveStep setAnimatorAndMoveStep = (SetAnimatorAndMoveStep)step;
                //     break;
                // case "StartDialogueStep":
                //     // StartDialogueStep startDialogueStep = (StartDialogueStep)step;
                //     break;
                default:
                    GUILayout.BeginVertical("Error", "window");
                    GUILayout.Label($"Step called '{step.GetType().ToString()}' does not have defined editor layout :(");
                    break;
            }
        }

        selectedStep = EditorGUILayout.Popup("Select step", selectedStep, stepOptions);
        if(GUILayout.Button("Add step"))
        {
            switch(selectedStep)
            {
                case 0:
                    cutscene.AddStep(new CameraFocusPointStep());
                    break;
                case 1:
                    // cutscene.AddStep<CameraMoveStep>();
                    break;
                case 2:
                    // cutscene.AddStep<SetAnimatorBoolStep>();
                    break;
                case 3:
                    // cutscene.AddStep<SetAnimatorAndMoveStep>();
                    break;
                case 4:
                    // cutscene.AddStep<StartDialogueStep>();
                    break;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}      