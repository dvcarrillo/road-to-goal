/**
 * Road To Goal
 * David Vargas Carrillo, 2016
 * 
 * File: CarPickup.cs
 * This script controls the picking up and firing of pickups from the car
 */

using UnityEngine;
using System.Collections;

public class CarPickup : MonoBehaviour {

	public int m_PlayerNumber = 1;							// Number over the total players
	public GameObject m_Opponent;							// Reference to the opponent
	public AudioSource m_SoundEffectSource;					// Audio source for pickups sounds
	public AudioClip m_ExplosionSound;						// Audio clip when activating the explosion prefab
	public ParticleSystem m_ExplosionParticles;				// Reference to the particles for the explosion pickup

	public Rigidbody m_Rigidbody;

	private string m_ActionButton;							// Reference to the name of the action button
	private CarMovement m_CMScript;							// Reference to the car movement script

	public bool m_HasPickup;								// True when the car has picked up a pickup
	public PickupType m_PickupType;							// Type of the pickup that the car has
	private bool m_UsingPickup;								// True when the player is using the pickup
	private int m_FrameLimit = 40,							// Duration of the pickup effect that require it
				m_FrameCounter;								// Counter for the duration of the pickups


	// Use this for initialization
	private void Start () {
		m_ActionButton = "Action" + m_PlayerNumber;
		// The player begins with no pickup collected
		m_HasPickup = false;
		// Save the reference to the car movement script
		m_CMScript = this.GetComponent<CarMovement> ();
		// Frame counter starts at 0
		m_FrameCounter = 0;
	}
	
	// Update is called once per frame
	private void Update () {
		// Pushing the action key
		if (m_HasPickup && !m_UsingPickup && Input.GetButton (m_ActionButton)) {
			ActivatePickup ();
		} else if (m_HasPickup && m_UsingPickup) {
			bool finished = UsingPickup ();
			if (finished)
				StopPickup ();
		} 
	}

	// Activate the collected pickup
	private void ActivatePickup() {
		if (m_HasPickup) {
			switch (m_PickupType) {
			case PickupType.Speed:		// Pickup of type speed
				float speedGain = SpeedPickup.m_SpeedGain;
				m_CMScript.m_MovementAudio.pitch += SpeedPickup.m_PitchGain;
				m_CMScript.m_TopSpeed += speedGain;
				m_CMScript.m_SpeedIncr += speedGain;
				m_UsingPickup = true;
				break;
			case PickupType.Explosion:	// Pickup of type explosion
				// Get the opponent CarMovement script
				CarPickup opponentCPScript = m_Opponent.GetComponent<CarPickup> ();
				CarMovement opponentCMScript = m_Opponent.GetComponent<CarMovement> ();

				// Play the explosion particles in the explosion area
				opponentCPScript.m_ExplosionParticles.Play ();

				// Play the explosion sound
				m_SoundEffectSource.clip = m_ExplosionSound;
				m_SoundEffectSource.Play ();

				// Reduce opponents movement
				opponentCMScript.m_MovementAudio.pitch -= ExplosionPickup.m_PitchLoss;
				opponentCMScript.m_TopSpeed -= ExplosionPickup.m_SpeedLoss;
				opponentCMScript.SetSpeed (opponentCMScript.m_TopSpeed);
				m_UsingPickup = true;
				break;
			}
		}
	}

	// Count the time using the pickup
	private bool UsingPickup() {
		bool finished = false;
		if (m_FrameCounter < m_FrameLimit)
			m_FrameCounter++;
		else {
			finished = true;
			m_FrameCounter = 0;
		}
		return finished;
	}

	// Stop using the activated pickup
	private void StopPickup() {
		switch (m_PickupType) {
		case PickupType.Speed:
			float speedGain = SpeedPickup.m_SpeedGain;
			m_CMScript.m_MovementAudio.pitch -= SpeedPickup.m_PitchGain;
			m_CMScript.m_TopSpeed -= speedGain;
			m_CMScript.m_SpeedIncr -= speedGain;
			m_CMScript.SetSpeed (m_CMScript.m_TopSpeed);
			break;
		case PickupType.Explosion:
			CarMovement opponentCMScript = m_Opponent.GetComponent<CarMovement> ();
			opponentCMScript.m_MovementAudio.pitch += ExplosionPickup.m_PitchLoss;
			opponentCMScript.m_TopSpeed += ExplosionPickup.m_SpeedLoss;
			break;
		}
		m_HasPickup = false;
		m_UsingPickup = false;
	}

	// Adds a collected pickup of the desired type
	public void AddPickup(PickupType type) {
		m_HasPickup = true;
		if (type == PickupType.Speed)
			m_PickupType = PickupType.Speed;
		if (type == PickupType.Explosion)
			m_PickupType = PickupType.Explosion;
	}

	// Delete the collected pickup
	public void DeletePickup() {
		m_HasPickup = false;
	}
}
