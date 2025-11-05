using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem DraggableItem = dropped.GetComponent<DraggableItem>();
        DraggableItem.parentAfterDrag = transform;
    }
}
