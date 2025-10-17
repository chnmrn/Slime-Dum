using System;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    Sound[] sounds;

    AudioSource _musicSource;
    AudioSource _sfxSource;

    private void Awake()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _sfxSource = gameObject.AddComponent<AudioSource>();
    }

    private Sound FindSound(string name)
    {
        return Array.Find(sounds, f => f.getName() == name);
    }

    public void PlayMusic(string name, float volume)
    {
        Sound music = FindSound(name);
        if (music == null)
        {
            return;
        }
        _musicSource.volume = volume;
        _musicSource.loop = true;
        _musicSource.clip = music.getAudio();
        _musicSource.Play();
    }

    public void PlaySFX(string name, bool loop = false)
    {
        Sound sfx = FindSound(name);
        if (sfx == null)
        {
            return;
        }

        if (loop == true)
        {
            _sfxSource.loop = true;
            _sfxSource.clip = sfx.getAudio();
            _sfxSource.Play();
        }
        else
        {
            _sfxSource.PlayOneShot(sfx.getAudio());
        }
    }

    public void StopSFX()
    {
        _sfxSource.Stop();
    }
}
