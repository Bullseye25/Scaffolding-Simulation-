using UnityEngine;

[CreateAssetMenu(fileName = "ScaffoldingCatalog", menuName = "Scaffolding/Catalog", order = 1)]
public class ScaffoldingCatalog : ScriptableObject
{
    public ScaffoldingElement[] elements;
}
