using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 0.5f;
    public float maxZoom = 15f;

    [Header("Pan Settings")]
    public float panSpeed = 1f;
    public float minX, maxX, minY, maxY;

    private Vector3 lastMousePosition;
    public float CurrentZoom => Camera.main.orthographicSize;

    void Update()
    {
        HandleZoom();
        HandlePan();
        ClampCameraPosition();
    }

    void HandleZoom()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            Camera.main.orthographicSize -= scrollDelta * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }
    }

    void HandlePan()
    {
        if (Input.GetMouseButtonDown(2)) // MMB down
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2)) // MMB pushed
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x * panSpeed * Time.deltaTime, -delta.y * panSpeed * Time.deltaTime, 0f);

            Camera.main.transform.Translate(move, Space.Self);

            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.transform.position = new Vector3(0f, 0f, Camera.main.transform.position.z);
        }
    }

    void ClampCameraPosition()
    {
        Camera cam = Camera.main;

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        Vector3 pos = cam.transform.position;

        pos.x = Mathf.Clamp(pos.x, minX + horzExtent, maxX - horzExtent);
        pos.y = Mathf.Clamp(pos.y, minY + vertExtent, maxY - vertExtent);

        cam.transform.position = pos;
    }
}
