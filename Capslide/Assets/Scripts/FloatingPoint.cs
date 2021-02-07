using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPoint : MonoBehaviour
{
    private float timeUntilDisappear = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if (timeUntilDisappear > 0f)
        {
            timeUntilDisappear -= Time.deltaTime;
            timeUntilDisappear = Mathf.Max(0f, timeUntilDisappear);
        }
        else
            Destroy(this.gameObject);
    }
}
