using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventorySwitcher : MonoBehaviour
{
    public GameObject notes;
    public GameObject Inventory;
    public KeyCode switchLeftKey = KeyCode.Q;
    public KeyCode switchRightKey = KeyCode.E;
    public KeyCode GameKey = KeyCode.Escape;
    

    // Start is called before the first frame update
    void Start()
    {
        notes.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(switchLeftKey))
        {
            Inventory.SetActive(false);
            notes.SetActive(true);
        }

        if (Input.GetKeyDown(switchRightKey))
        {
            notes.SetActive(false);
            Inventory.SetActive(true);
        }
        
        if  (Input.GetKeyDown(GameKey))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
}
