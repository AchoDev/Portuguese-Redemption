using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void ProgressStory(int id) {
        StaticSceneInformation.currentProgression = (StoryProgression)id;
    }

    public StoryProgression GetStoryProgression() {
        return StaticSceneInformation.currentProgression;
    }
}
