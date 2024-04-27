using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LoopScript : MonoBehaviour
{
    PlayableDirector director;
    bool looping = false;
    Tuple<float, float> loopRange;

    public void Check(float dt)
    {
        if(director == null) director = GetComponent<PlayableDirector>();
        if(loopRange == null) return;

        if(LoopBehaviour.leave) StopLoop();

        if (looping)
        {
            float currentTime = (float)director.time + (dt * 5);
            // Debug.Log(loopRange.Item2);
            // Debug.Log(currentTime);
            if (currentTime >= loopRange.Item2)
            {
                director.time = loopRange.Item1;
            }
        }
    }

    public void StartLoop(float start, float end)
    {
        if(director == null) director = GetComponent<PlayableDirector>();
        looping = true;

        start += (float)director.time;
        end += (float)director.time;

        loopRange = new Tuple<float, float>(start, end);
    }

    public void StopLoop()
    {
        Debug.Log("Stop loop");
        looping = false;
        LoopBehaviour.leave = false;
    }
}
