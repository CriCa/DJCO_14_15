using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Randomizer Room Controller
 * Switches the position of several rooms, randomizing the level.
 * TODO: close doors in all roooms?
 * TODO: refactor
 */
public class RandomizerRoom : MonoBehaviour 
{
	public float secondsToTrigger = 2f;
	public float secondsInTheDark = 5f;
	public int requiredSwitches; // number of switches that should take place

	bool triggered;
	List<Transform> rooms;
	Transform world;

	void Start () {
		triggered = false;
		rooms = new List<Transform>();
		world = GameObject.FindGameObjectWithTag("WorldRooms").transform;
	}

	void OnTriggerEnter(Collider other) {
		if (!triggered && other.tag == "PlayerBody") {
			triggered = true;
			StartCoroutine("RandomizeRooms");
		}
	}

	IEnumerator RandomizeRooms() {
		yield return new WaitForSeconds(secondsToTrigger);

		// getting objects here, rather than on start, to avoid keep all rooms in memory
		foreach (Transform child in world) {
			if (child == this.transform) {
				continue;
			}

			rooms.Add(child);
		}

		int roomIndex1 = 0;
		int roomIndex2 = 0;
		Vector3 tempPos;

		// randomizing process
		for (int i = 0; i < requiredSwitches; i++) {
			roomIndex1 = Random.Range(0, rooms.Count);
			roomIndex2 = Random.Range(0, rooms.Count);

			tempPos = rooms[roomIndex1].position;

			rooms[roomIndex1].position = rooms[roomIndex2].position;
			rooms[roomIndex2].position = tempPos;
		}

		// no need to keep rooms anymore
		rooms.Clear();

		// wait before letting players walk out
		yield return new WaitForSeconds(secondsInTheDark);

		GetComponent<DoorsController>().TriggerDoors(true);
		Destroy(this);
	} 
}
