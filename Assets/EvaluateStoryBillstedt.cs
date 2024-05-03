using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class EvaluateStoryBillstedt : MonoBehaviour
{

    
    [Header("General things")]
    [SerializeField] PlayableDirector DefeatRamzaCutscene;
    [SerializeField] SceneManager sceneManager;
    [SerializeField] CinemachineVirtualCamera VirtualCamera;

    [SerializeField] GameObject LeoTshirt;
    [SerializeField] GameObject Leon;


    [Header("Day one night (4)")]
    [SerializeField] Light2D globalLight;
    [SerializeField] float nightLightIntensity = 0.1f;
    [SerializeField] Camera mainCamera;
    [SerializeField] Color skyColor;
    [SerializeField] GameObject LeoNightguard;
    [SerializeField] GameObject nightCutsceneTrigger;

    [Header("Story start")]
    [SerializeField] StoryProgression storyProgression;

    void Awake()
    {
        StaticSceneInformation.currentProgression = storyProgression;
        Evaluate();
    }

    public void Evaluate()
    {
        StoryProgression progression = sceneManager.GetStoryProgression();
        switch (progression)
        {
            case StoryProgression.Prologue:
                // ActivateCharacter(LeoTshirt);
                globalLight.intensity = 1;
                break;
            case StoryProgression.DayOne:
                ActivateCharacter(LeoTshirt);
                globalLight.intensity = 1;
                break;
            case StoryProgression.DefeatRamza:
                ActivateCharacter(LeoTshirt);
                globalLight.intensity = 1;
                DefeatRamzaCutscene.Play();
                break;
            case StoryProgression.DayOneAfternoon:
                ActivateCharacter(LeoTshirt);
                globalLight.intensity = 1;
                break;

            case StoryProgression.DayOneNight:
                ActivateCharacter(LeoNightguard);
                globalLight.intensity = nightLightIntensity;
                mainCamera.backgroundColor = skyColor;
                nightCutsceneTrigger.SetActive(true);
                Leon.SetActive(false);
                break;
        }
    }

    void ActivateCharacter(GameObject character)
    {
        LeoTshirt.SetActive(false);
        LeoNightguard.SetActive(false);
        character.SetActive(true);
        VirtualCamera.Follow = character.transform;
    }
}
