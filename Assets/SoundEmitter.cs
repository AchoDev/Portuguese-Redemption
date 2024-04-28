using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    
    SoundManager manager;

    void Start()
    {
        manager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
    }

    public void PlaySound(string soundName)
    {
        manager.Play(soundName);
    }
}
