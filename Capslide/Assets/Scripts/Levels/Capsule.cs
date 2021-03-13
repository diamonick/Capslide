using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Capsule : MonoBehaviour
{
    // Constants
    private const float INITIAL_GRAVITY = 50f;
    private const float GRAVITY_INTERVAL = 1f;
    private const float MAX_SPEED = 720f;
    private const float DEADZONE_VALUE = 144f;

    public SpriteRenderer SPR;
    [SerializeField] private int points = 0;
    [SerializeField] private ParticleSystem trailPS;
    [SerializeField] private ParticleSystem BouncePS;
    [SerializeField] private GameObject floatingPoint;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite starSprite;
    [SerializeField] private Sprite fakeSprite;
    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private CircleCollider2D circleCollider2d;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool starred = false;
    [SerializeField] private bool faked = false;
    public bool isLaunched = false;
    private Vector3 startForce;

    // Start is called before the first frame update
    void OnEnable()
    {
        points = 0;
        startForce = new Vector3(Random.Range(-MAX_SPEED, MAX_SPEED), 0f, 0f);
        RB.AddForce(startForce, ForceMode2D.Impulse);

        var main = trailPS.main;
        main.startColor = SPR.color;
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
                // Play Bounce particle effect is Power Saving is OFF.
                if (!GameManager.Instance.powerSaving)
                {
                    var pp = BouncePS.main;
                    Color col1 = PaletteManager.Instance.GetColor(Random.Range(0, 4));
                    Color col2 = PaletteManager.Instance.GetColor(Random.Range(0, 4));
                    pp.startColor = new ParticleSystem.MinMaxGradient(col1, col2);

                    BouncePS.Play();
                }
                
                if (InDeadZone())
                    return;

                SetCapsuleValue(ref points);
                AudioManager.Instance.PlayAltSFX("Bounce", Random.Range(0.75f, 1f), Random.Range(0.8f, 1f));
                GameObject fp = Instantiate(floatingPoint, this.gameObject.transform.position, Quaternion.identity);
                GameplayManager.Instance.SetScore(points);

                if (faked)
                    fp.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{points}";
                else
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
        if (GameplayManager.Instance.GetCapsulesInGame() == 1 && !faked)
            GameplayManager.Instance.timeStopped = true;

        if (GameManager.Instance.screenShake)
            yield return StartCoroutine(ScreenShake.Shake(20f, 0.5f));

        points = 0;
        RB.gravityScale = INITIAL_GRAVITY;
        SPR.sprite = normalSprite;
        starred = false;
        this.gameObject.SetActive(false);

        if (GameplayManager.Instance.NoCapsulesInGame())
        {
            if (GameplayManager.Instance.DispenserIsEmpty())
            {
                if (GameplayManager.Instance.NoFakeCapsulesInGame())
                    GameplayManager.Instance.EndGame();
            }
            else
                GameplayManager.Instance.DispenseCapsule();
        }
    }

    /// <summary>
    /// Set this capsule into a Star Capsule.
    /// </summary>
    public void StarCapsule()
    {
        starred = true;
        SPR.sprite = starSprite;
    }

    /// <summary>
    /// Set this capsule into a Fake Capsule.
    /// </summary>
    public void FakeCapsule()
    {
        faked = true;
        SPR.sprite = fakeSprite;
    }

    /// <summary>
    /// Increments the amount of points the player recieves based on capsule type.
    /// </summary>
    /// <returns>Total points.
    ///  |Normal Capsule: Add +1 point for each bounce.|
    ///  |Star Capsule: Add +3 point for each bounce.|
    ///  |Fake Capsule: Deduct -5 point for each bounce.|</returns>
    private void SetCapsuleValue(ref int pts)
    {
        if (starred)
            pts += 3;
        else if (faked)
            pts -= 5;
        else
            pts += 1;
    }

    public void Launch()
    {
        isLaunched = true;
        RB.velocity = Vector3.zero;
        startForce = new Vector3(Random.Range(-MAX_SPEED, MAX_SPEED), 720f, 0f);
        RB.AddForce(startForce, ForceMode2D.Impulse);
        Invoke("TurnOffLaunch", 0.5f);
    }

    private void TurnOffLaunch() => isLaunched = false;

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

    private void OnDisable()
    {
        CancelInvoke();
    }
}
