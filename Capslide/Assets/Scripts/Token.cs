using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    // Constants
    private const float TIME_MAX = 10f;

    private GameObject obj;
    [SerializeField] private SpriteRenderer SPR;
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem CollectPS;
    [SerializeField] private bool timeRunning = true;
    [SerializeField] private bool startBlinking = false;
    [SerializeField] private bool isCollected = false;
    private float timeUntilDisappear;

    private void Awake() => obj = this.gameObject;

    private void OnEnable()
    {
        var main = CollectPS.main;
        main.startColor = PaletteManager.Instance.GetColor(2);

        timeUntilDisappear = TIME_MAX;
        timeRunning = true;
        startBlinking = false;
        isCollected = false;
        anim.speed = 1f;
        SPR.color = new Color(SPR.color.r, SPR.color.g, SPR.color.b, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!timeRunning)
            return;

        if (timeUntilDisappear > 0f)
        {
            timeUntilDisappear -= Time.deltaTime;
            timeUntilDisappear = Mathf.Max(0f, timeUntilDisappear);

            if (isCollected)
                return;

            if (timeUntilDisappear < 5f && !startBlinking)
                StartCoroutine(Blink());
        }
        else
            Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected)
            return;

        GameObject other = collision.gameObject;

        if (other.CompareTag("Capsule"))
        {
            isCollected = true;
            CollectPS.Play();
            timeUntilDisappear = 1.5f;
            GameplayManager.Instance.tokensEarned++;

            StopAllCoroutines();
            StartCoroutine(Rise());
            anim.speed = 10f;
        }
    }

    /// <summary>
    /// Have token blink a few times before disappearing.
    /// </summary>
    private IEnumerator Blink()
    {
        Color col1 = new Color(SPR.color.r, SPR.color.g, SPR.color.b, 0.05f);
        Color col2 = new Color(SPR.color.r, SPR.color.g, SPR.color.b, 1f);

        startBlinking = true;
        while (timeUntilDisappear > 0f)
        {
            yield return StartCoroutine(Ease.ColorChangeFromRGBA(obj, col1, 0.2f));
            yield return StartCoroutine(Ease.ColorChangeFromRGBA(obj, col2, 0.2f));
        }
    }

    /// <summary>
    /// Token rises into the air.
    /// </summary>
    private IEnumerator Rise()
    {
        float y1 = obj.transform.position.y + 360f;
        float y2 = obj.transform.position.y + 240f;
        Color noColor = new Color(SPR.color.r, SPR.color.g, SPR.color.b, 0f);

        SPR.color = new Color(SPR.color.r, SPR.color.g, SPR.color.b, 1f);
        yield return StartCoroutine(Ease.TranslateYTo(obj, y1, 1f, 2, Easing.EaseOut));
        StartCoroutine(Ease.ColorChangeFromRGBA(obj, noColor, 0.25f));
        StartCoroutine(Ease.TranslateYTo(obj, y2, 0.5f, 2, Easing.EaseIn));
    }

    /// <summary>
    /// Deactivate token and reset token spawn timer.
    /// </summary>
    private void Deactivate()
    {
        obj.SetActive(false);
        GameplayManager.Instance.ResetTokenSpawnTimer();
    }
}
