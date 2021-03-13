using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorChange : MonoBehaviour
{
    public enum ColorType
    {
        Black = -2,
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
        Token
    }

    [SerializeField] private SpriteRenderer SPR;
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private ForegroundOverlay overlay;
    [SerializeField] private Camera camera;
    [SerializeField] private ColorType colorType;
    private ColorType colorTypePrev;
    private Color mainColor;
    private GameObject obj;

    private void Awake()
    {
        obj = this.gameObject;
    }

    private void Start()
    {
        ChangeColor();
    }

    private void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        Color col;
        switch (colorType)
        {
            case ColorType.White:
                col = Color.white;
                break;
            case ColorType.Black:
                col = Color.black;
                break;
            case ColorType.Score:
                col = PaletteManager.Instance.GetScoreColor();
                break;
            case ColorType.CapsuleTimer:
                col = PaletteManager.Instance.GetCapsuleTimerColor();
                break;
            case ColorType.Back:
                col = PaletteManager.Instance.GetBackButtonColor();
                break;
            case ColorType.Retry:
                col = PaletteManager.Instance.GetRetryButtonColor();
                break;
            case ColorType.RewardedAd:
                col = PaletteManager.Instance.GetRewardedAdButtonColor();
                break;
            case ColorType.Token:
                col = PaletteManager.Instance.GetTokenColor();
                break;
            default:
                col = PaletteManager.Instance.GetColor((int)colorType);
                break;
        }

        if (col == mainColor)
            return;

        if (SPR != null)
            SPR.color = new Color(col.r, col.g, col.b, SPR.color.a);
        if (image != null)
            image.color = new Color(col.r, col.g, col.b, image.color.a);
        if (tmpText != null)
            tmpText.color = new Color(col.r, col.g, col.b, tmpText.color.a);
        if (button != null)
        {
            var colors = button.colors;
            colors.pressedColor = col;
            colors.selectedColor = new Color(col.r, col.g, col.b, 0f);
            button.colors = colors;
        }
        if (overlay != null)
        {
            overlay.SetForegroundColor(col);
            overlay.FadeOutForeground(0f);
        }
        if (camera != null)
            camera.backgroundColor = col;

        mainColor = col;
    }
}
