using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : MonoBehaviour, IInteractable
{
    [Header("Item Info")]
    public Item item;
    public int quantity = 1; // should always be 1
    
    [Header("Sprite Settings")]
    public SpriteRenderer spriteRenderer;
    public bool billboard = true; // this allows the item to always face the player as a 2D sprite
    public float rotationOffset = 0f;
    
    [Header("Pickup Settings")]
    public float pickupDelay = 0.2f;
    public float hoverAmplitude = 0.1f; // Gentle floating effect
    public float hoverSpeed = 1f;
    
    private bool canBePickedUp = true;
    private Vector3 startPosition;
    private Transform playerTransform;
    
    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (spriteRenderer == null)
        {
            GameObject child = new GameObject("Sprite");
            child.transform.SetParent(transform);
            child.transform.localPosition = Vector3.zero;
            spriteRenderer = child.AddComponent<SpriteRenderer>();
        }
        
        if (item != null && item.icon != null) spriteRenderer.sprite = item.icon;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);
        
        Collider col = GetComponent<Collider>();
        if (col != null) Destroy(col);
        
        SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.5f;
        
        startPosition = transform.position;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }
    
    void Update()
    {
        if (billboard && playerTransform != null)
        {
            Vector3 lookDirection = transform.position - playerTransform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        
        // Gentle hovering animation
        if (hoverAmplitude > 0)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    
    // i love scaling up
    public void Interact(Vector3 interactorPosition, Item item)
    {
        if (!canBePickedUp) return;
        
        TryPickUp();
    }
    
    private void TryPickUp()
    {
        Inventory playerInventory = FindObjectOfType<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogWarning("No Inventory found in scene!");
            return;
        }
        
        bool wasAdded = playerInventory.AddItem(item, quantity);
        
        if (wasAdded) PickUp();
        else HUDNotification.Instance.displayMessage("Inventory full.");
    }
    
    private void PickUp()
    {
        HUDNotification.Instance.displayMessage("Picked up " + item.itemName + ".");
        
        gameObject.SetActive(false);
        canBePickedUp = false;
        
        // add sound effects and player notifications here
    }
    
    public void DropItem(Vector3 position)
    {
        HUDNotification.Instance.displayMessage("Dropped " + item.itemName + ".");
        transform.position = position;
        startPosition = position;
        gameObject.SetActive(true);
        
        canBePickedUp = false;
        Invoke(nameof(EnablePickup), pickupDelay);
    }
    
    private void EnablePickup()
    {
        canBePickedUp = true;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) spriteRenderer.color = Color.yellow;
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) spriteRenderer.color = Color.white;
    }

    // debugging
    void OnDrawGizmos()
    {
        if (item != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.color = Color.white;
            Gizmos.DrawIcon(transform.position + Vector3.up, "d_Asset Store", true);
        }
    }
}
