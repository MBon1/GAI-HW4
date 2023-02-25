using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera mainCamera;
    float defaultFieldOfView;
    Vector3 defaultPosition;

    Vector2 lastHoldtimes = Vector2.zero;
    [SerializeField] float inputDelay = 0.075f;

    [SerializeField] Vector2 fovRange = new Vector2(5,25);

    private void Awake()
    {
        mainCamera = this.gameObject.GetComponent<Camera>();
        defaultFieldOfView = mainCamera.orthographicSize;
        defaultPosition = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Scroll Wheel to zoom in & out 
        // Zoom focuses in on screen center
        // Arrow keys to change camera position
        ReadInput();
    }

    private void ReadInput()
    {
        // Bring to original position
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.transform.position = defaultPosition;
            mainCamera.orthographicSize = defaultFieldOfView;
            return;
        }

        // Zoom In/Out
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0)
        {
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - scroll, fovRange.x, fovRange.y);
            return;
        }


        Vector3 move = mainCamera.transform.position;
        if (Time.time - lastHoldtimes.x >= inputDelay)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                move.x -= 1;
                lastHoldtimes.x = Time.time;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                move.x += 1;
                lastHoldtimes.x = Time.time;
            }
        }

        if (Time.time - lastHoldtimes.y >= inputDelay)
        {
            if (Input.GetKey(KeyCode.DownArrow) ||
                Input.mouseScrollDelta.y < 0)
            {
                move.y -= 1;
                lastHoldtimes.y = Time.time;
            }

            if (Input.GetKey(KeyCode.UpArrow) ||
                Input.mouseScrollDelta.y > 0)
            {
                move.y += 1;
                lastHoldtimes.y = Time.time;
            }

            if (move.y > 0)
            {
                move.y = 0;
            }
        }

        this.gameObject.transform.position = move;
    }
}
