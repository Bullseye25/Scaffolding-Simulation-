using UnityEngine;

[CreateAssetMenu(fileName = "ScaffoldingElement", menuName = "Scaffolding/Element", order = 0)]
public class ScaffoldingElement : ScriptableObject
{
    public string elementName;
    public Sprite icon;
    public GameObject prefab;
}
