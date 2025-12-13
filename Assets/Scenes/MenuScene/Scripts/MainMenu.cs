using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject playbutton;
    public GameObject CreditButton;
    public GameObject quitButton;
    public GameObject backButton;
    public GameObject Credits;

    public void Awake()
    {
        playbutton.SetActive(true);
        CreditButton.SetActive(true);
        quitButton.SetActive(true);
        backButton.SetActive(false);
        Credits.SetActive(false);
    }
    
    public void Back()
    {
        playbutton.SetActive(true);
        CreditButton.SetActive(true);
        quitButton.SetActive(true);
        backButton.SetActive(false);
        Credits.SetActive(false);
    }
    public void ShowCredits()
    {
        playbutton.SetActive(false);
        CreditButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
        Credits.SetActive(true);
    }
    public void playGame(){
        SceneManager.LoadScene("GameScene");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    
}
