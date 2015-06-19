using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Chair Game Controllr
 * Similar to the traditional chairs game, where players must try to sit as fast as possible. 
 * The number of chairs spawned is numPlayers-1, so that one player always dies.
 */
public class ChairGameRoomController : MonoBehaviour 
{	
	public float secondsToTrigger = 0f;
	public GameObject chair;

	bool triggered = false;
	List<GameObject> playersInside;
	List<GameObject> playersSit;

	void Start()
	{
		playersInside = new List<GameObject> ();
		playersSit = new List<GameObject> ();
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			playersInside.Add(other.gameObject);
			
			if(!triggered)
			{
				triggered = true;
				StartCoroutine("SpawnChairs");
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player") 
		{
			playersInside.Remove(other.gameObject);
		}
	}
	
	IEnumerator SpawnChairs() 
	{
		yield return new WaitForSeconds(secondsToTrigger);

		if (playersInside.Count < 2) {
			GetComponent<DoorsController>().TriggerDoors(true);
			Destroy(this);
			return true;
		}

		for (int i=0; i<playersInside.Count - 1; i++)
		{
			Vector3 roomPosition = transform.position;

			//random X
			float sideX = Random.value > 0.5 ? 1f : -1f;
			float sideZ = Random.value > 0.5 ? 1f : -1f;

			float sideXRange = Random.Range(i, i+1);
			float sideZRange = Random.Range(i, i+1);

			roomPosition.x += sideX * sideXRange;
			roomPosition.y += 0.5f;
			roomPosition.z += sideZ * sideZRange;

			float randomAngle = Random.Range(0, 360);

			GameObject chairObj = Instantiate(chair, roomPosition, transform.rotation) as GameObject;
			chairObj.transform.parent = transform;
			chairObj.transform.localEulerAngles = new Vector3(-90f, randomAngle, 0);
			chairObj.GetComponent<ChairController>().SetController(this);
		}
	}
	
	public void ChairEnter(GameObject player)
	{
		playersSit.Add (player);

		if (playersSit.Count == playersInside.Count - 1)
		{
			//Kill left player
			foreach(GameObject playerInside in playersInside)
			{
				if(!playersSit.Contains(playerInside) && playerInside == NetworkManager.instance.GetPlayer()) {
					playerInside.GetComponent<PlayerNetworkManager>().TakeDamage(1000);
				}	
			}

			GetComponent<DoorsController>().TriggerDoors(true);
			Destroy(this);
		}
	}
	
	public void ChairExit(GameObject player)
	{
		playersSit.Remove (player);
	}
}