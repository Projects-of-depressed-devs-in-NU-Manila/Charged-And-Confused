using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private bool dragging = false;
    private Rigidbody2D rb;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMouseDown()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);

        dragging = true;
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void OnMouseDrag()
    {
        if (!dragging) return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z) + offset;
    }

    void OnMouseUp()
    {
        dragging = false;
        if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
