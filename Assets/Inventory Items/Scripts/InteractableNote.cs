using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNote : MonoBehaviour, IInteractable
{
    public string noteID;
    public float pickupDelay = 0.1f;
    private bool pickedUp = false;
    public AudioClip pickupNoteSound;

    public void Interact(Vector3 pos, Item item)
    {
        if (pickedUp) return;
        AudioSource.PlayClipAtPoint(pickupNoteSound, transform.position);

        pickedUp = true;
        string noteTitle = NoteManager.Instance.GetNoteTitle(noteID);

        NoteManager.Instance.UnlockNote(noteID);
        HUDNotification.Instance.displayMessage($"Picked up note \"{noteTitle}\".");
        gameObject.SetActive(false);
    }
}