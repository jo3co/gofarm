using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BackMenu : MonoBehaviour
{
    //public Text scoreText;
    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI scoreText2;

    void Start() {
        int score = PlayerPrefs.GetInt("score");
        scoreText1.text = score.ToString();
        scoreText2.text = score.ToString();
    }

    public void BackGame ()
    {
        SceneManager.LoadScene(0);
    }
}

