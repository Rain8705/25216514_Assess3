using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level01_Manual");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game pressed");
        Application.Quit();
    }
}
