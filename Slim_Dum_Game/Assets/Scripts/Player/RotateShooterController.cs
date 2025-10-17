using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class RotateShooterController : MonoBehaviour
{


    [SerializeField]
    Camera cameraMain;

    
    public float rotationSpeed = 5f;

    private float positionOfPlayer;

    public float FacingDirection { get; private set; } = -1;

    private Vector2 mousePosition;
    private float currentAngle = 0f; 


    private Vector3 MousePosition()
    {
        Vector3 mouseWorldPosition = cameraMain.ScreenToWorldPoint(mousePosition);
        return mouseWorldPosition;
    }

    private Quaternion ShooterDirectionInWorld()
    {

        mousePosition = Mouse.current.position.ReadValue();
        Vector2 direction = (MousePosition() - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        return targetRotation;
    }

    private float ShooterPositionInWorld()
    {
        Vector3 mousePos = MousePosition();
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
        int normalizedDirection = (ShooterPositionInWorld() > 0) ? 1 : -1;
        FacingDirection = normalizedDirection;
        if (FacingDirection > 0)
        {
            positionOfPlayer = 0.5f;
        }
        else
        {
            positionOfPlayer = -0.5f;

        }
        

        transform.localPosition = new Vector2(positionOfPlayer, 0.14f);

    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, ShooterDirectionInWorld(), rotationSpeed * Time.deltaTime);

    }
}
