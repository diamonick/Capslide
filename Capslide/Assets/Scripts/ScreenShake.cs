using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake
{
    private static Camera mainCamera;
    public static IEnumerator Shake(float frequency, float rate)
    {
        mainCamera = Camera.main;
        Vector3 origPos = new Vector3(414, 900);

        while (IsShaking(frequency))
        {
            float x = Random.Range(-1f, 1f) * frequency;
            float y = Random.Range(-1f, 1f) * frequency;
            Vector3 offset = new Vector3(x, y, 0f);

            mainCamera.transform.position = origPos + offset;
            frequency = Mathf.Max(0f, frequency - rate);
            yield return new WaitForEndOfFrame();
        }
        mainCamera.transform.position = origPos;
    }
    private static bool IsShaking(float frequency) => frequency > 0f;
}
