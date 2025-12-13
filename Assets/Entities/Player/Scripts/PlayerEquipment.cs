using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public Transform handSlot;
    private GameObject equippedObject;
    public Item equippedItem;
    public Interactor interactor;
    public AudioClip equipSound;

    public void Equip(Item item)
    {
        Unequip();

        if (item == null || item.icon == null) {
            Debug.LogWarning("Item is null!");
            return;
        }

        equippedItem = item;
        interactor.SetEquippedItem(item);

        equippedObject = new GameObject("EquippedItem");
        AudioSource.PlayClipAtPoint(equipSound, transform.position);
        
        SpriteRenderer spriteRenderer = equippedObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.icon;
        
        equippedObject.transform.SetParent(handSlot);
        equippedObject.transform.localPosition = Vector3.zero;
        equippedObject.transform.localRotation = Quaternion.identity;
    }

    public void Unequip()
    {
        if (equippedObject != null) Destroy(equippedObject);
        AudioSource.PlayClipAtPoint(equipSound, transform.position);

        equippedObject = null;
        equippedItem = null;
        
        if (interactor != null)
            interactor.SetEquippedItem(null);
    }
}