using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackMenu : MonoBehaviour
{
    public Text scoreText;

    void Start() {
        int score = PlayerPrefs.GetInt("score");
        scoreText.text = score.ToString();
    }

    public void BackGame ()
    {
        SceneManager.LoadScene(0);
    }
}

