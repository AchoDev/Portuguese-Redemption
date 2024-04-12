using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCFightController))]
public class NPCControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
     
        NPCFightController myScript = (NPCFightController)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Probabilities", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Idle: ", ("0% - " + myScript.idleProbability * 100 + "%"));
        EditorGUILayout.LabelField("Attack: ", ("0% - " + myScript.attackProbability * 100 + "%"));
        EditorGUILayout.LabelField("Defend: ", ("0% - " + myScript.defendProbability * 100 + "%"));

        EditorGUILayout.LabelField("Unusued Probability: ", Mathf.Round((1 - myScript.idleProbability - myScript.attackProbability - myScript.defendProbability) * 10000) / 100 + "%");

        myScript.idleProbability = EditorGUILayout.Slider("Idle Probability", myScript.idleProbability, 0, 1 - myScript.attackProbability - myScript.defendProbability);
        myScript.attackProbability = EditorGUILayout.Slider("Attack Probability", myScript.attackProbability, 0, 1 - myScript.idleProbability - myScript.defendProbability);
        myScript.defendProbability = EditorGUILayout.Slider("Defend Probability", myScript.defendProbability, 0, 1 - myScript.attackProbability - myScript.idleProbability);

    }
}
