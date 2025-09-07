using UnityEngine;
using UnityEngine.UI;

public class PhysicsController : MonoBehaviour
{
    [Header("Physics Settings")]
    public Button enablePhysicsButton;
    public Button disablePhysicsButton;

    protected virtual void Start()
    {
        enablePhysicsButton.onClick.AddListener(EnablePhysicsForAllDraggables);
        disablePhysicsButton.onClick.AddListener(DisablePhysicsForAllDraggables);
    }

    private void EnablePhysicsForAllDraggables()
    {
        Draggable[] allDraggables = FindObjectsByType<Draggable>(FindObjectsSortMode.None);
        foreach (var d in allDraggables)
        {
            d.EnablePhysics();
        }
    }

    private void DisablePhysicsForAllDraggables()
    {
        Draggable[] allDraggables = FindObjectsByType<Draggable>(FindObjectsSortMode.None);
        foreach (var d in allDraggables)
        {
            d.DisablePhysics();
        }
    }
}
