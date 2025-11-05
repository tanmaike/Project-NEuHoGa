using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public GameObject Notes;
    public GameObject Inventory;
    public GameObject Crafting;
    public KeyCode switchRightKey = KeyCode.Q;
    public KeyCode switchLeftKey = KeyCode.E;
    public KeyCode InventoryKey = KeyCode.Escape;
    public CraftingLogic CraftingLogic; 
    public ClearNotes clearNotes;
    bool IsOpen = false;

    private GameObject CurrentMenu;
    private GameObject LeftMenu;
    private GameObject RightMenu;
    private GameObject Holding;


    // Start is called before the first frame update
    void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();

        CurrentMenu = Inventory;
        LeftMenu = Crafting;
        RightMenu = Notes;
        LeftMenu.SetActive(false);
        RightMenu.SetActive(false);
        CurrentMenu.SetActive(false);
        Debug.Log("working");
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOpen == true) {
            if (Input.GetKeyDown(switchRightKey))
            {
                CurrentMenu.SetActive(false);
                RightMenu.SetActive(true);
                Holding = CurrentMenu;
                CurrentMenu = RightMenu;
                RightMenu = LeftMenu;
                LeftMenu = Holding;
                CraftingLogic.Reset();
                clearNotes.Deactivate();
                Debug.Log("Q pressed");
            }

            if (Input.GetKeyDown(switchLeftKey))
            {
                CurrentMenu.SetActive(false);
                LeftMenu.SetActive(true);
                Holding = CurrentMenu;
                CurrentMenu = LeftMenu;
                LeftMenu = RightMenu;
                RightMenu = Holding;
                CraftingLogic.Reset();
                clearNotes.Deactivate();
                Debug.Log("E pressed");
            }
        }

        if (Input.GetKeyDown(InventoryKey) && !playerMovement.IsJumping())
        {
            if (IsOpen == false)
            {
                CurrentMenu.SetActive(true);
                IsOpen = true;
                Debug.Log("esc pressed to on");
            }
            else
            {
                CurrentMenu = Inventory;
                LeftMenu = Crafting;
                RightMenu = Notes;
                LeftMenu.SetActive(false);
                RightMenu.SetActive(false);
                CurrentMenu.SetActive(false);
                IsOpen = false;
                Debug.Log("esc pressed to off");
            }
        }
    }
}
