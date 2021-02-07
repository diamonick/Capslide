using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorChange : MonoBehaviour
{
    public enum ColorType
    {
        White = 0,
        Lightest,
        Light,
        Main,
        Dark,
        Darkest,
        Score,
        CapsuleTimer
    }

    [SerializeField] private SpriteRenderer SPR;
    [SerializeField] private Image image;
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
    }
}
