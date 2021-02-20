using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForegroundOverlay : MonoBehaviour
{
    // Game Manager Singleton
    public static ForegroundOverlay Instance { get; private set; }

    [SerializeField] private Image foregroundImage;
    [SerializeField] private Color color => foregroundImage.color;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning($"WARNING: There can only be one instance of this class.");
        }
    }
    private void Start()
    {
        foregroundImage.color = new Color(color.r, color.g, color.b, 0f);
    }

    /// <summary>
    /// Fade in foreground overlay.
    /// </summary>
    public void FadeInForeground(float time)
    {
        Color noColor = new Color(color.r, color.g, color.b, 0f);
        Color onColor = new Color(color.r, color.g, color.b, 1f);
        foregroundImage.color = noColor;

        if (time == 0f)
        {
            SetForegroundColor(onColor);
            return;
        }
        StartCoroutine(Ease.ChangeImageColor(foregroundImage, onColor, time));
    }

    /// <summary>
    /// Fade out foreground overlay.
    /// </summary>
    public void FadeOutForeground(float time)
    {
        Color noColor = new Color(color.r, color.g, color.b, 0f);

        if (time == 0f)
        {
            StopAllCoroutines();
            SetForegroundColor(noColor);
            return;
        }

        StartCoroutine(Ease.ChangeImageColor(foregroundImage, noColor, time));
    }

    /// <summary>
    /// Automatically set the foreground color.
    /// </summary>
    public void SetForegroundColor(Color newColor) => foregroundImage.color = newColor;
}
