using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    private float angle;
    [SerializeField] private Transform center;
    [SerializeField] private float angleRate;
    [SerializeField] private float radius;

    private void OnEnable()
    {
        angle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        angle += angleRate;
        this.gameObject.transform.position = RadialVector(center.position, radius);
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
