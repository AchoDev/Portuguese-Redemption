using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class EvaluateStory : MonoBehaviour
{

    
    [SerializeField] PlayableDirector DefeatRamzaCutscene;
    [SerializeField] SceneManager sceneManager;

    [Header("Day one night (4)")]
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject nightOverlay;


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

            case StoryProgression.DayOneNight:
                mainCamera.backgroundColor = new Color(0, 0, 0);
                nightOverlay.SetActive(true);
                break;
        }
    }
}
