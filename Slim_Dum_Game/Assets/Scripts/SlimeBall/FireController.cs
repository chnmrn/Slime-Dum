using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField]
    Transform firePoint;

    [SerializeField]
    GameObject slimeBallPrefab;
 
    [SerializeField]
    [Range(0.1F, 1.0F)]
    public float fireRate = 0.35F;

    [SerializeField]
    private string nameOfSFXShoot;

    private AudioManager audioManager;


    public void Fire()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager no encontrado en la escena.");

        }
        audioManager.PlaySFX(nameOfSFXShoot); // can be loop on true  PlaySFX(name,bool);
        Instantiate(slimeBallPrefab, firePoint.position, firePoint.rotation);
    }
}
