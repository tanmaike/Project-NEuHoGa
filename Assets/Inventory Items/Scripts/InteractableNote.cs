using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNote : MonoBehaviour, IInteractable
{
    public string noteID; // "PlateNote" or "CouchNote"
    public float pickupDelay = 0.1f;
    private bool pickedUp = false;

    public void Interact(Vector3 pos, Item item)
    {
        if (pickedUp) return;

        pickedUp = true;

        NoteManager.Instance.UnlockNote(noteID);
        HUDNotification.Instance.displayMessage("Picked up note.");

        gameObject.SetActive(false);
    }
}