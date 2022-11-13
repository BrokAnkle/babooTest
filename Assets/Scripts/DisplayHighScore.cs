using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplayHighScore : MonoBehaviour
{
    Text text;
    Score highScore;

    void Start()
    {
        text = GetComponent<Text>();
        
        highScore.time = PlayerPrefs.GetFloat("Time");
        highScore.nbMoves = PlayerPrefs.GetInt("Moves");
        
        int trimedTime = Mathf.RoundToInt(highScore.time);

        if (highScore.time > 0 && highScore.nbMoves > 0)
            text.text = "Current high score : " + trimedTime.ToString() + " seconds in " + highScore.nbMoves.ToString() + " moves";
        else text.text = "No current highscore";
    }
}
