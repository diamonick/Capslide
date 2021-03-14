using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaletteButton : MonoBehaviour
{
    // Constants
    private const int PALETTE_SIZE = 5;

    public Palette palette;
    [SerializeField] private Image[] paletteSquares;
    public bool isEquipped;
    public bool isUnlocked;
    public GameObject checkmark;
    public GameObject costPrompt;
    public TMP_Text paletteName;
    public TMP_Text costText;

    private void OnEnable()
    {
        SetPaletteButton();
    }

    public void SetPaletteButton()
    {
        if (isUnlocked)
            SetPaletteColors();

        if (PaletteManager.Instance.mainPalette == palette)
            Equip(false);

        paletteName.text = $"{palette.name}";
        costText.text = $"Pay {palette.tokenCost}";

        costPrompt.SetActive(!isUnlocked);
        paletteName.gameObject.SetActive(isUnlocked);
    }

    public void Equip(bool playSound = true)
    {
        if (playSound)
            AudioManager.Instance.PlaySFX("Select Palette");

        isEquipped = true;
        PaletteManager.Instance.mainPalette = palette;
        checkmark.SetActive(true);
        GameManager.Instance.Save();
    }

    public void Unequip()
    {
        isEquipped = false;
        checkmark.SetActive(false);
    }

    public void Purchase()
    {
        if (GameManager.Instance.tokens < palette.tokenCost)
            return;

        Deduct();
        isUnlocked = true;
        costPrompt.SetActive(false);
        paletteName.gameObject.SetActive(true);
        paletteName.text = $"{palette.name}";
        SetPaletteColors();

        GameManager.Instance.palettesUnlocked[palette.ID] = isUnlocked;
        CloudOnceServices.Instance.AwardColorMeImpressed();
        GameManager.Instance.Save();
    }

    private void Deduct()
    {
        GameManager.Instance.tokens -= palette.tokenCost;
        GameManager.Instance.SetTokenText();
    }

    private void SetPaletteColors()
    {
        for (int i = 0; i < PALETTE_SIZE; i++)
            paletteSquares[i].color = palette.colors[i];
    }
}
