using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class OnTimelineFinish : MonoBehaviour
{
    PlayableDirector director;

    [SerializeField] UnityEvent onTimelineFinish;

    void Start()
    {
        director = GetComponent<PlayableDirector>();
        director.stopped += OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        Debug.Log("Timeline finished");
        if (aDirector == director)
        {
            onTimelineFinish.Invoke();
        }
    }
}
