/**
 * Road To Goal
 * David Vargas Carrillo, 2016
 * 
 * File: CarMovement.cs
 * This script controls the movement and audio of each car in the game
 */

using UnityEngine;
using System.Collections;

public class CarMovement : MonoBehaviour {

	public int m_PlayerNumber;			// Number over the total of players

	public float m_TopSpeed = 100;		// Maximum speed the car can reach
	public float m_SpeedIncr = 1.5f;	// Increment of speed after each frame
	private float m_CurrentSpeed = 0;	// Stores the current speed of the car

	public float m_TurnSpeed = 180f;	// Turn speed of the car

	public AudioSource m_MovementAudio;	// Audio source for the clips
	public AudioClip m_EngineDriving;	// Audio clip for the car moving
	public AudioClip m_EngineIdling;	// Audio clip for the car stopped
	public float m_PitchRange = 0.1f;	// To prevent overlapping on the audio clips

	private string m_MovementAxisName;	// Name of the axis for movement, set in the game preferences
	private string m_TurnAxisName;		// Name of the axis for turning, set in the game preferences
	private Rigidbody m_Rigidbody;		// Reference to the rigidbody of the car
	private float m_OriginalPitch;		// Stores the original value of the pitch

	// These variables are used to store input values in certain situations
	private float m_MovementInputValue;	
	private float m_TurnInputValue;		


	// Called once, just before the rest of the script starts
	private void Awake()
	{
		// Save the Rigidbody component in the member variable
		m_Rigidbody = GetComponent<Rigidbody> ();
	}

	// Called everytime the object of this script is enabled
	private void OnEnable()
	{
		// Make sure that the Rigidbody of the car is not kinematic
		m_Rigidbody.isKinematic = false;

		// Reset input values
		m_MovementInputValue = 0f;
		m_TurnInputValue = 0f;
	}

	// Called just before the object of this script is disabled
	private void OnDisable()
	{
		// Make the object kinematic
		m_Rigidbody.isKinematic = true;
	}

	// Initialization
	private void Start ()
	{
		m_MovementAxisName = "Vertical" + m_PlayerNumber;
		m_TurnAxisName = "Horizontal" + m_PlayerNumber;

		m_OriginalPitch = m_MovementAudio.pitch;
	}
	
	// Called once per frame
	private void Update () {
		// Store player's input and make sure the engine audio is playing
		m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
		m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

		EngineAudio ();
	}

	// Play the correct audio clip based on car's state
	private void EngineAudio()
	{
		// If the car is idle
		if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f) {
			// If the driving clip is playing, change it for the idling clip
			if (m_MovementAudio.clip == m_EngineDriving) {
				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, 
					m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play ();
			}
		}
		// If th car is driving
		else {
			// If the idling clip is playing, change it for the driving clip
			if (m_MovementAudio.clip == m_EngineIdling) {
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange,
					m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play ();
			}
		}
	}
	
	// Called before any physics step
	private void FixedUpdate()
	{
		Move ();
		Turn ();
	}

	// Move the car forward or backward depending on the input
	private void Move()
	{
		// If the player is moving the car and the speed is not the maximum, increment it
		if (Mathf.Abs (m_MovementInputValue) > 0.1f) {
			if (m_CurrentSpeed < m_TopSpeed)
				m_CurrentSpeed += m_SpeedIncr;
		}
		// If not, return to the speed value to zero
		else {
			if (m_CurrentSpeed > 0f)
				m_CurrentSpeed = 0f;
		}

		// Calculates the movement vector
		Vector3 movement = transform.right * m_MovementInputValue * m_CurrentSpeed * Time.deltaTime;
		// Applies the movement vector to the rigidbody
		m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
	}

	// Turn the car right or left depending on the input
	private void Turn()
	{
		// Calculates the rotation value and apply to a quaternion
		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
		// Applies the rotation quaterion to the rigidbody
		m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
	}

	public void SetSpeed (float speed) {
		m_CurrentSpeed = speed;
	}

}
