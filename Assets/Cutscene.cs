using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class CutsceneStep
{
    float duration;
    public bool folded;

    abstract public void act();
}

[System.Serializable]
public class CameraFocusPointStep : CutsceneStep
{
    public Transform focusPoint;
    public CameraFocusPoint cameraFocusPoint;

    public override void act()
    {
        cameraFocusPoint.Focus(focusPoint.position);
    }
}


public class Cutscene : MonoBehaviour
{

    [SerializeField] List<CutsceneStep> steps = new List<CutsceneStep>();

    public void AddStep(CutsceneStep cutsceneStep)
    {
        steps.Add(cutsceneStep);
    }

    public List<CutsceneStep> GetSteps()
    {
        return steps;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
