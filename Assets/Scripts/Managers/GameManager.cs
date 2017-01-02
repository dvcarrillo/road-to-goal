/**
 * Road To Goal
 * David Vargas Carrillo, 2016
 * 
 * File: GameManager.cs
 * Contains the properties of the game and manages each round
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

	public int m_NumGoalsToWin;				// Number of goals a player has to score to win the game
	public float m_StartDelay;				// Delay until the start of a new round
	public float m_EndDelay;				// Delay from the end of a round to the start of a new one
	public CameraControl m_CameraControl;	// Reference to the camera control script
	public Text m_MessageText;				// Reference to the informative text shown in different cincumstances
	public Text m_Scoreboard;				// Reference to text on the the scoreboard
	public Sprite m_SpeedPickupIMG;			// Sprite referencing the speed pickup
	public Sprite m_ExplosionPickupIMG;		// Sprite referencing the explosion pickup
	public Sprite m_NoPickupIMG;			// Sprite referencing no pickup collected
	public GameObject m_CarPrefab;			// Reference to the prefab of the car of each player
	public BallShoot m_Ball;				// Reference to the ball for detecting goals
	public AudioSource m_BackgroundMusic;	// Background music during the game
	public AudioSource m_GoalSound;			// Sound effect after goals
	public Image[] m_UIPickupsIMG;			// UI images to show player pickups
	public Text[] m_UIPickupsText;			// UI text to show player pickups
	public CarManager[] m_Cars;				// Array of CarManager scripts that will change car properties in game

	private WaitForSeconds m_StartWait;		// Used to have a delay when the round starts
	private WaitForSeconds m_EndWait;		// Used to have a delay after the round ends, until the next one
	private CarManager m_RoundWinner;		// Reference to the winner of the current round, for the message text
	private CarManager m_GameWinner;		// Reference to the winner of the game, for displaying in the message text


	// Use this for initialization
	private void Start () {
		// Create the delays so they only have to be made once
		m_StartWait = new WaitForSeconds(m_StartDelay);
		m_EndWait = new WaitForSeconds (m_EndDelay);

		// Spawn the cars and ajust 
		SpawnAllCars ();
		SetCameraTargets ();

		// Once the cars have been created and the camera is using them as targets, start the game
		StartCoroutine (GameLoop ());
	}

	// Spawn all the cars in their spawn points
	private void SpawnAllCars() {
		// Set the number of players and i the cars
		for (int i = 0; i < m_Cars.Length; i++) {
			m_Cars [i].m_Instance = Instantiate (m_CarPrefab, m_Cars [i].m_SpawnPoint.position, m_Cars [i].m_SpawnPoint.rotation) as GameObject;
			// Set the number of this player
			m_Cars [i].m_PlayerNumber = i + 1;
		}
		// Set the number of opponents
		for (int i = 0; i < m_Cars.Length; i++) {
			if (i == 0) 	// Player 1
				m_Cars [i].m_Opponent = m_Cars [1].m_Instance;
			else 			// Player 2
				m_Cars [i].m_Opponent = m_Cars [0].m_Instance;
			m_Cars [i].Setup ();
		}
	}

	// Set the targets that the camera has to follow
	private void SetCameraTargets() {
		// Create a collection of transforms the same size as the number of cars
		Transform[] targets = new Transform[m_Cars.Length];

		// For each of these transforms set it to the appropriate car transform
		for (int i = 0; i < targets.Length; i++)
		{
			targets[i] = m_Cars[i].m_Instance.transform;
		}

		// These are the targets the camera should follow
		m_CameraControl.m_Targets = targets;
	}

	// This is called from start and will run each phase of the game one after another
	private IEnumerator GameLoop () {
		// Start off by running the 'RoundStarting' coroutine but don't return until it's finished
		yield return StartCoroutine (RoundStarting ());

		// Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished
		yield return StartCoroutine (RoundPlaying());

		// Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished
		yield return StartCoroutine (RoundEnding());

		// This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found
		if (m_GameWinner != null)
		{
			// If there is a game winner, restart the level
			SceneManager.LoadScene("Main");
		}
		else
		{
			// If there isn't a winner yet, restart this coroutine so the loop continues
			StartCoroutine (GameLoop ());
		}
	}

	private IEnumerator RoundStarting() {
		// As soon as the roud starts, reset the cars and make sure they cannot move
		ResetAllCars();
		m_Ball.Reset ();
		DisableCarControl ();

		// Play the background music
		m_BackgroundMusic.Play();

		// Refresh the pickup indicators
		RefreshPickupsUI();

		// Snap the camera's zoom and position to something appropriate for the reset cars
		m_CameraControl.SetStartPositionAndSize ();

		// Display text to warn that the round is about to begin
		m_MessageText.text = "PREPARE";

		// Refresh the scoreboard text
		m_Scoreboard.text = ScoreMessage();

		// Wait for the specified length of time until yielding control back to the game loop
		yield return m_StartWait;
	}

	private IEnumerator RoundPlaying () {
		// As soon as the round begins playing let the players control the cars
		EnableCarControl ();

		// Clear the text from the screen.
		m_MessageText.text = string.Empty;

		// While there is not a goal, return on the next frame
		while (!IsGoal())
		{
			RefreshPickupsUI ();
			yield return null;
		}
	}

	private IEnumerator RoundEnding () {
		// Play the goal sound
		m_BackgroundMusic.Stop();
		m_GoalSound.Play ();

		// Stop cars from moving
		DisableCarControl ();

		// Stop ball from moving
		m_Ball.Freeze();

		// Clear the winner from the previous round
		m_RoundWinner = null;

		// See if there is a winner now the round is over
		m_RoundWinner = GetRoundWinner ();

		// If there is a winner, increment its score
		m_RoundWinner.m_Goals++;

		// See if someone has won the game
		m_GameWinner = GetGameWinner ();

		// Indicate a goal in the scoreboard
		m_Scoreboard.text = "GOOOOOAL!";

		// Display the ending message, which will change according to the result of the last round
		string message = EndMessage ();
		m_MessageText.text = message;

		// Wait for the specified length of time until yielding control back to the game loop.
		yield return m_EndWait;
	}

	// Used to check if there is a goal in any gate
	private bool IsGoal() {
		bool goal = m_Ball.IsGoal ();
		return goal;
	}

	// Used to check who is the winner of a round
	private CarManager GetRoundWinner() {
		int roundWinner = m_Ball.m_whoScored;
		return m_Cars[roundWinner-1];
	}

	// Used to find out is there is a game winner
	private CarManager GetGameWinner() {
		// Iterate over all the cars and check if any has enough score to win
		for (int i = 0; i < m_Cars.Length; i++) {
			if (m_Cars [i].m_Goals == m_NumGoalsToWin)
				return m_Cars [i];
		}
		// If no one has enough score, return null
		return null;
	}

	// Return a string message to display at the end of each round
	private string EndMessage() {
		// If this method is reached without any winner, display the word 'error'
		string message = "ERROR";

		// See if there is a round winner and change the message consequently
		if (m_RoundWinner != null)
			message = m_RoundWinner.m_ColoredPlayerText + " SCORES!";
		
		// Add some line breaks after the initial message
		message += "\n\n\n\n";

		// Go through all the cars and add each of their scores to the message.
		for (int i = 0; i < m_Cars.Length; i++)
		{
			message += m_Cars[i].m_ColoredPlayerText + ": " + m_Cars[i].m_Goals + " GOALS\n";
		}

		// If there is a game winner, change the entire message to reflect that
		if (m_GameWinner != null)
			message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

		return message;
	}

	// Return a string message to display in the scoreboard
	private string ScoreMessage() {
		string message = "";
		for (int i = 0; i < m_Cars.Length; i++) {
			message += m_Cars [i].m_Goals;
			if (i != (m_Cars.Length - 1))
				message += " - ";
		}
		return message;
	}

	// Move all the cars back to their starting point and reset their properties
	private void ResetAllCars() {
		for (int i = 0; i < m_Cars.Length; i++) {
			m_Cars [i].Reset ();
		}
	}

	// Enable the movement and action of all the cars
	private void EnableCarControl() {
		for (int i = 0; i < m_Cars.Length; i++) {
			m_Cars [i].EnableControl ();
		}
	}

	// Disable the movement and action of all the cars
	private void DisableCarControl() {
		for (int i = 0; i < m_Cars.Length; i++) {
			m_Cars [i].DisableControl ();
		}
	}

	// Refresh the pickup image in the UI for each car
	private void RefreshPickupsUI() {
		for (int i = 0; i < m_Cars.Length; i++) {
			if (m_Cars [i].HasPickup ()) {
				PickupType pickup = m_Cars [i].GetPickupType ();
				switch (pickup) {
				case PickupType.Speed:
					m_UIPickupsIMG [i].sprite = m_SpeedPickupIMG;
					break;
				case PickupType.Explosion:
					m_UIPickupsIMG [i].sprite = m_ExplosionPickupIMG;
					break;
				}
				m_UIPickupsText [i].text = pickup.ToString ();
			} 
			else {
				m_UIPickupsIMG [i].sprite = m_NoPickupIMG;
				m_UIPickupsText [i].text = "";
			}
		}
	}
}
