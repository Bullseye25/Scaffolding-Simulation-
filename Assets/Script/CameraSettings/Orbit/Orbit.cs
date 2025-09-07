using UnityEngine;

public enum CameraAction
{
    None,
    Orbit,
    Zoom,
    Pan
}

[RequireComponent(typeof(Camera))]
public class Orbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    public float rotationSpeed = 5f;

    protected Camera cam;
    protected Vector3 pivot = Vector3.zero;

    protected virtual void Start()
    {
        cam = GetComponent<Camera>();
        pivot = Vector3.zero;
    }

    /// <summary> Handles orbiting rotation around the pivot. </summary>
    public virtual void HandleOrbit()
    {
        float rotX = Input.GetAxis("Mouse X") * rotationSpeed;
        float rotY = -Input.GetAxis("Mouse Y") * rotationSpeed;

        transform.RotateAround(pivot, Vector3.up, rotX);
        transform.RotateAround(pivot, transform.right, rotY);
    }
}
