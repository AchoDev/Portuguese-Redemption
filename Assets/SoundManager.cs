using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Sound[] sounds;

    Sound findSound(string name) {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }
        return s;
    }

    public void Play(string name) {
        Sound s = findSound(name);
        s.Play();
    }

    public void Play(string name, float volume) {
        Sound s = findSound(name);
        s.volume = volume;
        s.Play();
    }

    void Start() {
        foreach (Sound s in sounds) {
            AudioSource a = gameObject.AddComponent<AudioSource>();
            s.SetSource(a);
            if(s.playOnAwake) {
                s.Play();
            }
        }
    }
} 

[System.Serializable]
class Sound
{
    public string name;
    [Range(0, 1)] public float volume = 0.5f;
    public AudioClip clip;
    public bool loop;
    public bool playOnAwake;
    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        source.playOnAwake = playOnAwake;
    }

    public void Play()
    {
        source.Play();
    }
}