using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject optionsScreen;
    public GameObject CarrierMainMenu;
   
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void backtoMainMenu()
    {
        SceneManager.LoadScene("menu");
    }

    public void LoadGame(string sceneLoad)
    {
        SceneManager.LoadScene(sceneLoad);
        CarrierMainMenu.SetActive(false);

    }

      public void LoadTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void OpenOptions()
    {
        optionsScreen.SetActive(true);
        CarrierMainMenu.SetActive(false);
        
    }

    public void CloseOptions()
    {
        optionsScreen.SetActive(false);
        CarrierMainMenu.SetActive(true);

    }

    public void OpenTutorial()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting application");

    }
}

