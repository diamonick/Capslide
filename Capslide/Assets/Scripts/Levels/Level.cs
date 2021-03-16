using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int ID;
    public Transform[] spawnLocations;
    [Range(0,360)]
    public int[] launchers;

    public int GetRandomLauncher() => launchers[Random.Range(0, launchers.Length)];
}
