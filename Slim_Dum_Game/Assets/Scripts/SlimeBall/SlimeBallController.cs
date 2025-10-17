using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlimeBallController : MonoBehaviour
{

    [SerializeField]
    private float damage = 20f;

    [SerializeField]
    private float lifeTime = 4.0F;

    [SerializeField]
    private float speed;

    [SerializeField]
    private LayerMask enemyMask;

    [SerializeField]
    private LayerMask obstacleMask;

    [SerializeField]
    private string nameOfSFXHit;

    [SerializeField]
    private string nameOfSFXDeath;

    private Rigidbody2D rb;

    private GameObject player;

    private AudioManager audioManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity =  transform.right * speed * Time.fixedDeltaTime;
        Destroy(gameObject, lifeTime);
        audioManager = FindObjectOfType<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (audioManager == null)
        {
            Debug.LogError("AudioManager no encontrado en la escena.");

        }
    }

   
    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (enemyMask.Includes(other.gameObject.layer))
        {
            HealthController healthController = other.gameObject.GetComponent<HealthController>();

            if (healthController != null)
            {
                
                healthController.TakeDamage(damage);
                healthController.UpdateHealthBar();
                
                audioManager.PlaySFX(nameOfSFXHit);
                
                if (healthController._currentHealth <= healthController.minHealth || healthController._currentHealth == 0.0f)
                {
                    if (audioManager != null)
                    {
                        audioManager.PlaySFX(nameOfSFXDeath); // can be loop on true  PlaySFX(name,bool);
                    }
                    if (healthController.CompareTag(player.name))
                    {
                        Scene currentScene = SceneManager.GetActiveScene();
                        SceneManager.LoadScene(currentScene.name);
                    }
                    else
                    {
                        Destroy(other.gameObject);
                    }
                }
                Destroy(gameObject);
            }
        }
        if (obstacleMask.Includes(other.gameObject.layer))
        {
            Destroy(gameObject);
        }
    }
}
