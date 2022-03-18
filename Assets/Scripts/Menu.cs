using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
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
    }
}
