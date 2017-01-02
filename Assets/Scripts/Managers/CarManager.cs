/**
 * Road To Goal 
 * David Vargas Carrillo, 2016
 * 
 * File: CarManager.cs
 * Contains the properties that relates the cars and the game
 */

using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class CarManager{

	public Color m_PlayerColor;								// Color of the car in the game
	public Transform m_SpawnPoint;							// Spawn position of the car in the game
	[HideInInspector] public int m_PlayerNumber;			// Number of the player in the game
	[HideInInspector] public GameObject m_Opponent;			// Player's opponent
	[HideInInspector] public string m_ColoredPlayerText;	// Coloured text for composing the UI messages
	[HideInInspector] public GameObject m_Instance;			// Reference to the instance of the car when it is created
	[HideInInspector] public int m_Goals;					// The number of goals the player has scored so far

	private CarMovement m_Movement;							// Reference to the CarMovement script of the car element
	public CarPickup m_Pickup;								// Reference to the CarPickup script of the car element


	// Initialization of the attributes
	public void Setup() {
		// Get the references to the components
		m_Movement = m_Instance.GetComponent<CarMovement>();
		m_Pickup = m_Instance.GetComponent<CarPickup> ();
	
		// Set the player numbers
		m_Movement.m_PlayerNumber = m_PlayerNumber;
		m_Pickup.m_PlayerNumber = m_PlayerNumber;

		// Set the opponent numbers
		m_Pickup.m_Opponent = m_Opponent;

		// Create a string using the correct color based on the car's color and the player's number
		m_ColoredPlayerText =  "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

		// Get all of the renderers of the car
		MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer> ();

		// Go through all the renderers to set their material color to the color specific to this car
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material.color = m_PlayerColor;
		}
	}

	// Disable the control of the car
	public void DisableControl() {
		m_Movement.enabled = false;
		m_Pickup.enabled = false;
	}

	// Enable the control of the car
	public void EnableControl() {
		m_Movement.enabled = true;
		m_Pickup.enabled = true;
	}
	
	// Reset the car to the starting point
	public void Reset() {
		m_Instance.transform.position = m_SpawnPoint.position;
		m_Instance.transform.rotation = m_SpawnPoint.rotation;
		m_Pickup.DeletePickup ();

		m_Instance.SetActive (false);
		m_Instance.SetActive (true);
	}

	// Checks if the car has a pickup
	public bool HasPickup() {
		return (m_Pickup.m_HasPickup);
	}

	// Returns the type of pickup the car has
	public PickupType GetPickupType() {
		return (m_Pickup.m_PickupType);
	}
}
