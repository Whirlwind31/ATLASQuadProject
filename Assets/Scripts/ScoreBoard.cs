using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public Text scoreText;
    public static int scoreCount;
    void Start()
    {
        
    }

    void Update()
    {
        scoreText.text = "Eggs Broken: " + Mathf.Round(scoreCount);
    }
}
