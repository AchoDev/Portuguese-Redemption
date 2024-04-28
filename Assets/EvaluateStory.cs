using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EvaluateStory : MonoBehaviour
{

    [SerializeField] PlayableDirector DefeatRamzaCutscene;

    public void Evaluate()
    {
        SceneManager sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<SceneManager>();
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
