using System;
using System.Collections.Generic;
using UnityEngine;

#region CORE
public class DNDCore : MonoBehaviour
{
    [Header("Tags (create these in Project Settings -> Tags)")]
    public string draggableTag = "Draggable";   // The label used to mark objects that can be moved
    public string tileEmptyTag = "Empty";       // The label for tiles that are free
    public string tileOccupiedTag = "Occupied"; // The label for tiles that are already taken

    [Header("Debug")]
    public bool debugLogs = false; // If true, extra messages will be shown in the console to help understand what’s happening

    protected Camera mainCamera; // The main camera we use to detect clicks and positions
    protected Draggable selectedDraggable; // The current object being moved
  
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // An invisible ground surface for free movement

    protected virtual void Start()
    {
        // At the beginning, find the main camera.
        // If no camera is marked as "MainCamera", show a warning.
        mainCamera = Camera.main;
        if (mainCamera == null) Debug.LogWarning("DNDCore: No camera tagged MainCamera found.");
    }

    #region Movement
    /// <summary>
    /// This makes the selected object follow the mouse along the ground,
    /// so the player can move it freely without snapping to tiles.
    /// </summary>
    protected void MoveSelectedAlongGround(Ray ray)
    {
        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 p = ray.GetPoint(enter);
            float halfH = selectedDraggable.GetHalfHeight();
            selectedDraggable.transform.position = new Vector3(p.x, halfH, p.z);
        }
    }
    #endregion

    #region Snap & Placement
    /// <summary>
    /// This checks if the object can be placed neatly on tiles.
    /// It makes a "preview" to show the player where the piece would land,
    /// and also checks if the piece has enough support (ground or other blocks below).
    /// </summary>
    protected bool ComputeSnapPreview(Draggable draggable, GameObject baseTile, out List<GameObject> outSnapTiles, out float outSupportTopY)
    {
        outSnapTiles = new List<GameObject>();
        outSupportTopY = 0f;

        Vector3 basePos = baseTile.transform.position;
        List<Vector2Int> offsets = draggable.GetRotatedOffsets();

        bool anySupportFound = false;
        float highestSupportTop = 0f;

        foreach (var off in offsets)
        {
            Vector3 checkPos = basePos + new Vector3(off.x, 0f, off.y);

            GameObject tile = FindTileAtPosition(checkPos);
            if (tile == null)
            {
                // If even one tile is missing, the placement is not allowed
                if (debugLogs) Debug.Log($"ComputeSnapPreview: missing tile at {checkPos}");
                return false;
            }

            if (!outSnapTiles.Contains(tile)) outSnapTiles.Add(tile);

            // If the piece cannot be stacked, make sure the tile is empty
            if (!draggable.isStackable)
            {
                if (tile.CompareTag(tileOccupiedTag))
                {
                    if (debugLogs) Debug.Log($"ComputeSnapPreview: tile already occupied at {tile.transform.position}");
                    return false;
                }
                continue;
            }

            // If the piece can be stacked, check if something solid exists below the tile
            Ray downRay = new Ray(tile.transform.position + Vector3.up * 5f, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(downRay, 10f);
            if (hits != null && hits.Length > 0)
            {
                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                foreach (var h in hits)
                {
                    if (h.collider == null) continue;
                    if (h.collider.gameObject == tile) continue;
                    if (selectedDraggable != null && h.collider.gameObject == selectedDraggable.gameObject) continue;

                    if (h.collider.gameObject.CompareTag(draggableTag))
                    {
                        anySupportFound = true;
                        float topY = h.collider.bounds.max.y;
                        if (topY > highestSupportTop) highestSupportTop = topY;
                        break;
                    }
                }
            }
        }

        outSupportTopY = anySupportFound ? highestSupportTop : 0f;
        if (debugLogs) Debug.Log($"ComputeSnapPreview: tiles={outSnapTiles.Count} anySupport={anySupportFound} supportTop={outSupportTopY}");
        return true;
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Finds a tile at a given position on the board.
    /// </summary>
    protected GameObject FindTileAtPosition(Vector3 worldPos)
    {
        Collider[] cols = Physics.OverlapSphere(worldPos, 0.35f);
        if (cols == null || cols.Length == 0) return null;
        foreach (var c in cols)
        {
            if (c.gameObject.CompareTag(tileEmptyTag) || c.gameObject.CompareTag(tileOccupiedTag))
                return c.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Calculates the center point of a group of tiles,
    /// so the object can be placed exactly in the middle.
    /// </summary>
    protected Vector3 CalculateCenterOfTiles(List<GameObject> tiles)
    {
        if (tiles == null || tiles.Count == 0) return Vector3.zero;
        Vector3 sum = Vector3.zero;
        foreach (var t in tiles) sum += t.transform.position;
        return sum / tiles.Count;
    }

    /// <summary>
    /// Finds the very first object the mouse is pointing at.
    /// </summary>
    protected GameObject GetTopmostHitUnderMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 200f);
        if (hits == null || hits.Length == 0) return null;
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        return hits[0].collider?.gameObject;
    }

    /// <summary>
    /// Finds the tile the mouse is pointing at.
    /// Used for snapping objects neatly onto the grid.
    /// </summary>
    protected GameObject GetTopmostTileUnderRay(Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, 200f);
        if (hits == null || hits.Length == 0) return null;
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (var h in hits)
        {
            if (h.collider == null) continue;
            if (selectedDraggable != null && h.collider.gameObject == selectedDraggable.gameObject) continue;
            GameObject go = h.collider.gameObject;
            if (go.CompareTag(tileEmptyTag) || go.CompareTag(tileOccupiedTag)) return go;
        }
        return null;
    }
    #endregion
}
#endregion
