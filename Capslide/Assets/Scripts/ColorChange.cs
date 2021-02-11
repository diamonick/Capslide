using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorChange : MonoBehaviour
{
    public enum ColorType
    {
        White = -1,
        Lightest,
        Light,
        Main,
        Dark,
        Darkest,
        Score,
        CapsuleTimer,
        Back,
        Retry,
        RewardedAd,
    }

    [SerializeField] private SpriteRenderer SPR;
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private ColorType colorType;
    private GameObject obj;

    private void Awake()
    {
        obj = this.gameObject;
    }

    private void Start()
    {
        Color col;
        switch (colorType)
        {
            case ColorType.White:
                col = Color.white;
                break;
            case ColorType.Score:
                col = GameManager.Instance.GetScoreColor();
                break;
            case ColorType.CapsuleTimer:
                col = GameManager.Instance.GetCapsuleTimerColor();
                break;
            case ColorType.Back:
                col = GameManager.Instance.GetBackButtonColor();
                break;
            case ColorType.Retry:
                col = GameManager.Instance.GetRetryButtonColor();
                break;
            case ColorType.RewardedAd:
                col = GameManager.Instance.GetRewardedAdButtonColor();
                break;
            default:
                col = GameManager.Instance.GetColor((int)colorType);
                break;
        }

        if (SPR != null)
            SPR.color = col;
        if (image != null)
            image.color = col;
        if (tmpText != null)
            tmpText.color = col;
        if (button != null)
        {
            var colors = button.colors;
            colors.pressedColor = col;
            colors.selectedColor = new Color(col.r, col.g, col.b, 0f);
            button.colors = colors;
        }
    }
}
