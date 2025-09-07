using UnityEngine;

public class Zoom : Pan
{
    public float zoomSpeed = 30f;

    /// <summary> Handles camera zooming. </summary>
    public virtual void HandleZoom()
    {
        float zoomDelta = 0f;

        if (Input.GetMouseButton(1)) // RMB drag
            zoomDelta = Input.GetAxis("Mouse X") * zoomSpeed * 0.1f;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
            zoomDelta += scroll * zoomSpeed;

        if (Mathf.Abs(zoomDelta) > 0.001f)
        {
            Vector3 dir = transform.forward * zoomDelta;
            Vector3 newPos = transform.position + dir;

            float dist = Vector3.Distance(newPos, pivot);
            if (dist > minZoomDistance && dist < maxZoomDistance)
                transform.position = newPos;
        }
    }
}
