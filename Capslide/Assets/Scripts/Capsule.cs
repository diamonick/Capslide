﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Capsule : MonoBehaviour
{
    // Constants
    private const float INITIAL_GRAVITY = 50f;
    private const float GRAVITY_INTERVAL = 1f;
    private const float MAX_SPEED = 720f;
    private const float DEADZONE_VALUE = 180f;

    [SerializeField] private int points = 0;
    [SerializeField] private ParticleSystem BouncePS;
    [SerializeField] private GameObject floatingPoint;
    [SerializeField] private SpriteRenderer SPR;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite starSprite;
    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private CircleCollider2D circleCollider2d;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool starred = false;
    private Vector3 startForce;

    // Start is called before the first frame update
    void OnEnable()
    {
        startForce = new Vector3(Random.Range(-MAX_SPEED, MAX_SPEED), 0f, 0f);
        RB.AddForce(startForce, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -MAX_SPEED, MAX_SPEED), Mathf.Clamp(RB.velocity.y, -MAX_SPEED, MAX_SPEED));
        HasTouchedSlider();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -MAX_SPEED, MAX_SPEED), Mathf.Clamp(RB.velocity.y, -MAX_SPEED, MAX_SPEED));


        if (other.CompareTag("Slider") || other.CompareTag("Platform"))
        {
            RB.gravityScale += GRAVITY_INTERVAL;
            RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -MAX_SPEED, MAX_SPEED), Mathf.Clamp(RB.velocity.y, -MAX_SPEED, MAX_SPEED));

            if (other.CompareTag("Slider") && HasTouchedSlider())
            {
                SetCapsuleValue(ref points);

                var pp = BouncePS.main;
                Color col1 = GameManager.Instance.GetColor(Random.Range(0, 5));
                Color col2 = GameManager.Instance.GetColor(Random.Range(0, 5));
                pp.startColor = new ParticleSystem.MinMaxGradient(col1, col2);

                BouncePS.Play();
                if (InDeadZone())
                    return;

                GameObject fp = Instantiate(floatingPoint, this.gameObject.transform.position, Quaternion.identity);
                GameplayManager.Instance.SetScore(points);
                fp.transform.GetChild(0).GetComponent<TMP_Text>().text = $"+{points}";
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Dead Zone"))
            StartCoroutine(ShakeScreen());
    }

    private IEnumerator ShakeScreen()
    {
        if (GameManager.Instance.screenShakeON)
            yield return StartCoroutine(ScreenShake.Shake(20f, 0.5f));

        points = 0;
        RB.gravityScale = INITIAL_GRAVITY;
        SPR.sprite = normalSprite;
        starred = false;
        this.gameObject.SetActive(false);

        if (GameplayManager.Instance.NoCapsulesInGame())
        {
            if (GameplayManager.Instance.DispenserIsEmpty())
                GameplayManager.Instance.EndGame();
            else
                GameplayManager.Instance.DispenseCapsule();
        }
    }

    public void StarCapsule()
    {
        starred = true;
        SPR.sprite = starSprite;
        RB.gravityScale = 75f;
    }

    /// <summary>
    /// Increments the amount of points the player recieves based on capsule type.
    /// </summary>
    /// <returns>Total points. Points are incremented by 1 or 3 for each bounce.</returns>
    private void SetCapsuleValue(ref int pts)
    {
        if (starred)
            pts += 3;
        else
            pts += 1;
    }

    /// <summary>
    /// Checks if capsule is touching the silder from the top.
    /// </summary>
    /// <returns>TRUE if capsule has collided the top of the slider. Otherwise, FALSE.</returns>
    private bool HasTouchedSlider()
    {
        Color raycastColor1, raycastColor2, raycastColor3;
        RaycastHit2D raycastHit = Physics2D.Raycast(circleCollider2d.bounds.center, Vector2.down, circleCollider2d.bounds.extents.y + 48f, groundLayer);
        RaycastHit2D raycastRight = Physics2D.Raycast(circleCollider2d.bounds.center, Vector2.right, circleCollider2d.bounds.extents.x + 48f, groundLayer);
        RaycastHit2D raycastLeft = Physics2D.Raycast(circleCollider2d.bounds.center, Vector2.left, circleCollider2d.bounds.extents.x + 48f, groundLayer);
#if UNITY_EDITOR
        if (raycastHit.collider != null)
            raycastColor1 = Color.green;
        else
            raycastColor1 = Color.red;

        if (raycastRight.collider != null)
            raycastColor2 = Color.green;
        else
            raycastColor2 = Color.red;

        if (raycastLeft.collider != null)
            raycastColor3 = Color.green;
        else
            raycastColor3 = Color.red;

        Debug.DrawRay(circleCollider2d.bounds.center, Vector2.down * (circleCollider2d.bounds.extents.y + 48f), raycastColor1);
        Debug.DrawRay(circleCollider2d.bounds.center, Vector2.right * (circleCollider2d.bounds.extents.x + 48f), raycastColor2);
        Debug.DrawRay(circleCollider2d.bounds.center, Vector2.left * (circleCollider2d.bounds.extents.x + 48f), raycastColor3);
#endif
        return true;
        //return raycastHit.collider != null || raycastRight.collider != null || raycastLeft.collider != null;
    }

    private bool InDeadZone() => RB.velocity.y < DEADZONE_VALUE && RB.velocity.y > -DEADZONE_VALUE;
}
