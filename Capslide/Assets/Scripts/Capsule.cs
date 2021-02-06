using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Capsule : MonoBehaviour
{
    // Constants
    private const float GRAVITY_INTERVAL = 1f;
    private const float MAX_SPEED = 720f;

    [SerializeField] private int points = 0;
    [SerializeField] private ParticleSystem BouncePS;
    [SerializeField] private GameObject[] pointTallies;
    [SerializeField] private GameObject floatingPoints;
    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private CircleCollider2D circleCollider2d;
    [SerializeField] private LayerMask groundLayer;
    private Vector3 startForce;

    // Start is called before the first frame update
    void Start()
    {
        startForce = new Vector3(Random.Range(-MAX_SPEED, MAX_SPEED), 0f, 0f);
        RB.AddForce(startForce, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
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
                BouncePS.Play();
                if (InDeadZone())
                    return;

                GameObject fp = Instantiate(floatingPoints, this.gameObject.transform.position, Quaternion.identity);
                GameplayManager.Instance.SetScore(++points);
                fp.transform.GetChild(0).GetComponent<TMP_Text>().text = $"+{points}";
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Dead Zone"))
        {
            this.gameObject.SetActive(false);

            if (GameplayManager.Instance.NoCapsulesInGame())
                GameplayManager.Instance.DispenseCapsule();
        }
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
        return raycastHit.collider != null || raycastRight.collider != null || raycastLeft.collider != null;
    }

    private bool InDeadZone() => RB.velocity.y < 100f && RB.velocity.y > -100f;
}
