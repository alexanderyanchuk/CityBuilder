using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool isDragging;
    private Vector3 dragOrigin;
    private Vector3 dragStartPosition;

    private new Camera camera;

    [SerializeField] private float moveSpeedX = 1.0f;
    [SerializeField] private float moveSpeedY = 1.0f;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragOrigin = camera.ScreenToViewportPoint(Input.mousePosition);
            dragStartPosition = transform.position;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 dragDelta = camera.ScreenToViewportPoint(Input.mousePosition) - dragOrigin;

            Vector3 right = transform.right;
            right.y = 0;
            right.Normalize();

            Vector3 up = transform.up;
            up.y = 0;
            up.Normalize();

            transform.position = dragStartPosition - (moveSpeedX * dragDelta.x * right + moveSpeedY * dragDelta.y * up);
        }
    }
}
