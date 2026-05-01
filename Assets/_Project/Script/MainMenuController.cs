using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    //Bottone GIOCA
    public void PlayGame()
    {
        SceneManager.LoadScene(1); //carica la scena con index 1
        Debug.Log("Open Game - da implementare");
    }
    //Bottone OPZIONI
    public void OpenOptions()
    {
        Debug.Log("Options clicked - da implementare");
    }
    //Bottone ESCI
    public void ExitGame()
    {
        Debug.Log("Exit Game");
        //Funziona solo nella build
        Application.Quit();
    }
}
