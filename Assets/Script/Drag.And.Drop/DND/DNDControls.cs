using System.Collections.Generic;
using UnityEngine;

#region CONTROLS

public class DNDControls : DNDCore
{
    [Header("Controls")]
    public KeyCode snapKey = KeyCode.LeftShift; // Key to "snap" pieces into the grid
    public KeyCode rotateKey = KeyCode.R;       // Key to rotate pieces

    [Header("Events")]
    public GameObjectHandler OnPickup;   // Fires when a piece is picked up
    public GameObjectHandler OnDragging; // Fires while dragging a piece
    public GameObjectHandler OnDrop;     // Fires when a piece is placed

    private List<GameObject> previewTiles = new List<GameObject>(); // The tiles being previewed for placement
    private bool previewValid = false; // Whether the previewed placement is allowed
    private float previewSupportTopY = 0f; // The height of the support (floor or stack) where the piece will be placed

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        // Split into 3 steps for clarity
        HandlePickup();
        HandleDragging();
        HandleDrop();
    }

    #region Input Handling
    /// <summary>
    /// STEP 1: Try to pick up a draggable piece when the left mouse button is pressed.
    /// </summary>
    protected virtual void HandlePickup()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            GameObject hitGO = GetTopmostHitUnderMouse();
            if (hitGO != null && hitGO.CompareTag(draggableTag))
            {
                selectedDraggable = hitGO.GetComponent<Draggable>();
                if (selectedDraggable != null)
                {
                    // Free up any tiles it was sitting on before
                    if (selectedDraggable.occupiedTiles != null)
                    {
                        foreach (var t in selectedDraggable.occupiedTiles)
                            if (t != null) t.tag = tileEmptyTag;
                        selectedDraggable.occupiedTiles.Clear();
                    }

                    // Let the piece move freely
                    selectedDraggable.SetKinematic(true);

                    // Trigger pickup event
                    OnPickup?.Invoke(selectedDraggable.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// STEP 2: While holding the mouse button, drag the selected piece around.
    /// Supports rotating (R) and snapping (Shift).
    /// </summary>
    protected virtual void HandleDragging()
    {
        if (Input.GetMouseButton(0) && selectedDraggable != null)
        {
            if (Input.GetKeyDown(rotateKey)) // Rotate if "R" pressed
                selectedDraggable.Rotate90();

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Input.GetKey(snapKey)) // If snap key is held
            {
                GameObject baseTile = GetTopmostTileUnderRay(ray);
                if (baseTile != null)
                {
                    bool ok = ComputeSnapPreview(selectedDraggable, baseTile, out List<GameObject> snapTiles, out float supportTopY);
                    if (ok)
                    {
                        previewValid = true;
                        previewTiles = snapTiles;
                        previewSupportTopY = supportTopY;

                        Vector3 center = CalculateCenterOfTiles(snapTiles);
                        float halfH = selectedDraggable.GetHalfHeight();
                        float finalY = (supportTopY > 0f) ? (supportTopY + halfH) : halfH;
                        selectedDraggable.transform.position = new Vector3(center.x, finalY, center.z);

                        if (debugLogs) Debug.Log($"Preview: center {center} supportTopY {supportTopY} placedY {finalY}");
                    }
                    else
                    {
                        previewValid = false;
                        MoveSelectedAlongGround(ray);
                    }
                }
                else
                {
                    previewValid = false;
                    MoveSelectedAlongGround(ray);
                }
            }
            else
            {
                previewValid = false;
                MoveSelectedAlongGround(ray);
            }

            // Trigger dragging event
            OnDragging?.Invoke(selectedDraggable.gameObject);
        }
    }

    /// <summary>
    /// STEP 3: Release the piece when the mouse button is let go.
    /// If snapping was valid, lock it onto the grid. Otherwise, leave it where it is.
    /// </summary>
    protected virtual void HandleDrop()
    {
        if (Input.GetMouseButtonUp(0) && selectedDraggable != null)
        {
            if (previewValid && previewTiles != null && previewTiles.Count > 0)
            {
                // Mark tiles as occupied
                foreach (var t in previewTiles)
                    if (t != null) t.tag = tileOccupiedTag;

                // Remember which tiles this piece belongs to
                selectedDraggable.occupiedTiles = new List<GameObject>(previewTiles);

                // Place piece neatly
                Vector3 center = CalculateCenterOfTiles(previewTiles);
                float halfH = selectedDraggable.GetHalfHeight();
                float finalY = (previewSupportTopY > 0f) ? (previewSupportTopY + halfH) : halfH;
                selectedDraggable.transform.position = new Vector3(center.x, finalY, center.z);
            }

            // Lock piece in place
            selectedDraggable.SetKinematic(true);

            // Trigger drop event
            OnDrop?.Invoke(selectedDraggable.gameObject);

            // Reset preview data
            previewTiles.Clear();
            previewValid = false;
            selectedDraggable = null;
        }
    }
    #endregion

    #region UI Pickup
    /// <summary>
    /// Allows picking up a piece directly from the UI (not from the board).
    /// </summary>
    public void PickUpFromUI(Draggable d)
    {
        if (d == null) return;

        if (d.occupiedTiles != null)
        {
            foreach (var t in d.occupiedTiles)
                if (t != null) t.tag = tileEmptyTag;
            d.occupiedTiles.Clear();
        }

        selectedDraggable = d;
        selectedDraggable.SetKinematic(true);

        // Trigger pickup event for UI spawns too
        OnPickup?.Invoke(d.gameObject);
    }
    #endregion
}
#endregion
