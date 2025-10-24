using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventorySwitcher : MonoBehaviour
{
    public GameObject Notes;
    public GameObject Inventory;
    public GameObject Testing;
    public KeyCode switchNextKey = KeyCode.Q;
    public KeyCode switchPreviousKey = KeyCode.E;
    public KeyCode GameKey = KeyCode.Escape;

    public GameObject CurrentMenu;
    public GameObject PreviousMenu;
    public GameObject NextMenu;
    public GameObject Holding;
    

    // Start is called before the first frame update
    void Awake()
    {
        CurrentMenu = Inventory;
        PreviousMenu = Testing;
        NextMenu = Notes;
        PreviousMenu.SetActive(false);
        NextMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(switchNextKey))
        {
            CurrentMenu.SetActive(false);
            NextMenu.SetActive(true);
            Holding = CurrentMenu;
            CurrentMenu = NextMenu;
            NextMenu = PreviousMenu;
            PreviousMenu = Holding;
        }

        if (Input.GetKeyDown(switchPreviousKey))
        {
            CurrentMenu.SetActive(false);
            PreviousMenu.SetActive(true);
            Holding = CurrentMenu;
            CurrentMenu = PreviousMenu;
            PreviousMenu = NextMenu;
            NextMenu = Holding;
        }
        
        if  (Input.GetKeyDown(GameKey))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
}