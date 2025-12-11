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


    // Start is called before the first frame update
    void Awake()
    {
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
            }
        }

        if (Input.GetKeyDown(InventoryKey) && !playerMovement.IsJumping())
        {
            if (IsOpen == false)
            {
                CurrentMenu.SetActive(true);
                IsOpen = true;
                playerMovement.SetInventoryState(true);
                Debug.Log("esc pressed to on");
            }
            else
            {
                CurrentMenu = Inventory;
                NextMenu = Notes;
                NextMenu.SetActive(false);
                CurrentMenu.SetActive(false);
                IsOpen = false;
                playerMovement.SetInventoryState(false);
                Debug.Log("esc pressed to off");
            }
        }
    }
}
