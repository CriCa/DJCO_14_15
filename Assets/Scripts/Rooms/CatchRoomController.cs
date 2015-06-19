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
		}
	}


	void OnTriggerExit(Collider other) {
		if (other.tag == "PlayerBody") {
			players.Remove(other.transform);
		}
	}

	void Update()
	{
		if (players.Count == 0) {
			monsterController.StopFollowing ();
		}
		else
		{
			float closestDistance = Mathf.Infinity;
			Transform closestTransform = null;

			foreach(Transform player in players)
			{
				float distance = Vector3.Distance(player.transform.position, monster.transform.position);

				if(distance < closestDistance)
					closestTransform = player;
			}

			monsterController.SetTarget(closestTransform);
		}

	}
}
