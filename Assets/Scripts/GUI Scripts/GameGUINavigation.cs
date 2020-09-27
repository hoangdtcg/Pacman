using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class GameGUINavigation : MonoBehaviour {

	//------------------------------------------------------------------
	// Variable declarations
	
	private bool _paused;
    private bool quit;
    private string _errorMsg;
	//public bool initialWaitOver = false;

	public float initialDelay;

	// canvas
	public Canvas PauseCanvas;
	public Canvas QuitCanvas;
	public Canvas ReadyCanvas;
	public Canvas ScoreCanvas;
    public Canvas ErrorCanvas;
    public Canvas GameOverCanvas;
	
	// buttons
	public Button MenuButton;

	//------------------------------------------------------------------
	// Function Definitions

	// Use this for initialization
	void Start () 
	{
		StartCoroutine("ShowReadyScreen", initialDelay);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			// if scores are show, go back to main menu
			if(GameManager.gameState == GameManager.GameState.Scores)
				Menu();

			// if in game, toggle pause or quit dialogue
			else
			{
				if(quit == true)
					ToggleQuit();
				else
					TogglePause();
			}
		}
	}

	// public handle to show ready screen coroutine call
	public void H_ShowReadyScreen()
	{
		StartCoroutine("ShowReadyScreen", initialDelay);
	}

    public void H_ShowGameOverScreen()
    {
        StartCoroutine("ShowGameOverScreen");
    }

	IEnumerator ShowReadyScreen(float seconds)
	{
		//initialWaitOver = false;
		GameManager.gameState = GameManager.GameState.Init;
		ReadyCanvas.enabled = true;
		yield return new WaitForSeconds(seconds);
		ReadyCanvas.enabled = false;
		GameManager.gameState = GameManager.GameState.Game;
		//initialWaitOver = true;
	}

    IEnumerator ShowGameOverScreen()
    {
        Debug.Log("Showing GAME OVER Screen");
        GameOverCanvas.enabled = true;
        yield return new WaitForSeconds(2);
        Menu();
    }

	public void getScoresMenu()
	{
		Time.timeScale = 0f;		// stop the animations
		GameManager.gameState = GameManager.GameState.Scores;
		MenuButton.enabled = false;
		ScoreCanvas.enabled = true;
	}

	//------------------------------------------------------------------
	// Button functions

	public void TogglePause()
	{
		// if paused before key stroke, unpause the game
		if(_paused)
		{
			Time.timeScale = 1;
			PauseCanvas.enabled = false;
			_paused = false;
			MenuButton.enabled = true;
		}
		
		// if not paused before key stroke, pause the game
		else
		{
			PauseCanvas.enabled = true;
			Time.timeScale = 0.0f;
			_paused = true;
			MenuButton.enabled = false;
		}


        Debug.Log("PauseCanvas enabled: " + PauseCanvas.enabled);
	}
	
	public void ToggleQuit()
	{
		if(quit)
        {
            PauseCanvas.enabled = true;
            QuitCanvas.enabled = false;
			quit = false;
		}
		
		else
        {
            QuitCanvas.enabled = true;
			PauseCanvas.enabled = false;
			quit = true;
		}
	}

	public void Menu()
	{
		Application.LoadLevel("menu");
		Time.timeScale = 1.0f;

        // take care of game manager
	    GameManager.DestroySelf();
	}

    IEnumerator AddScore(string name, int score)
    {
        
            Debug.Log("SCORE POSTED!");

            Destroy(GameObject.Find("Game Manager"));
            GameManager.score = 0;
            GameManager.Level = 0;

            Application.LoadLevel("menu");
            Time.timeScale = 1.0f;
        

        yield return new WaitForSeconds(2);
    }


	public void SubmitScores()
	{
		// Check username, post to database if its good to go
	    int highscore = GameManager.score;
        string username = ScoreCanvas.GetComponentInChildren<InputField>().GetComponentsInChildren<Text>()[1].text;
        Regex regex = new Regex("^[a-zA-Z0-9]*$");

	    if (username == "")                 ToggleErrorMsg("Username cannot be empty");
        else if (!regex.IsMatch(username))  ToggleErrorMsg("Username can only consist alpha-numberic characters");
        else if (username.Length > 10)      ToggleErrorMsg("Username cannot be longer than 10 characters");
        else                                StartCoroutine(AddScore(username, highscore));
	    
	}

    public void LoadLevel()
    {
        GameManager.Level++;
        Application.LoadLevel("game");
    }

    public void ToggleErrorMsg(string errorMsg)
    {
        if (ErrorCanvas.enabled)
        {
            ScoreCanvas.enabled = true;
            ErrorCanvas.enabled = false;

        }
        else
        {
            ScoreCanvas.enabled = false;
            ErrorCanvas.enabled = true;
            ErrorCanvas.GetComponentsInChildren<Text>()[1].text = errorMsg;

        }
    }
}
