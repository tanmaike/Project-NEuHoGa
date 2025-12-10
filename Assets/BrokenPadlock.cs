using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPadlock : MonoBehaviour, IInteractable
{
    public Item requiredItem;
    public GameObject actualPadlock;

    public void Interact(Vector3 interactorPosition, Item item)
    {
        Inventory inv = FindObjectOfType<Inventory>();
        
        if (item != requiredItem)
        {
            HUDNotification.Instance.displayMessage("This lock is broken. It requires another part to work.");
            return;
        }
        else
        {
            inv.RemoveItem(item);
            HUDNotification.Instance.displayMessage("The padlock was fixed.");
            actualPadlock.gameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}
