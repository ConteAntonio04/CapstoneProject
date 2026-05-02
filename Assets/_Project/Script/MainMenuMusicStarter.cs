using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusicStarter : MonoBehaviour
{
    private void Start()
    {
        MusicManager music = FindObjectOfType<MusicManager>();

        if (music != null)
            music.PlayMusic();
    }
}
