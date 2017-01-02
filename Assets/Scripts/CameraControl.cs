/**
 * Road To Goal
 * David Vargas Carrillo, 2016
 * 
 * File: CameraControl.cs
 * This script controls the movement of the camera in the game
 */

using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	
	public float m_DampTime = 0.2f;                 	// Approx. time for the camera to refocus
	public float m_ScreenEdgeBuffer = 4f;           	// Space between the top/bottom most target and the screen edge
	public float m_MinFieldOfView = 25f;                // The smallest field of view the camera can be
	[HideInInspector]public Transform[] m_Targets; 		// Array of targets to focus

	private Camera m_Camera;              				// Reference to the camera          
	private float m_ZoomSpeed;                      	// Reference to the zooming speed for a smooth change of zoom
	private Vector3 m_MoveVelocity;                 	// Reference to the moving speed for a smooth movement
	private Vector3 m_DesiredPosition;              	// Where is the camera moving to


	// Called once, just before the rest of the script starts
	private void Awake()
	{
		m_Camera = GetComponentInChildren<Camera>();
	}


	// Called before any physics step
	private void FixedUpdate()
	{
		Move();
		Zoom();
	}


	// Move the car forward or backward depending on the input
	private void Move()
	{
		FindAveragePosition();

		transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
	}


	// Gets the position between every object in the game
	private void FindAveragePosition()
	{
		Vector3 averagePos = new Vector3();
		int numTargets = 0;

		for (int i = 0; i < m_Targets.Length; i++)
		{
			if (!m_Targets[i].gameObject.activeSelf)
				continue;

			averagePos += m_Targets[i].position;
			numTargets++;
		}

		if (numTargets > 0)
			averagePos /= numTargets;

		averagePos.y = transform.position.y;

		m_DesiredPosition = averagePos;
	}


	// Zooms the camera the required size
	private void Zoom()
	{
		float requiredSize = FindRequiredFOV();
		m_Camera.fieldOfView = Mathf.SmoothDamp(m_Camera.fieldOfView, requiredSize, ref m_ZoomSpeed, m_DampTime);
	}


	// Gets the required field of view depending on the situation
	private float FindRequiredFOV()
	{
		Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

		float size = 0f;

		for (int i = 0; i < m_Targets.Length; i++)
		{
			if (!m_Targets[i].gameObject.activeSelf)
				continue;

			// Find the target in the camera rig local space
			Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

			Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

			size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

			size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
		}

		size += m_ScreenEdgeBuffer;

		size = Mathf.Max(size, m_MinFieldOfView);

		return size;
	}


	// Sets the start position for the camera
	public void SetStartPositionAndSize()
	{
		FindAveragePosition();

		transform.position = m_DesiredPosition;

		m_Camera.fieldOfView = FindRequiredFOV();
	}
}