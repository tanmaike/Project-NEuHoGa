using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable {
    public void Interact(Vector3 interactorPosition, Item item);
}
public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public KeyCode interactKey = KeyCode.E;

    public Item equippedItem;

    void Update() {
        if (Input.GetKeyDown(interactKey)) {
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitinfo, InteractRange)) {
                if (hitinfo.collider.gameObject.TryGetComponent(out IInteractable interactObj)) {
                    interactObj.Interact(InteractorSource.position, equippedItem);
                }
            }
        }
    }

    public void SetEquippedItem(Item item) { equippedItem = item; }
}
