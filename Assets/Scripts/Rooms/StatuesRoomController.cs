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
	public int timesToTurn = 5;
	public GameObject monster;
	public float monsterFOV = 45f;

	public GameObject spotlight;
	public GameObject pointlight;

	private bool triggered;
	private bool isPlayerInside;

	private Vector3 initialPos;
	private Vector3 goal;
	private int timesTurned = 0;
	private Transform playerTransform;
	private CharacterController playerControls;
	private PlayerNetworkManager playerNetManager;


	void Start() {
		triggered = false;
		isPlayerInside = false;

		initialPos = monster.transform.localEulerAngles;
		goal = new Vector3(0f, -135, 0f);
	}


	void OnTriggerEnter(Collider other) {
		// using "Player", and not "PlayerBody", because we want to access the charactercontroller script on the parent object
		if (other.tag == "Player") {
			// if our own player entered the room
			if (!isPlayerInside && other.gameObject == NetworkManager.instance.GetPlayer()) {
				isPlayerInside = true;
				playerTransform = other.gameObject.transform;
				playerControls = other.gameObject.GetComponent<CharacterController>();
				playerNetManager = other.gameObject.GetComponent<PlayerNetworkManager>();
			}

			// when the first player enters, start process
			if (!triggered) {
				triggered = true;
				Invoke("StartEvent", secondsToTrigger);
			}
		}
	}

	void StartEvent()
	{
		spotlight.GetComponent<Light> ().enabled = true;
		spotlight.GetComponent<Light> ().spotAngle = 38.8f;

		pointlight.GetComponent<Light> ().enabled = true;

		StartCoroutine ("TurnAround");
	}


	IEnumerator TurnAround() {
		// if local player is not inside, no need to check anything
		if (!isPlayerInside) {
			return true;
		}

		bool reachedGoal = false;

		// rotate towards the other side, checking for player
		while (!reachedGoal) {
			monster.transform.localEulerAngles = Vector3.MoveTowards(monster.transform.localEulerAngles, goal, Time.deltaTime * 40f);

			// check if player is moving within fov
			if (IsPlayerMoving(playerTransform, playerControls)) {
				playerNetManager.TakeDamage(1000);
			}

			// if we're close enough, stop
			if (monster.transform.localEulerAngles == goal) {
				reachedGoal = true;
			}

			yield return null;
		}

		reachedGoal = false;

		// rotate back towards initial goal
		while (!reachedGoal) {
			monster.transform.localEulerAngles = Vector3.MoveTowards(monster.transform.localEulerAngles, initialPos, Time.deltaTime * 40f);

			if (Vector3.Distance(monster.transform.localEulerAngles, initialPos) < 0.01f) {
				reachedGoal = true;
			}
			
			yield return null;
		}

		if (++timesTurned < timesToTurn) {
			StartCoroutine ("TurnAround");
		} 
		else {
			Destroy(this);
		}
	}


	// since a character controller is being used to check for movement, calculations can only be done locally (as in, each player is responsible for himself)
	bool IsPlayerMoving(Transform playerTr, CharacterController playerCC) {
		Vector3 direction = playerTr.position - monster.transform.position;
		float angle = Vector3.Angle(direction, monster.transform.forward);

		// if player is within field of view and currently moving
		return (angle <= monsterFOV && playerCC.velocity != Vector3.zero);
	}

	public void MonsterTagged()
	{
		GetComponent<DoorsController> ().TriggerDoors (true);
		Destroy (this);
	}
}
