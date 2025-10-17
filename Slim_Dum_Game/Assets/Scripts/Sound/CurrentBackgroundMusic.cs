using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentBackgroundMusic : MonoBehaviour
{

    [SerializeField]
    private string music;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager no encontrado en la escena.");

        }
        audioManager.PlayMusic(music, 0.7f);
    }

}
