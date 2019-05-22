using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public bool notDeleting;

    //public static AudioManager instance;

    void Awake()
    {
        /*if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }*/

        if (notDeleting) {
            DontDestroyOnLoad(this.gameObject);
        }

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }

    void Start() {
        Play("Theme");
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s != null) {
            s.source.Play();
        }
    }
}
