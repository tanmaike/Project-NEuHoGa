using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WinControls : MonoBehaviour
{
    public void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void Quit()
    {
        Debug.Log("Quit");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
     }
}
