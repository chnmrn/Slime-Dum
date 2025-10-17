using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float yOffset=1f;
    private Transform target;
    private void Update()
    {
        target = GameObject.FindWithTag("Player")?.transform;
        if (target == null ) return;
        Vector3 newPos = new(target.position.x,target.position.y + yOffset,-10F);
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed*Time.deltaTime);
    }
}
