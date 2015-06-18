using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Status Room Controller
 * "Green light, red light" game, where players must either avoid a curator's line of sight or remain completely still.
 */
public class StatuesRoomController : MonoBehaviour 
{
	public float secondsToTrigger = 0f;
	public float lookAroundInterval = 3f;
	public GameObject monster;
	public float monsterFOV = 45f;

	private bool triggered;
	private bool isPlayerInside;
	private Transform playerTransform;
	private CharacterController playerControls;

	int timesLooked = 0; // testing purposes only


	void Start() {
		triggered = false;
		isPlayerInside = false;
	}


	void OnTriggerEnter(Collider other) {
		// using "Player", and not "PlayerBody", because we want to access the charactercontroller script on the parent object
		if (other.tag == "Player") {
			// when the first player enters, start process
			if (!triggered) {
				triggered = true;
				InvokeRepeating("LookAround", secondsToTrigger, lookAroundInterval);
			}

			// if our own player entered the room
			if (!isPlayerInside && other.gameObject == NetworkManager.instance.GetPlayer()) {
				isPlayerInside = true;
				playerTransform = other.gameObject.transform;
				playerControls = other.gameObject.GetComponent<CharacterController>();

			}
		}
	}


	void LookAround() {
		// if local player is not inside, no need to check anything
		if (!isPlayerInside) {
			return;
		}

		// otherwise, we need to check if player is moving within fov
		if (IsPlayerMoving(playerTransform, playerControls)) {
			ChatManager.instance.AddMessage("Player is within field of view.");
		}
		else {
			ChatManager.instance.AddMessage("Player can't be seen.");
		}

		timesLooked++;

		if (timesLooked >= 10) {
			GetComponent<DoorsController>().TriggerDoors(true);
			Destroy(this);
		}
	}


	// since a character controller is being used to check for movement, calculations can only be done locally (as in, each player is responsible for himself)
	bool IsPlayerMoving(Transform playerTr, CharacterController playerCC) {
		var direction = playerTr.position - monster.transform.position;
		float angle = Vector3.Angle(direction, monster.transform.forward);

		// if player is within field of view and currently moving
		return (angle <= monsterFOV && playerCC.velocity != Vector3.zero);
	}
}
