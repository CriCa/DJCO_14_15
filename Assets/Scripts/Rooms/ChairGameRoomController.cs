using UnityEngine;
using System.Collections;

public class ChairGameRoomController : MonoBehaviour 
{	
	public float secondsToTrigger = 0f;
	public GameObject chair;

	private bool triggered = false;
	private int playersInside = 0;
	private int playersSit = 0;
	
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
			playersInside = Mathf.Max(0, playersInside - 1);
	}
	
	IEnumerator SpawnChairs() {
		yield return new WaitForSeconds(secondsToTrigger);
		
		for (int i=0; i<playersInside; i++)
		{
			Vector3 roomPosition = transform.position;
			roomPosition.x += Random.Range(-5, 5);
			roomPosition.z += Random.Range(-5, 5);
			
			GameObject chairObj = Instantiate(chair, roomPosition, transform.rotation) as GameObject;
			chairObj.GetComponent<ChairController>().SetController(this);
		}
		
		this.enabled = false; // disable script, as everything has already been done
	}
	
	public void ChairEnter()
	{
		playersSit++;
		
		if (playersSit == playersInside) {
			Debug.Log ("Open Doors");
			GetComponent<DoorsController>().TriggerDoors(true);
		}
	}
	
	public void ChairExit()
	{
		playersSit--;
	}
}