using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchRoomController : MonoBehaviour 
{
	public float secondsToTrigger = 5f;
	public GameObject monster;

	private List<Transform> players;
	private MonsterFollowerController monsterController;


	void Start () {
		players = new List<Transform>();
		monsterController = monster.GetComponent<MonsterFollowerController>();
	}


	void OnTriggerEnter(Collider other) {
		if (other.tag == "PlayerBody") {
			players.Add(other.transform);
			UpdateTarget();
		}
	}


	void OnTriggerExit(Collider other) {
		if (other.tag == "PlayerBody") {
			players.Remove(other.transform);
			UpdateTarget();
		}
	}


	void UpdateTarget() {
		// if we have no players inside, monster should be stopped
		if (players.Count == 0) {
			monsterController.StopFollowing();
		}

		// otherwise, update target and let monster handle current/new targets internally
		else {
			monsterController.SetTarget(players[0]);
		}
	}
}
