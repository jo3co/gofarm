using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource bgAudioSource;

    public void Start() {
        bgAudioSource.Play();
    }

    public void PlayGame ()
    {
        bgAudioSource.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
