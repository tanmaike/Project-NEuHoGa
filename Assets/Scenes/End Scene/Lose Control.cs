using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseControl : MonoBehaviour
{
    public void Retry()
    {
        SceneManager.LoadScene("Gamescene");
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
