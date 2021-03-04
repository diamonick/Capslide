using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockHand : MonoBehaviour
{
    private GameObject obj;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        obj = this.gameObject;
    }

    private void Update()
    {
        if (slider == null)
            return;

        obj.transform.eulerAngles = new Vector3(0f, 0f, (-180f * slider.GetBarRatio()) + 90f);
    }
}
