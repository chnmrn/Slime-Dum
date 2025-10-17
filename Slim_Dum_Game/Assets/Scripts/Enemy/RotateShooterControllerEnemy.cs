using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RotateShooterControllerEnemy : MonoBehaviour
{

    private Transform player;
    
    public float rotationSpeed = 5f;

    private float positionOfPlayer;

    public float FacingDirection { get; private set; } = -1;

    private float currentAngle = 0f;

    Vector3 directionPlayerOrPatrol;


    private float ShooterPositionInWorld()
    {
        Vector3 mousePos = directionPlayerOrPatrol;
        Vector3 playerPos = transform.position;

        float angleMouseAccordPlayer = Mathf.Atan2(mousePos.x - playerPos.x, mousePos.y - playerPos.y) * Mathf.Rad2Deg;

        float deadZoneLow = -170f;
        float deadZoneHigh = -8f;
        float deadZoneLow2 = 8f;
        float deadZoneHigh2 = 170f;


        if (currentAngle > deadZoneLow && currentAngle < deadZoneHigh)
        {
            if (angleMouseAccordPlayer > deadZoneLow2 && angleMouseAccordPlayer < deadZoneHigh2)
            {
                currentAngle = deadZoneLow2; 
            }
        }
        else if (currentAngle > deadZoneLow2 && currentAngle < deadZoneHigh2)
        {
            if (angleMouseAccordPlayer > deadZoneLow && angleMouseAccordPlayer < deadZoneHigh)
            {
                currentAngle = deadZoneLow; 
            }
        }
        else 
        {
            currentAngle = angleMouseAccordPlayer;
        }

        return currentAngle;

    }
    private void Update()
    {
        if (player == null) return;
        directionPlayerOrPatrol = player.transform.position - transform.position;
       
        int normalizedDirection = (ShooterPositionInWorld() > 0) ? 1 : -1;
        FacingDirection = normalizedDirection;
        if (FacingDirection > 0)
        {
            positionOfPlayer = transform.localPosition.x;
        }
        else
        {
            positionOfPlayer = -transform.localPosition.x;

        }
        transform.localPosition = new Vector2(positionOfPlayer, transform.localPosition.y);

    }

    private void RotateTowardsPlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

        // Smoothly rotate towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        RotateTowardsPlayer();
    }
}
