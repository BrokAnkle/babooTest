using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TeasingGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


struct Score
{
    public float time; //time took to complete the puzzle
    public int nbMoves;    //number of moves to complete the puzzle
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Tooltip("Prefab object to spawn if the device running the game is an iOS device")]
    [SerializeField] GameObject ApplePrefab;
    [Tooltip("Prefab object to spawn if the device running the game is an Android device")]
    [SerializeField] GameObject AndroidPrefab;

    [Tooltip("Reference to the text that will display the time remaining to finish the game")]
    [SerializeField] Text timerText;
    [Tooltip("Canvas to display when the player has ran out of time")]
    [SerializeField] Canvas GameOverCanvas;
    [Tooltip("Canvas to display when the player has finished the puzzle")]
    [SerializeField] Canvas WinCanvas;
    [Tooltip("Reference to the grid that contains all the slots to put the tiles in")]
    [SerializeField] GameObject grid;

    [Min(0)] float timer = 180f;    //player has 3 minutes (180 seconds) to complete the puzzle
    public TileScript hiddenTile;   //The tile that is randomly selected to be hidden, will be shown when the puzzle is finished

    public int nbMoves = 0; //The number of moves the player has done to finish the puzzle, used for the score
    public bool tileConstraintToTheNextSlot = true; //if true, the tile can only be dragged on a slot next to it (disable if to not being stuck due to poor tile shuffle)
    bool won = false;   //check if the player win each time he drop a tile in a slot, use to stop updating the timer
    Score score;    //the current score that is being tracked

    private void Awake()
    {
        #region Singleton
        if (instance != null)
            Destroy(this);
        else instance = this;
        #endregion

        nbMoves = 0;

        #region PlatformCheck
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            Instantiate(ApplePrefab);
        else if (Application.platform == RuntimePlatform.Android)
            Instantiate(AndroidPrefab);
        else
            Debug.LogWarning("Current platform neither Android nor iPhone: " + Application.platform.ToString());
        if (Random.Range(0, 2) == 0) Instantiate(ApplePrefab); else Instantiate(AndroidPrefab);
        #endregion
    }


    void Update()
    {
        if (!won || timer > 0f)
            UpdateTimer();  //keep updating the timer as long as the player has neither lost nor won
    }

    void UpdateTimer()
    {
        timer -= Time.deltaTime;
        int iTime = Mathf.CeilToInt(timer); //trim the decimals
        timerText.text = "Time remaining : " + iTime.ToString() + " seconds";
        if (timer == 0f)
            GameOver();
    }

    void GameOver()
    {
        GameOverCanvas.gameObject.SetActive(true);
    }

    public void CheckWin()
    {
        won = true;
        //iterate throught every slots
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            SlotScript slot = grid.transform.GetChild(i).GetComponent<SlotScript>();
            if (slot.holdingTile == TILE_POSITION.NONE) continue;   //ignore the slot that hold nothing
            if (slot.slotPosition != slot.holdingTile)
                won = false;    //if the current tile is not in its win position, the player has not won yet
        }
        if (won)
        {
            WinCanvas.gameObject.SetActive(true);   //show the win screen
            hiddenTile.gameObject.SetActive(true);  //show the hidden tile

            RegisterScore();    //check if the highscore
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   //reload the game scene
    }

    void RegisterScore()
    {
        score.time = 180f - timer;  //Max time - time remaining = time took
        score.nbMoves = nbMoves;

        Score highscore = new Score();
        //Get the saved highscore in the PlayerPrefs
        highscore.time = PlayerPrefs.GetFloat("Time");
        highscore.nbMoves = PlayerPrefs.GetInt("Moves");

        //if a highscore already exists and the player did better OR if there is no highscore currently saved, save this one
        if ((highscore.time == 0 || score.time < highscore.time) || (highscore.nbMoves == 0 || score.nbMoves < highscore.nbMoves))
        {
            PlayerPrefs.SetFloat("Time", 180f - timer);
            PlayerPrefs.SetInt("Moves", nbMoves);
            PlayerPrefs.Save();
        }
    }
}
