/**
 * Road To Goal
 * David Vargas Carrillo, 2016
 * 
 * File: BallShoot.cs
 * This script controls the movement of the ball when a car hits it
 */

using UnityEngine;
using System.Collections;

public class BallShoot : MonoBehaviour {

	public float m_shootSpeed;							// Speed that the ball will acquire
	public AudioSource m_HitAudio;						// Audio to be played when hitted
	public Transform m_SpawnPoint;						// Position where the ball will appear after each round

	public Rigidbody m_Rigidbody;						// Reference to the rigidbody of the ball

	[HideInInspector] public bool m_isGoal;				// Store if some player has scored in the current round
	[HideInInspector] public int m_whoScored;			// Store the number of the player who scored

	// When the ball collides with something this method is called
	void OnCollisionEnter(Collision collision) {
		Collider hit = collision.collider;

		// Look for the CarMovement class in the hit element
		CarMovement carScript = hit.GetComponent<CarMovement> ();
	
		// If the element has a CarMovement script
		if (carScript) {
			Transform carTransform = hit.GetComponent<Transform>();
			Vector3 shootVector = carTransform.position;

			m_Rigidbody.AddForce (shootVector * m_shootSpeed);

			m_HitAudio.Play ();
		}
		if (hit.CompareTag("Goal1")) {
			m_isGoal = true;
			m_whoScored = 1;
		}
		if (hit.CompareTag("Goal2")) {
				m_isGoal = true;
				m_whoScored = 2;
		}
	}

	public bool IsGoal() {
		bool goal = false;
		// If there is a goal, return true and reset the attribute to false
		if (m_isGoal) {
			goal = true;
			m_isGoal = false;
		}
		return goal;
	}

	// Freeze the position of the ball
	public void Freeze() {
		m_Rigidbody.isKinematic = true;
	}

	// Unfreeze the position of the ball
	public void Unfreeze() {
		m_Rigidbody.isKinematic = false;
	}

	// Reset ball position to the starting point
	public void Reset() {
		this.transform.position = m_SpawnPoint.position;
		this.transform.rotation = m_SpawnPoint.rotation;
		Freeze ();
		Unfreeze ();
	}
}
