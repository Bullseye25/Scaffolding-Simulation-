using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Draggable : MonoBehaviour
{
    [Header("Visual / Highlight")]
    public Material highlightMaterial;

    [Header("Shape footprint (local grid offsets)")]
    public List<Vector2Int> footprintOffsets = new List<Vector2Int>() { Vector2Int.zero };

    [Header("Stacking")]
    public bool isStackable = false;

    [Space]
    [SerializeField] private bool requiresPatch;
    [SerializeField] private GameObject[] patchs;
    [SerializeField] private List<float> lockers = new List<float>();
    [SerializeField] private float serial;

    [HideInInspector] public List<GameObject> occupiedTiles = new List<GameObject>();

    [SerializeField] private int currentRotation = 0; // 0/90/180/270
    private MeshRenderer[] meshRenderers;
    private Material[] originalMaterials;
    private Rigidbody rb;

    private void Awake()
    {
        // Grab all renderers in self + children
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        if (meshRenderers != null && meshRenderers.Length > 0)
        {
            originalMaterials = new Material[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                originalMaterials[i] = meshRenderers[i].sharedMaterial;
            }
        }

        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true; // keep physics off until user enables it

        if (requiresPatch == true)
        {
            for (int index = 0; index < patchs.Length; index++)
            {
                GameObject patch = patchs[index];
                var localPosition = patch.transform.localPosition;

                lockers.Add(localPosition.z);
            }
        }
    }

    private void OnMouseEnter()
    {
        if (meshRenderers == null || highlightMaterial == null) return;

        foreach (var mr in meshRenderers)
        {
            if (mr != null)
                mr.sharedMaterial = highlightMaterial;
        }
    }

    private void OnMouseExit()
    {
        if (meshRenderers == null || originalMaterials == null) return;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i] != null && originalMaterials[i] != null)
                meshRenderers[i].sharedMaterial = originalMaterials[i];
        }
    }

    public void Rotate90()
    {
        currentRotation = (currentRotation + 90) % 360;
        transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);

        //Patch
        if (requiresPatch == true)
            for (int index = 0; index < patchs.Length; index++)
            {
                var sequence = currentRotation == 90 || currentRotation == 270;

                GameObject patch = patchs[index];
                var localPosition = patch.transform.localPosition;

                var z = lockers[index] + (sequence == true ? serial : 0);

                patchs[index].transform.localPosition =
                    new Vector3(localPosition.x, localPosition.y, z);

                var boxCollider = GetComponent<BoxCollider>();
                var center = boxCollider.center;
                boxCollider.center = new Vector3(center.x, center.y, (sequence == true ? 0.25f : -0.25f));
            }
    }

    /// <summary>
    /// Returns rotated offsets (Vector2Int x,z) according to current orientation (90deg steps).
    /// </summary>
    public List<Vector2Int> GetRotatedOffsets()
    {
        var rotated = new List<Vector2Int>(footprintOffsets.Count);
        foreach (var off in footprintOffsets)
        {
            Vector2Int r;
            switch (currentRotation)
            {
                case 90: r = new Vector2Int(-off.y, off.x); break;
                case 180: r = new Vector2Int(-off.x, -off.y); break;
                case 270: r = new Vector2Int(off.y, -off.x); break;
                default: r = off; break;
            }
            rotated.Add(r);
        }
        return rotated;
    }

    /// <summary>
    /// Give a rough half-height for placement math (prefers collider bounds if present).
    /// </summary>
    public float GetHalfHeight()
    {
        var col = GetComponent<Collider>();
        if (col != null) return col.bounds.size.y * 0.5f;
        return transform.localScale.y * 0.5f;
    }

    public float GetTopY()
    {
        var col = GetComponent<Collider>();
        if (col != null) return col.bounds.max.y;
        return transform.position.y + GetHalfHeight();
    }

    public void SetKinematic(bool isKinematic)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = isKinematic;
    }

    public void EnablePhysics()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    public void DisablePhysics()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;   // turn off gravity
            rb.isKinematic = true;   // freeze rigidbody so it can only be moved by code
        }
    }

}
