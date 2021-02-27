using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteButton : MonoBehaviour
{
    // Constants
    private const int PALETTE_SIZE = 5;

    public Palette palette;
    [SerializeField] private Image[] paletteSquares;
    public bool isEquipped;
    public bool isUnlocked;
    public GameObject checkmark;

    private void OnEnable()
    {
        for (int i = 0; i < PALETTE_SIZE; i++)
            paletteSquares[i].color = palette.colors[i];

        if (PaletteManager.Instance.mainPalette == palette)
            Equip();
    }

    public void Equip()
    {
        isEquipped = true;
        PaletteManager.Instance.mainPalette = palette;
        checkmark.SetActive(true);
    }

    public void Unequip()
    {
        isEquipped = false;
        checkmark.SetActive(false);
    }

}
