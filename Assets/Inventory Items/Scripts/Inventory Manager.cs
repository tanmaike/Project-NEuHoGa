using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public GameObject Notes;
    public GameObject Inventory;
    public KeyCode switchRightKey = KeyCode.Q;
    public KeyCode switchLeftKey = KeyCode.E;
    public KeyCode InventoryKey = KeyCode.Escape;
    bool IsOpen = false;

    private GameObject CurrentMenu;
    private GameObject NextMenu;
    private GameObject Holding;
    public GameObject Hint;
    public AudioSource openNotes, openInventory;

    // Start is called before the first frame update
    void Awake()
    {
        Hint.SetActive(false);
        playerMovement = FindObjectOfType<PlayerMovement>();
        CurrentMenu = Inventory;
        NextMenu = Notes;
        NextMenu.SetActive(false);
        CurrentMenu.SetActive(false);
        Debug.Log("working");
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOpen == true) {
            if (Input.GetKeyDown(switchRightKey) || Input.GetKeyDown(switchLeftKey))
            {
                CurrentMenu.SetActive(false);
                NextMenu.SetActive(true);
                Holding = CurrentMenu;
                CurrentMenu = NextMenu;
                NextMenu = Holding;
                openInventory.Play();
            }
        }

        if (Input.GetKeyDown(InventoryKey) && !playerMovement.IsJumping())
        {
            if (IsOpen == false)
            {
                openInventory.Play();
                Hint.SetActive(true);
                CurrentMenu.SetActive(true);
                IsOpen = true;
                playerMovement.SetInventoryState(true);
            }
            else 
            {
                openInventory.Play();
                Hint.SetActive(false);
                CurrentMenu = Inventory;
                NextMenu = Notes;
                NextMenu.SetActive(false);
                CurrentMenu.SetActive(false);
                IsOpen = false;
                playerMovement.SetInventoryState(false);
            }
        }
    }
}
