using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EvaluateStory : MonoBehaviour
{

    [SerializeField] PlayableDirector DefeatRamzaCutscene;
    [SerializeField] SceneManager sceneManager;

    void Start()
    {
        Evaluate();
    }

    public void Evaluate()
    {
        StoryProgression progression = sceneManager.GetStoryProgression();
        switch (progression)
        {
            case StoryProgression.Prologue:
                // Evaluate Prologue
                break;
            case StoryProgression.DayOne:
                // Evaluate DayOne
                break;
            case StoryProgression.DefeatRamza:
                DefeatRamzaCutscene.Play();
                break;
            case StoryProgression.DayOneAfternoon:
                // Evaluate DayOneAfternoon
                break;
        }
    }
}
