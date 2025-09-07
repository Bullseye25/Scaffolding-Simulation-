using UnityEngine;

public class Pan : Orbit
{
    [Header("Pan Settings")]
    public float panSpeed = 1.5f;

    /// <summary> Handles camera panning movement. </summary>
    public virtual void HandlePan()
    {
        float moveX = -Input.GetAxis("Mouse X") * panSpeed;
        float moveY = -Input.GetAxis("Mouse Y") * panSpeed;

        Vector3 right = transform.right;
        Vector3 up = transform.up;

        Vector3 move = right * moveX + up * moveY;

        transform.Translate(move, Space.World);
        pivot += move;
    }
}
