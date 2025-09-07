using UnityEngine;

public class CameraControl : Zoom
{
    [Header("Key Bindings")]
    public KeyCode orbitKey = KeyCode.Mouse0;       // Orbit with LMB
    public KeyCode zoomKey = KeyCode.Mouse1;        // Zoom with RMB
    public KeyCode panKey = KeyCode.Mouse2;         // Pan with MMB
    public KeyCode altKey = KeyCode.LeftAlt;        // Modifier
    public KeyCode ctrlKey = KeyCode.LeftControl;   // Modifier
    public KeyCode focusKey = KeyCode.F;            // Focus pivot

    private CameraAction camAction = CameraAction.None;

    protected override void Start()
    {
        base.Start();
        Focus();
    }

    void Update()
    {
        camAction = GetMouseAction();

        switch (camAction)
        {
            case CameraAction.Orbit:
                HandleOrbit();
                break;

            case CameraAction.Zoom:
                HandleZoom();
                break;

            case CameraAction.Pan:
                HandlePan();
                break;

            case CameraAction.None:
                break;
        }

        HandleKeyboardInput();
    }

    protected virtual CameraAction GetMouseAction()
    {
        bool altHeld = Input.GetKey(altKey);
        bool ctrlHeld = Input.GetKey(ctrlKey);

        if (altHeld && Input.GetKey(orbitKey) && !ctrlHeld)
            return CameraAction.Orbit;

        if (altHeld && Input.GetKey(zoomKey))
            return CameraAction.Zoom;

        if ((altHeld && ctrlHeld && Input.GetKey(orbitKey)) || Input.GetKey(panKey))
            return CameraAction.Pan;

        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01f)
            return CameraAction.Zoom;

        return CameraAction.None;
    }

    protected virtual void HandleKeyboardInput()
    {
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastMoveMultiplier : 1f);

        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        if (Input.GetKey(KeyCode.E)) move += transform.up;
        if (Input.GetKey(KeyCode.Q)) move -= transform.up;

        if (move != Vector3.zero)
        {
            transform.position += move.normalized * speed * Time.deltaTime;
            pivot += move.normalized * speed * Time.deltaTime;
        }

        // Focus pivot (like "F" in scene view)
        if (Input.GetKeyDown(focusKey))
        {
            Focus();
        }
    }

    private void Focus()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            pivot = hit.point;
        }
    }
}