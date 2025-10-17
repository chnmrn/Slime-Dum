using UnityEngine;

[System.Serializable]
public class Sound
{
    [SerializeField]
    string name;

    [SerializeField]
    AudioClip audio;

    public string getName()
    {
        return name;
    }

    public AudioClip getAudio()
    {
        return audio;
    }
}
