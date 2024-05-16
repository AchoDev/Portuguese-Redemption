using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class EvaluateStoryBillstedt : MonoBehaviour
{

    float normalOrtho;

    
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

    [Header("Day two flight (5)")]
    [SerializeField] GameObject LeoSuperhero;
    [SerializeField] float ortho;


    [Header("Story start")]
    [SerializeField] StoryProgression storyProgression;

    void Awake()
    {
        StaticSceneInformation.currentProgression = storyProgression;
        normalOrtho = VirtualCamera.m_Lens.OrthographicSize;
        
        Evaluate();
    }

    public void Evaluate()
    {
        StoryProgression progression = sceneManager.GetStoryProgression();

        globalLight.intensity = 1;
        VirtualCamera.m_Lens.OrthographicSize = normalOrtho;

        switch (progression)
        {
            case StoryProgression.Prologue:
                // ActivateCharacter(LeoTshirt);
                break;
            case StoryProgression.DayOne:
                ActivateCharacter(LeoTshirt);
                break;
            case StoryProgression.DefeatRamza:
                ActivateCharacter(LeoTshirt);
                DefeatRamzaCutscene.Play();
                break;
            case StoryProgression.DayOneAfternoon:
                ActivateCharacter(LeoTshirt);
                break;

            case StoryProgression.DayOneNight:
                ActivateCharacter(LeoNightguard);
                globalLight.intensity = nightLightIntensity;
                mainCamera.backgroundColor = skyColor;
                nightCutsceneTrigger.SetActive(true);
                Leon.SetActive(false);
                break;

            case StoryProgression.DayTwoFlight:
                ActivateCharacter(LeoSuperhero);
                VirtualCamera.m_Lens.OrthographicSize = ortho;
                break;
        }
    }

    void ActivateCharacter(GameObject character)
    {
        LeoTshirt.SetActive(false);
        LeoNightguard.SetActive(false);
        LeoSuperhero.SetActive(false);
        character.SetActive(true);
        VirtualCamera.Follow = character.transform;
    }
}
