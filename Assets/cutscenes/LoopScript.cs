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

    public void Check()
    {
        if(director == null) director = GetComponent<PlayableDirector>();
        if(loopRange == null) return;
        if (looping)
        {
            float currentTime = (float)director.time + (2 / 60f);
            // Debug.Log(loopRange.Item2);
            // Debug.Log(currentTime);
            // Debug.Log(50 / 60);
            if (currentTime >= loopRange.Item2)
            {
                director.time = loopRange.Item1;
            }
        }
    }

    public void StartLoop(float start, float end)
    {
        looping = true;

        start += (float)director.time;
        end += (float)director.time;

        loopRange = new Tuple<float, float>(start, end);
    }
}
