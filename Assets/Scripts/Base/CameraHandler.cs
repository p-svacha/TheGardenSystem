using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This is the default controls for handling camera movement on the world map.
/// Attach this script to the main camera.
/// </summary>
public class CameraHandler : MonoBehaviour
{
    public static CameraHandler Instance;
    private Camera Camera;

    protected static float ZOOM_SPEED = 0.8f; // Mouse Wheel Speed
    protected static float DRAG_SPEED = 0.025f; // Middle Mouse Drag Speed
    protected static float PAN_SPEED = 10f; // WASD Speed
    protected static float MIN_CAMERA_SIZE = 1f;
    protected static float MAX_CAMERA_SIZE = 100f;
    protected bool IsLeftMouseDown;
    protected bool IsRightMouseDown;
    protected bool IsMouseWheelDown;

    // Bounds
    protected float MinX, MinY, MaxX, MaxY;

    public float ZoomLevel => Camera.orthographicSize;
    public event System.Action OnCameraChanged;

    public void FocusPosition(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Camera = GetComponent<Camera>();
        _Singleton = Camera.main.GetComponent<CameraHandler>();

        // Initial zoom
        int pixelsPerUnit = 128;
        Camera.orthographicSize = Screen.height / (pixelsPerUnit * 2f);
    }


    public virtual void Update()
    {
        // Scroll
        /*
        if (Input.mouseScrollDelta.y != 0)
        {
            Camera.orthographicSize += -Input.mouseScrollDelta.y * ZOOM_SPEED;

            // Zoom Boundaries
            if (Camera.orthographicSize < MIN_CAMERA_SIZE) Camera.orthographicSize = MIN_CAMERA_SIZE;
            if (Camera.orthographicSize > MAX_CAMERA_SIZE) Camera.orthographicSize = MAX_CAMERA_SIZE;

            OnCameraChanged?.Invoke();
        }
        */
        if (Input.mouseScrollDelta.y < 0)
        {
            if (Camera.orthographicSize < 6)
            {
                Camera.orthographicSize *= 2;
            }
        }
        if (Input.mouseScrollDelta.y > 0)
        {
            if (Camera.orthographicSize > 2)
            {
                Camera.orthographicSize /= 2;
            }
        }

        // Dragging with right/middle mouse button
        if (Input.GetKeyDown(KeyCode.Mouse2)) IsMouseWheelDown = true;
        if (Input.GetKeyUp(KeyCode.Mouse2)) IsMouseWheelDown = false;
        if (Input.GetKeyDown(KeyCode.Mouse1)) IsRightMouseDown = true;
        if (Input.GetKeyUp(KeyCode.Mouse1)) IsRightMouseDown = false;
        if (IsMouseWheelDown)
        {
            float speed = DRAG_SPEED * Camera.orthographicSize;
            transform.position += new Vector3(-Input.GetAxis("Mouse X") * speed, -Input.GetAxis("Mouse Y") * speed, 0f);

            OnCameraChanged?.Invoke();
        }

        // Panning with WASD
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0f, PAN_SPEED * Time.deltaTime, 0f);
            OnCameraChanged?.Invoke();
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-PAN_SPEED * Time.deltaTime, 0f, 0f);
            OnCameraChanged?.Invoke();
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0f, -PAN_SPEED * Time.deltaTime, 0f);
            OnCameraChanged?.Invoke();
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(PAN_SPEED * Time.deltaTime, 0f, 0f);
            OnCameraChanged?.Invoke();
        }

        // Drag triggers
        if (Input.GetKeyDown(KeyCode.Mouse0) && !IsLeftMouseDown)
        {
            IsLeftMouseDown = true;
            OnLeftMouseDragStart();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && IsLeftMouseDown)
        {
            IsLeftMouseDown = false;
            OnLeftMouseDragEnd();
        }

        // Bounds
        if (transform.position.x < MinX) transform.position = new Vector3(MinX, transform.position.y, transform.position.z);
        if (transform.position.x > MaxX) transform.position = new Vector3(MaxX, transform.position.y, transform.position.z);
        if (transform.position.y < MinY) transform.position = new Vector3(transform.position.x, MinY, transform.position.z);
        if (transform.position.y > MaxY) transform.position = new Vector3(transform.position.x, MaxY, transform.position.z);
    }

    private static CameraHandler _Singleton;
    public static CameraHandler Singleton => _Singleton;

    public void SetBounds(float minX, float minY, float maxX, float maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    #region Triggers

    protected virtual void OnLeftMouseDragStart() { }

    protected virtual void OnLeftMouseDragEnd() { }

    #endregion
}
