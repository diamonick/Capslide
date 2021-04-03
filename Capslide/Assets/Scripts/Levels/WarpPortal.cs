using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpPortal : MonoBehaviour
{
    [SerializeField] private Transform[] destinations;
    [SerializeField] private SpriteRenderer SPR;
    [SerializeField] private ParticleSystem warpPS;

    private void OnEnable() => InvokeRepeating("RandomizeColors", 0f, 2f);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Capsule other = collision.gameObject.GetComponent<Capsule>();

        if (other.CompareTag("Capsule"))
            Warp(other);
    }

    private void Warp(Capsule capsule)
    {
        if (capsule.isLaunched)
            return;

        Transform dest = destinations.Length == 1 ? destinations[0] : destinations[Random.Range(0, 2)];
        capsule.transform.position = dest.position;
        capsule.Launch();
    }

    private void RandomizeColors()
    {
        var pp = warpPS.main;
        Color col1 = PaletteManager.Instance.GetColor(Random.Range(0, 4));
        Color col2 = PaletteManager.Instance.GetColor(Random.Range(0, 4));
        pp.startColor = new ParticleSystem.MinMaxGradient(col1, col2);
    }
}
