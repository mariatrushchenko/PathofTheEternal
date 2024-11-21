using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("UNIVERSAL SOUNDS ONLY")]
    public Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // create an audiosource component for each element in the Sounds array and copy values to the audio source
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source = gameObject.AddComponent<AudioSource>();
            AudioSource source = sounds[i].source;

            // add audio output group later
            source.clip = sounds[i].audioClip;
            source.outputAudioMixerGroup = sounds[i].audioMixer;
            source.volume = sounds[i].volume;
            source.pitch = sounds[i].pitch;
            source.loop = sounds[i].loop;
        }
    }

    public void Play(string name)
    {
        bool foundSound = false;
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].soundName == name)
            {
                sounds[i].source.Play();
                foundSound = true;
            }
        }

        if (foundSound == false)
            Debug.LogError("Sound: " + name + " was not found!");
    }

    // loops through the sounds array to find the sound clip that matches the input parameter. Then get the source component
    // of the sound clip to play the actual audio. 
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " was not found!");
            return;
        }

        s.source.Stop();
    }
}
