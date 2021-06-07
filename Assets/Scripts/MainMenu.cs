using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource bgAudioSource;


    public GameObject lastMenu;

    public void Start() {
        if(!bgAudioSource.isPlaying)
            bgAudioSource.Play();
    }

    public void PlayGame ()
    {
        lastMenu.SetActive(false);
        bgAudioSource.Stop();
        SceneManager.LoadScene(1);
    }
}
