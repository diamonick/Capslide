using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    private const float gravityInterval = 0.05f;
    [SerializeField] private int points;
    [SerializeField] private Rigidbody2D RB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        RB.gravityScale += gravityInterval;

        if (other.CompareTag("Slider"))
        {
            Debug.Log("HIT!");
        }
    }
}
