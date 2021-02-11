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
    private const float DRAG_TIME_INTERVAL = 3f;

    private Vector3 positionPrev;
    private GameObject obj;

    [Header("Slider"), Space(8)]
    [SerializeField] private Vector2 origin;
    [SerializeField] private Placement placement;
    private Vector2 startpoint;
    private Vector2 endpoint;
    [SerializeField] private GameObject sliderBar;
    [Space(8)]
    [SerializeField, Range(0f,720f)] private float barLength;
    [SerializeField] private GameObject leftBar;
    [SerializeField] private GameObject rightBar;
    [SerializeField] private GameObject upBar;
    [SerializeField] private GameObject downBar;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    private GameObject mainBar;
    [SerializeField] private bool isDraggable;
    private bool onDrag = false;

    [Header("Drag Timer"), Space(8)]
    [SerializeField] private float dragTime = 4f;

    [Header("Freeze Positions"), Space(8)]
    [SerializeField] private bool freezeX = false;
    [SerializeField] private bool freezeY = false;

    private void Awake()
    {
        SetupSlider();

        ResetFillBars();
        obj.transform.position = GetPlacement();
        origin = obj.transform.position;

    }

    private void OnValidate()
    {
        SetupSlider();
        leftPoint.position = new Vector3(startpoint.x, startpoint.y, -2f);
        rightPoint.position = new Vector3(endpoint.x, endpoint.y, -2f);
        ResetFillBars();
        obj.transform.position = GetPlacement();
        origin = obj.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.Instance.GameOver())
            return;

        if (positionPrev != obj.transform.position)
            positionPrev = obj.transform.position;

        if (AtOrigin())
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

        DragTimer();
    }

    private Vector2 AdjustSliderRange(Vector2 touchPos)
    {
        float clampedX = Mathf.Clamp(touchPos.x, startpoint.x, endpoint.x);
        float clampedY = Mathf.Clamp(touchPos.y, startpoint.y, endpoint.y);
        touchPos = new Vector2(clampedX, clampedY);

        if (freezeX)
            return new Vector3(obj.transform.position.x, touchPos.y, -4f);
        else if (freezeY)
            return new Vector3(touchPos.x, obj.transform.position.y, -4f);
        else
            return new Vector3(touchPos.x, touchPos.y, -4f);
    }

    private void AdjustFillBars()
    {
        int newX = 0;
        int newY = 0;

        if (freezeX)
            newY = (int)Vector2.Distance((Vector2)obj.transform.position, origin);
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
            case Placement.Top:
                upBar.transform.localScale = new Vector2(SLIDER_WEIGHT, newY);
                break;
            case Placement.Bottom:
                downBar.transform.localScale = new Vector2(SLIDER_WEIGHT, newY);
                break;
        }
    }

    private Vector2 GetPlacement()
    {
        switch (placement)
        {
            case Placement.Left:
                leftBar.SetActive(true);
                leftBar.transform.position = new Vector3(startpoint.x, startpoint.y, -1f);
                mainBar = leftBar;
                return Vector3.Lerp(startpoint, endpoint, 0f);
            case Placement.Right:
                rightBar.SetActive(true);
                rightBar.transform.position = new Vector3(endpoint.x, endpoint.y, -1f);
                mainBar = rightBar;
                return Vector3.Lerp(startpoint, endpoint, 1f);
            case Placement.Top:
                upBar.SetActive(true);
                upBar.transform.position = new Vector3(endpoint.x, endpoint.y, -1f);
                mainBar = upBar;
                return Vector3.Lerp(startpoint, endpoint, 1f);
            case Placement.Bottom:
                downBar.SetActive(true);
                downBar.transform.position = new Vector3(startpoint.x, startpoint.y, -1f);
                mainBar = downBar;
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

        upBar.transform.localScale = new Vector3(SLIDER_WEIGHT, 0f, 1f);
        downBar.transform.localScale = new Vector3(SLIDER_WEIGHT, 0f, 1f);
        upBar.SetActive(false);
        downBar.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!isDraggable)
            return;

        StopAllCoroutines();
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        obj.transform.position = AdjustSliderRange(mousePos);
        onDrag = true;
    }
    private void OnMouseDrag()
    {
        if (!isDraggable)
            return;

        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        obj.transform.position = AdjustSliderRange(mousePos);
        AdjustFillBars();
    }

    private void OnMouseUp()
    {
        ReleaseDrag();
    }

    /// <summary>
    /// Sets the drag timer. Once drag time reaches 0, it will automatically release the slider button.
    /// </summary>
    private void DragTimer()
    {
        if (dragTime == 0f && !onDrag)
            dragTime = DRAG_TIME_INTERVAL;
        if (!onDrag)
            return;


        if (dragTime > 0f)
        {
            dragTime -= Time.deltaTime;
            dragTime = Mathf.Max(0f, dragTime);
        }
        else
            ReleaseDrag();

    }

    /// <summary>
    /// Lets go of the slider button and move back to its origin.
    /// </summary>
    private void ReleaseDrag()
    {
        dragTime = 0f;
        onDrag = false;
        isDraggable = false;
        float time = Vector2.Distance((Vector2)obj.transform.position, origin) / 200f;
        if (time == 0f)
            return;

        StartCoroutine(Ease.TranslateTo(obj, origin, time));
        if (placement == Placement.Left || placement == Placement.Right)
            StartCoroutine(Ease.ScaleTo(mainBar, new Vector2(0f, SLIDER_WEIGHT), time));
        else if (placement == Placement.Top || placement == Placement.Bottom)
            StartCoroutine(Ease.ScaleTo(mainBar, new Vector2(SLIDER_WEIGHT, 0f), time));
    }

    /// <summary>
    /// Checks if slider button is at the origin.
    /// </summary>
    /// <returns>TRUE if slider button is at the origin. Otherwise, FALSE.</returns>
    private bool AtOrigin() => (Vector2)obj.transform.position == origin;

    public void ResetSlide()
    {
        StopAllCoroutines();
        if (placement == Placement.Left || placement == Placement.Right)
            StartCoroutine(Ease.ScaleTo(mainBar, new Vector2(0f, SLIDER_WEIGHT), 0.1f));
        else if (placement == Placement.Top || placement == Placement.Bottom)
            StartCoroutine(Ease.ScaleTo(mainBar, new Vector2(SLIDER_WEIGHT, 0f), 0.1f));
    }

    private void SetupSlider()
    {
        obj = this.gameObject;

        if (freezeX)
        {
            sliderBar.transform.localScale = new Vector2(SLIDER_WEIGHT, barLength);
            float topY = sliderBar.transform.position.y + (barLength / 2);
            float bottomY = sliderBar.transform.position.y - (barLength / 2);

            startpoint = new Vector3(sliderBar.transform.position.x, bottomY, -2f);
            endpoint = new Vector3(sliderBar.transform.position.x, topY, -2f);
        }
        else if (freezeY)
        {
            sliderBar.transform.localScale = new Vector2(barLength, SLIDER_WEIGHT);
            float leftX = sliderBar.transform.position.x - (barLength / 2);
            float rightX = sliderBar.transform.position.x + (barLength / 2);

            startpoint = new Vector3(leftX, sliderBar.transform.position.y, -2f);
            endpoint = new Vector3(rightX, sliderBar.transform.position.y, -2f);
        }
    }
}
