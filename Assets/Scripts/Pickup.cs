/**
 * Road To Goal
 * David Vargas Carrillo, 2016
 * 
 * File: Pickup.cs
 * This file contains everything related to the pickups
 */

using UnityEngine;
using System.Collections;

/**
 * Defines the properties of the speed pickup
 */
public class SpeedPickup {
	public static float m_SpeedGain = 100f;
	public static float m_PitchGain = 0.8f;
}

/**
 * Defines the properties of the explosion pickup
 */
public class ExplosionPickup {
	public static float m_SpeedLoss = 90f;
	public static float m_PitchLoss = 0.8f;
}

/**
 * This enumeration contains all the types of pickups
 */
public enum PickupType {
	Speed,		// Adds speed to the car for a short period of time 
	Explosion	// Creates an explosion in front of the opponent's car 
}

/**
 * General class for the pickups
 */
public class Pickup : MonoBehaviour {

	public float m_RotationSpeed;		// Global speed of the rotation of the element 
	public int m_RotX = 15;				// Speed of the rotation in X axis
	public int m_RotY = 30;				// Speed of the rotation in Y axis
	public int m_RotZ = 45;				// Speed of the rotation in Z axis
	public int m_NumberOfTypes = 2;		// Number of type of pickups that the game has to contain

	public Collider m_Collider;			// Collider of the pickup
	public MeshRenderer m_Renderer;		// Renderer of the pickup

	private PickupType m_Type;			// Type of the pickup

	// Use this for initialization
	void Start () {
		// Generate a random number to decide the pickup type
		int type = Random.Range (1, m_NumberOfTypes + 1);
		// Set the tyoe according to the generated number
		switch (type) {
		case 1:
			m_Type = PickupType.Speed;
			break;
		case 2:
			m_Type = PickupType.Explosion;
			break;
		}

		StartCoroutine (Spawn());
	}
	
	// Update is called once per frame
	private void Update () {
		// Rotation of the pickup
		transform.Rotate(new Vector3(m_RotX, m_RotY, m_RotZ) * Time.deltaTime * m_RotationSpeed);
	}

	private void OnTriggerEnter (Collider collider) {
		CarPickup cp = collider.GetComponent<CarPickup> ();
		if (cp) {
			// Disable the pickup from the game
			this.gameObject.SetActive (false);
			cp.AddPickup (m_Type);
			this.gameObject.SetActive (true);
			Start ();
		}
	}

	IEnumerator Spawn () {
		m_Collider.enabled = false;
		m_Renderer.enabled = false;
		yield return new WaitForSeconds(10f);  // Wait three seconds
		m_Collider.enabled = true;
		m_Renderer.enabled = true;
	}
}
