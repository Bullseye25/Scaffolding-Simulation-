using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GridTileEvent : UnityEvent<GameObject> { }

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] protected int width = 8;
    [SerializeField] protected int height = 8;

    [Header("Materials")]
    [SerializeField] protected Material materialA; // First material
    [SerializeField] protected Material materialB; // Second material

    [Header("Events")]
    public UnityEvent OnGridCreateBeginning;
    public UnityEvent OnGridCreateEnd;
    public GridTileEvent OnGridCreateInProcess;

    protected readonly List<GameObject> gridTiles = new List<GameObject>();

    #region Public API

    [ContextMenu("Generate Grid")]
    public virtual void GenerateGrid()
    {
        ClearGrid();

        if (!ValidateMaterials()) return;

        OnGridCreateBeginning?.Invoke();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GameObject tile = CreateTile(x, z);
                ConfigureTile(tile, x, z);
                gridTiles.Add(tile);

                OnGridCreateInProcess?.Invoke(tile);
            }
        }

        OnGridCreateEnd?.Invoke();
    }

    [ContextMenu("Clear Grid")]
    public virtual void ClearGrid()
    {
        if (gridTiles.Count > 0)
        {
            foreach (var tile in gridTiles)
            {
                if (tile != null)
                {
#if UNITY_EDITOR
                    DestroyImmediate(tile);
#else
                    Destroy(tile);
#endif
                }
            }
            gridTiles.Clear();
        }

        // Also ensure no orphan children remain
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }
    }

    #endregion

    #region Protected Helpers

    protected virtual bool ValidateMaterials()
    {
        if (materialA == null || materialB == null)
        {
            Debug.LogError($"{nameof(GridManager)}: Please assign both materials!");
            return false;
        }
        return true;
    }

    protected virtual GameObject CreateTile(int x, int z)
    {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
        tile.name = $"Tile_{x}_{z}";
        tile.tag = "Empty";

        tile.transform.parent = transform;
        tile.transform.position = transform.position + new Vector3(x, 0, z);
        tile.transform.rotation = Quaternion.Euler(90, 0, 0); // Face upward
        tile.transform.localScale = Vector3.one;

        return tile;
    }

    protected virtual void ConfigureTile(GameObject tile, int x, int z)
    {
        // Apply material
        MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = ((x + z) % 2 == 0) ? materialA : materialB;

        // Replace MeshCollider with BoxCollider
        MeshCollider meshCol = tile.GetComponent<MeshCollider>();
        if (meshCol != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(meshCol);
#else
            Destroy(meshCol);
#endif
        }

        BoxCollider boxCol = tile.AddComponent<BoxCollider>();
        boxCol.size = new Vector3(1f, 1f, 0.025f); // flat collider
    }

    #endregion
}
