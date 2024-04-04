using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnterableDoor))]
public class EnterableDoorScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnterableDoor myScript = (EnterableDoor)target;
        if(GUILayout.Button("Create scripts for target"))
        {
            GameObject dest = myScript.destination.gameObject;
            EnterableDoor destDoor = dest.AddComponent<EnterableDoor>();
            destDoor.destination = myScript.gameObject.transform;
        }
    }
}
