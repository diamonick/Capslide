using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Slider : MonoBehaviour
{
    public enum Placement
    {
        Left,
        Right,
        Top,
        Bottom,
        Center
    }

    // Constants
    private const float SLIDER_WEIGHT = 8f;

    private Vector3 positionPrev;
    private GameObject obj;

    [Header("Slider"), Space(8)]
    [SerializeField] private Vector2 origin;
    [SerializeField] private Placement placement;
    private Vector2 startpoint;
    private Vector2 endpoint;
    [SerializeField] private GameObject sliderBar;
    [Space(8)]
    [SerializeField] private GameObject leftBar;
    [SerializeField] private GameObject rightBar;
    [SerializeField] private bool isDraggable;

    [Header("Freeze Positions"), Space(8)]
    [SerializeField] private bool freezeX = false;
    [SerializeField] private bool freezeY = false;

    private void Awake()
    {
        obj = this.gameObject;

        if (freezeX)
        {
            float topY = sliderBar.transform.position.y + (sliderBar.transform.localScale.y / 2);
            float bottomY = sliderBar.transform.position.y - (sliderBar.transform.localScale.y / 2);

            startpoint = new Vector3(sliderBar.transform.position.x, topY, sliderBar.transform.position.z);
            endpoint = new Vector3(sliderBar.transform.position.x, bottomY, sliderBar.transform.position.z);
        }
        else if (freezeY)
        {
            float leftX = sliderBar.transform.position.x - (sliderBar.transform.localScale.x / 2);
            float rightX = sliderBar.transform.position.x + (sliderBar.transform.localScale.x / 2);

            startpoint = new Vector3(leftX, sliderBar.transform.position.y, sliderBar.transform.position.z);
            endpoint = new Vector3(rightX, sliderBar.transform.position.y, sliderBar.transform.position.z);
        }

        ResetFillBars();
        obj.transform.position = GetPlacement();
        origin = obj.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (positionPrev != obj.transform.position)
            positionPrev = obj.transform.position;

        if ((Vector2)obj.transform.position == origin)
            isDraggable = true;

        if (Input.touchCount > 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    obj.transform.position = AdjustSliderRange(touch.position);
                    break;
                case TouchPhase.Moved:
                    obj.transform.position = AdjustSliderRange(touch.position);
                    break;
                case TouchPhase.Ended:
                    break;
            }
        }
    }

    private Vector2 AdjustSliderRange(Vector2 touchPos)
    {
        float clampedX = Mathf.Clamp(touchPos.x, startpoint.x, endpoint.x);
        float clampedY = Mathf.Clamp(touchPos.y, startpoint.y, endpoint.y);
        touchPos = new Vector2(clampedX, clampedY);

        if (freezeX)
            return new Vector3(obj.transform.position.x, touchPos.y, obj.transform.position.z);
        else if (freezeY)
            return new Vector3(touchPos.x, obj.transform.position.y, obj.transform.position.z);
        else
            return new Vector3(touchPos.x, touchPos.y, obj.transform.position.z);
    }

    private void AdjustFillBars()
    {
        int newX = 0;
        if (freezeY)
            newX = (int)Vector2.Distance((Vector2)obj.transform.position, origin);

        switch (placement)
        {
            case Placement.Left:
                leftBar.transform.localScale = new Vector2(newX, SLIDER_WEIGHT);
                break;
            case Placement.Right:
                rightBar.transform.localScale = new Vector2(newX, SLIDER_WEIGHT);
                break;
        }
    }

    private Vector2 GetPlacement()
    {
        switch (placement)
        {
            case Placement.Left:
                leftBar.SetActive(true);
                leftBar.transform.position = startpoint;
                return Vector3.Lerp(startpoint, endpoint, 0f);
            case Placement.Right:
                rightBar.SetActive(true);
                rightBar.transform.position = endpoint;
                return Vector3.Lerp(startpoint, endpoint, 1f);
            case Placement.Top:
                return Vector3.Lerp(startpoint, endpoint, 0f);
            case Placement.Bottom:
                return Vector3.Lerp(startpoint, endpoint, 0f);
            case Placement.Center:
                return Vector3.Lerp(startpoint, endpoint, 0.5f);
            default:
                return Vector2.zero;
        }
    }

    private void ResetFillBars()
    {
        leftBar.transform.localScale = new Vector3(0f, SLIDER_WEIGHT, 1f);
        rightBar.transform.localScale = new Vector3(0f, SLIDER_WEIGHT, 1f);
        leftBar.SetActive(false);
        rightBar.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!isDraggable)
            return;

        StopAllCoroutines();
        Vector3 mousePos;
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        obj.transform.position = AdjustSliderRange(mousePos);
    }
    private void OnMouseDrag()
    {
        if (!isDraggable)
            return;

        Vector3 mousePos;
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        obj.transform.position = AdjustSliderRange(mousePos);
        AdjustFillBars();
    }

    private void OnMouseUp()
    {
        isDraggable = false;
        float time = Vector2.Distance((Vector2)obj.transform.position, origin) / 200f;
        if (time == 0f)
            return;

        StartCoroutine(Ease.TranslateTo(obj, origin, time));
        StartCoroutine(Ease.ScaleTo(leftBar, new Vector2(0f, SLIDER_WEIGHT), time));
    }

    public void ResetSlide()
    {
        StopAllCoroutines();
        StartCoroutine(Ease.ScaleTo(leftBar, new Vector2(0f, SLIDER_WEIGHT), 0.1f));
    }
}
