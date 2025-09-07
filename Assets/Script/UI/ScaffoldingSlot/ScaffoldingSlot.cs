using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ScaffoldingSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;

    private ScaffoldingElement elementData;
    private UIManager uiManager;

    public void Setup(ScaffoldingElement data, UIManager manager)
    {
        elementData = data;
        uiManager = manager;

        if (iconImage != null) iconImage.sprite = data.icon;
        if (nameText != null) nameText.text = data.elementName;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (elementData != null)
            uiManager.BeginDragFromUI(elementData);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        uiManager.EndDragFromUI();
    }
}
