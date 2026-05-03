using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioSource clickSource;
    public void PlayGame()
    {
        StartCoroutine(PlayGameRoutine());
    }
    private IEnumerator PlayGameRoutine()
    {
        if(clickSource != null) clickSource.Play();
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(1);
    }
    public void OpenOptions()
    {
        Debug.Log("Da implementare");
    }
    
    public void ExitGame()
    {
        Debug.Log("Exit Game");
        
        Application.Quit();
    }
}
