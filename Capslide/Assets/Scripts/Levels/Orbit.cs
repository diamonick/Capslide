﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    // Constants
    private const float SPEED_SCALE = 50f;

    private float angle;
    [SerializeField] private float startAngle;
    [SerializeField] private Transform center;
    [SerializeField] private float angleRate;
    [SerializeField] private float radius;

    private void OnEnable()
    {
        angle = startAngle;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            angle += (angleRate * SPEED_SCALE * Time.deltaTime) % 360f;
            this.gameObject.transform.position = RadialVector(center.position, radius);
        }
    }

    private Vector3 RadialVector(Vector3 center, float radius)
    {
        Vector3 pos;
        pos.x = center.x + radius * -Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }
}
