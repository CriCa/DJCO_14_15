using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Peekaboo Room Controller
 * After a set amount of seconds, the lights go out and all players' textures change to a monster's.
 * Players need to be able to understand the monsters in the room are not real ones.
 */
public class PeekabooRoomController : MonoBehaviour 
{
	public float secondsToTrigger = 6f;
	public float secondsTransformed = 15f;
	public Texture monsterTexture; // testing purposes only

	bool triggered;
	List<GameObject> players; // reference to players currently inside the room


	void Start() {
		players = new List<GameObject>();
		triggered = false;
	}


	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			players.Add(other.gameObject);

			// when the first player enters, start process
			if (!triggered) {
				triggered = true;
				StartCoroutine("TransformIntoMonsters");
			}
		}
	}


	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			players.Remove(other.gameObject);
		}
	}


	IEnumerator TransformIntoMonsters() {
		yield return new WaitForSeconds(secondsToTrigger);

		foreach (GameObject player in players) {
			player.GetComponent<PlayerControlsManager>().TransformIntoMonsterAppearance(false);
		}

		StartCoroutine("TransformIntoPlayers");
	}


	// although similar to the above for now, leaving it like this because they'll probably be different later
	IEnumerator TransformIntoPlayers() {
		yield return new WaitForSeconds(secondsTransformed);
		
		foreach (GameObject player in players) {
			player.GetComponent<PlayerControlsManager>().TransformIntoHumanAppearance(false);
		}

		GetComponent<DoorsController>().TriggerDoors(true);
		Destroy(this);
	}
}
