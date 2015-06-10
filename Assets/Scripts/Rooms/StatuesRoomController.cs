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
	public GameObject monster;
	public float monsterFOV = 45f;

	bool triggered;
	List<Transform> playersTr; // reference to players currently inside the room
	List<CharacterController> playersCC; // not worth creating a custrom structure to hold both values

	int timesLooked = 0; // testing purposes only

	void Start() {
		playersTr = new List<Transform>();
		playersCC = new List<CharacterController>();
		triggered = false;
	}
	
	void OnTriggerEnter(Collider other) {
		// using "Player", and not "PlayerBody", because we want to access the charactercontroller script on the parent object
		if (other.tag == "Player") {
			playersTr.Add(other.gameObject.transform);
			playersCC.Add(other.gameObject.GetComponent<CharacterController>());

			// when the first player enters, start process
			if (!triggered) {
				triggered = true;
				InvokeRepeating("LookAround", secondsToTrigger, 3f);
			}
		}
	}

	void LookAround() {
		for (int i = 0; i < playersTr.Count; i++) {
			if (IsPlayerMoving(playersTr[i], playersCC[i])) {
				Debug.Log("Player is within field of view.");
			}
			else {
				Debug.Log("Player can't be seen.");
			}
		}

		timesLooked++;

		if (timesLooked >= 10) {
			GetComponent<DoorsController>().TriggerDoors(true);
			Destroy(this);
		}
	}

	bool IsPlayerMoving(Transform playerTr, CharacterController playerCC) {
		var direction = playerTr.position - monster.transform.position;
		float angle = Vector3.Angle(direction, monster.transform.forward);

		// if player is within field of view and currently moving
		return (angle <= monsterFOV && playerCC.velocity != Vector3.zero);
	}
}
