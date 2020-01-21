using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour {

    [SerializeField]
    public Sound[] mmSounds;

    public static AudioManager instance;

	void Awake () {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
            

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in mmSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
	}
    private void Start()
    {
        Play("music_CafeMimic");
    }
    public void Play (string name)
    {
        Sound s = Array.Find(mmSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }
}
