using UnityEngine;
using System.Collections;

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
	int playersInside = 0;
	int playersSit = 0;
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "PlayerBody")
		{
			playersInside++;
			
			if(!triggered)
			{
				triggered = true;
				StartCoroutine("SpawnChairs");
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "PlayerBody") 
		{
			playersInside = Mathf.Max(0, playersInside - 1);
		}
	}
	
	IEnumerator SpawnChairs() 
	{
		yield return new WaitForSeconds(secondsToTrigger);
		
		for (int i=0; i<playersInside; i++)
		{
			Vector3 roomPosition = transform.position;
			roomPosition.x += i;
			roomPosition.y += 0.5f;
			roomPosition.z += i;
			
			GameObject chairObj = Instantiate(chair, roomPosition, transform.rotation) as GameObject;
			chairObj.transform.parent = transform;
			chairObj.transform.localEulerAngles = new Vector3(-90f, 0, 0);
			chairObj.GetComponent<ChairController>().SetController(this);
		}
	}
	
	public void ChairEnter()
	{
		playersSit++;
		
		if (playersSit == playersInside) {
			GetComponent<DoorsController>().TriggerDoors(true);
			Destroy(this);
		}
	}
	
	public void ChairExit()
	{
		playersSit--;
	}
}