using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDesc : MonoBehaviour, IInteractable
{
    public string objectID;
    public void Interact(Vector3 pos, Item item)
    {
        string flairText = ObjDescManager.Instance.GetDesc(objectID);
        HUDNotification.Instance.displayMessage(flairText);
    }
}
