using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    // Constants
    private const float ACTIVATION_TIME = 10f;

    [SerializeField] private SpriteRenderer SPR;
    [SerializeField] private BoxCollider2D boxcollider2D;
    [SerializeField] private float activationTime;


    private void OnEnable()
    {
        SPR.color = new Color(SPR.color.r, SPR.color.g, SPR.color.b, 1f);
        boxcollider2D.enabled = true;
        activationTime = ACTIVATION_TIME;
    }

    private void Update()
    {
        ActivationTimer();
    }

    private void Deactivate()
    {
        SPR.color = new Color(SPR.color.r, SPR.color.g, SPR.color.b, 0.5f);
        boxcollider2D.enabled = false;
        activationTime = ACTIVATION_TIME;
    }

    private void Activate()
    {
        SPR.color = new Color(SPR.color.r, SPR.color.g, SPR.color.b, 1f);
        boxcollider2D.enabled = true;
    }

    private void ActivationTimer()
    {
        if (activationTime > 0f)
        {
            activationTime -= Time.deltaTime;
            activationTime = Mathf.Max(0f, activationTime);
        }
        else if (!boxcollider2D.enabled)
            Activate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Capsule"))
            Deactivate();
    }

}
