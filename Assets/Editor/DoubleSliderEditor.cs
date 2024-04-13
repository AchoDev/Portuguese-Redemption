using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoubleSlider))]
public class DoubleSliderEditor : Editor
{

    bool show = true;
    int value = 90;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        GUILayout.Space(20);
        show = EditorGUILayout.Foldout(show, "Debugging tools");

        if(!show) {
            return;
        }

        value = EditorGUILayout.IntSlider(value, 0, 100);

        if(GUILayout.Button("Move to value")) {
            DoubleSlider myScript = (DoubleSlider)target;
            myScript.value = value;
        }
    }
}
