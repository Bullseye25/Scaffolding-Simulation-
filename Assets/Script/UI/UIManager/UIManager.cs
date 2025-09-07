using UnityEngine;

public class UIManager : PhysicsController
{
    [Header("Catalog Reference")]
    public ScaffoldingCatalog catalog;

    [Header("UI Setup")]
    public Transform slotParent;   // Parent inside ScrollRect content
    public GameObject slotPrefab;  // Prefab for UI slots

    private ScaffoldingElement currentDragElement;

    protected override void Start()
    {
        base.Start();
        PopulateElements();
    }

    private void PopulateElements()
    {
        foreach (var element in catalog.elements)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotParent);
            var slot = slotGO.GetComponent<ScaffoldingSlot>();
            slot.Setup(element, this);
        }
    }

    public void BeginDragFromUI(ScaffoldingElement element)
    {
        currentDragElement = element;
        if (element != null)
        {
            // Get mouse world position
            Vector3 spawnPos = Vector3.zero;
            Camera cam = Camera.main;
            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
                if (groundPlane.Raycast(ray, out float enter))
                    spawnPos = ray.GetPoint(enter);
            }

            GameObject newObj = Instantiate(element.prefab, spawnPos, Quaternion.identity);

            // Ensure it's draggable
            Draggable d = newObj.GetComponent<Draggable>();
            if (d != null) d.SetKinematic(true);

            // Hand off to DragAndDropHandler immediately
            var dragHandler = FindFirstObjectByType<DNDControls>();
            if (dragHandler != null)
                dragHandler.PickUpFromUI(d);
        }
    }

    public void EndDragFromUI()
    {
        currentDragElement = null;
    }
}