using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MouseObserverBevahior : MonoBehaviour
{
    public delegate void OnMouseDragHandler(int button, Vector3 startScreenPos, Vector3 currentScreenPos, float time, bool isOver);
    public delegate void OnMouseReleaseHandler(int button, Vector3 screenPos);
    public delegate void OnMousePressHandler(int button, Vector3 screenPos);
    public delegate void OnMouseClickHandler(int button, Vector3 pressScreenPos, Vector3 releaseScreenPos);

    /// <summary>
    /// Event called during the dragging of a mouse by at least <see cref="minDistDragScreen"/> pixels.
    /// </summary>
    public OnMouseDragHandler OnMouseDrag { get; set; }

    /// <summary>
    /// Event called when the mouse has been released
    /// </summary>
    public OnMouseReleaseHandler OnMouseRelease { get; set; }

    /// <summary>
    /// Event called when the mouse has been pressed
    /// </summary>
    public OnMousePressHandler OnMousePress { get; set; }

    /// <summary>
    /// Event called when the mouse has been pressed and released
    /// </summary>
    public OnMouseClickHandler OnMouseClick { get; set; }

    /// <summary>
    /// Minimum distance between mouse press and release location in screen-space for the <see cref="OnMouseDragEnd"/> and <see cref="OnMouseDrag"/> event to be invoked.
    /// </summary>
    public float minDistDragScreen = 10f;
    /// <summary>
    /// Mouse buttons being listened on. Only these buttons will invoke events within this behavior.
    /// </summary>
    public int[] mouseButtonsWatching = new int[] { 0, 1, 2 };
    
    private Vector3[] lastMouseDownPos;
    private bool[] lastMouseDownState;
    private float[] lastMouseDownTime;
    private bool[] lastMouseWasDragging;

    private float minDistDragScreenSqr;

    // Start is called before the first frame update
    void Start()
    {
        lastMouseDownState = new bool[mouseButtonsWatching.Length];
        lastMouseDownPos = new Vector3[mouseButtonsWatching.Length];
        lastMouseDownTime = new float[mouseButtonsWatching.Length];
        lastMouseWasDragging = new bool[mouseButtonsWatching.Length];

        minDistDragScreenSqr = minDistDragScreen * minDistDragScreen;


        for(int i = 0; i < lastMouseDownState.Length; i++)
        {
            lastMouseDownState[i] = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < mouseButtonsWatching.Length; i++)
        {
            UpdateButton(mouseButtonsWatching[i], i);
        }
    }

    private void UpdateButton(int button, int infoIndex)
    {
        bool wasPressed = lastMouseDownState[infoIndex];
        bool isPressed = Input.GetMouseButton(button);

        if(isPressed != wasPressed)
        {
            //Debug.Log("MOUSE " + wasPressed + " -> " + isPressed);
            if (isPressed)
            {
                // click started
                lastMouseDownPos[infoIndex] = Input.mousePosition;
                lastMouseDownTime[infoIndex] = Time.realtimeSinceStartup;

                OnMousePress?.Invoke(button, Input.mousePosition);
            } else
            {
                // click ended
                OnMouseRelease?.Invoke(button, Input.mousePosition);

                Vector3 lastPosition = lastMouseDownPos[infoIndex];
                float dragDist = (Input.mousePosition - lastPosition).sqrMagnitude;

                if (lastMouseWasDragging[infoIndex] || dragDist >= minDistDragScreenSqr)
                {
                    // drag
                    //Debug.Log("MOUSE DRAG from " + lastPosition + " to " + Input.mousePosition + " dist " + String.Format("{0,2}", dragDist));
                    OnMouseDrag?.Invoke(button, lastMouseDownPos[infoIndex], Input.mousePosition, Time.realtimeSinceStartup - lastMouseDownTime[infoIndex], true);
                } else
                {
                    OnMouseClick?.Invoke(button, lastMouseDownPos[infoIndex], Input.mousePosition);
                }
                
            }
            lastMouseDownState[infoIndex] = isPressed;
        }

        if (!isPressed)
        {
            lastMouseWasDragging[infoIndex] = false;
        }

        if (isPressed && wasPressed)
        {
            // potential dragging event

            bool isDrag = false;

            if (lastMouseWasDragging[infoIndex])
            {
                isDrag = true;
            }
            else
            {
                Vector3 startPosition = lastMouseDownPos[infoIndex];
                
                if((startPosition - Input.mousePosition).sqrMagnitude >= minDistDragScreenSqr)
                {
                    isDrag = true;
                }
            }

            if(isDrag)
            {
                lastMouseWasDragging[infoIndex] = true;

                OnMouseDrag?.Invoke(button, lastMouseDownPos[infoIndex], Input.mousePosition, Time.realtimeSinceStartup - lastMouseDownTime[infoIndex], false);
            }
        }
    }
}
