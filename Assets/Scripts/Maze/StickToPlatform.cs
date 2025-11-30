using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToPlatform : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayer;      
    [SerializeField] private float stickDistance = 0.5f;    
    [SerializeField] private float stickForce = 10f;        // Сила "прилипания"
    [SerializeField] private float maxVerticalVelocity = 0.2f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Проверяем, есть ли платформа под шариком
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, stickDistance + 0.1f, platformLayer))
        {
            // Если шарик слишком далеко от платформы — притягиваем его
            float distance = hit.distance;
            if (distance > stickDistance)
            {
                Vector3 targetPosition = hit.point + Vector3.up * stickDistance;
                rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * stickForce));
            }
        }
    }
}
