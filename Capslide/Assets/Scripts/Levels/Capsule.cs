using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Capsule : MonoBehaviour
{
    // Constants
    private const float INITIAL_GRAVITY = 50f;
    private const float GRAVITY_NORMAL = 1f;
    private const float GRAVITY_STAR = 1.8f;
    private const float MAX_SPEED = 720f;
    private const float DEADZONE_VALUE = 144f;

    public SpriteRenderer SPR;
    [SerializeField] private int points = 0;
    [SerializeField] private ParticleSystem trailPS;
    [SerializeField] private ParticleSystem specialTrailPS;
    [SerializeField] private ParticleSystem BouncePS;
    [SerializeField] private GameObject floatingPoint;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite starSprite;
    [SerializeField] private Sprite fakeSprite;
    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private CircleCollider2D circleCollider2d;
    [SerializeField] private LayerMask groundLayer;
    public bool starred = false;
    public bool faked = false;
    public bool isLaunched = false;
    private Vector3 startForce;

    // Start is called before the first frame update
    void OnEnable()
    {
        RB.isKinematic = false;
        circleCollider2d.enabled = true;
        SPR.color = new Color(SPR.color.r, SPR.color.g, SPR.color.b, 1f);

        points = 0;
        StartLaunch();
        //startForce = new Vector3(Random.Range(-MAX_SPEED, MAX_SPEED), 0f, 0f);
        //RB.AddForce(startForce, ForceMode2D.Impulse);

        // Set color of trail particle system.
        var main = trailPS.main;
        main.startColor = SPR.color;

        // Set color of special trail particle system.
        var main2 = specialTrailPS.main;
        main2.startColor = SPR.color;
    }

    // Update is called once per frame
    void Update()
    {
        float speedMax = RB.isKinematic ? 1440f : MAX_SPEED;
        RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -speedMax, speedMax), Mathf.Clamp(RB.velocity.y, -speedMax, speedMax));
        HasTouchedSlider();
    }

    private void StartLaunch()
    {
        Level level = GameManager.Instance.currentLevel;

        float ballSpeed = Random.Range(360f, MAX_SPEED);
        Vector3 direction = VectorFromAngle(level.GetRandomLauncher());
        RB.velocity = direction * ballSpeed;
    }

    //Converts an angle (in degrees) to a 2D vector
    private Vector3 VectorFromAngle(float theta)
    {
        var force = Quaternion.AngleAxis(theta, Vector3.forward) * Vector3.right;
        force.Normalize();
        return force;
    }

    private void Emit()
    {
        var pp = BouncePS.main;
        Color col1 = PaletteManager.Instance.GetColor(Random.Range(0, 4));
        Color col2 = PaletteManager.Instance.GetColor(Random.Range(0, 4));
        pp.startColor = new ParticleSystem.MinMaxGradient(col1, col2);
        BouncePS.Play();
    }

    public IEnumerator Deactivate()
    {
        Emit();
        trailPS.Stop();
        RB.velocity = Vector2.zero;
        RB.isKinematic = true;
        circleCollider2d.enabled = false;
        SPR.color = new Color(0f, 0f, 0f, 0f);
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -MAX_SPEED, MAX_SPEED), Mathf.Clamp(RB.velocity.y, -MAX_SPEED, MAX_SPEED));


        if (other.CompareTag("Slider") || other.CompareTag("Platform"))
        {
            RB.gravityScale += starred ? GRAVITY_STAR : GRAVITY_NORMAL;
            RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -MAX_SPEED, MAX_SPEED), Mathf.Clamp(RB.velocity.y, -MAX_SPEED, MAX_SPEED));

            if (other.CompareTag("Slider") && HasTouchedSlider())
            {
                Emit();

                if (InDeadZone())
                    return;

                SetCapsuleValue(ref points);
                AudioManager.Instance.PlayAltSFX("Bounce", Random.Range(0.75f, 1f), Random.Range(0.8f, 1f));
                GameObject fp = Instantiate(floatingPoint, this.gameObject.transform.position, Quaternion.identity);
                GameplayManager.Instance.SetScore(points);

                if (points == 99 && starred)
                    CloudOnceServices.Instance.AwardStarPlayer();

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

    private IEnumerator ShakeScreen(bool specialLaunch = false)
    {
        if (GameplayManager.Instance.GetCapsulesInGame() == 1 && !faked)
            GameplayManager.Instance.timeStopped = true;

        if (GameManager.Instance.screenShake && !specialLaunch)
            yield return StartCoroutine(ScreenShake.Shake(20f, 0.5f));

        points = 0;
        RB.gravityScale = INITIAL_GRAVITY;
        SPR.sprite = normalSprite;
        starred = false;
        this.gameObject.SetActive(false);

        if (faked)
            yield break;

        if (GameplayManager.Instance.NoCapsulesInGame())
        {
            if (GameplayManager.Instance.DispenserIsEmpty())
                GameplayManager.Instance.EndGame();
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

        pts = Mathf.Clamp(pts, -99, 99);
        
        if (pts == 99)
            StartCoroutine(SpecialLaunch());
    }

    private IEnumerator SpecialLaunch()
    {
        AudioManager.Instance.PlaySFX("Star Chime", Random.Range(0.75f, 1f));

        RB.isKinematic = true;
        circleCollider2d.enabled = false;
        trailPS.Stop();
        specialTrailPS.Play();
        RB.velocity = new Vector2(0f, 1920f);
        yield return new WaitForSeconds(1f);
        StartCoroutine(ShakeScreen(true));
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
