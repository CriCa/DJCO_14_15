using UnityEngine;
using System.Collections;

/* 
 * Chair Controller
 * Represents a chair in the Chair Game Room. Alerts the room's script when a player enters its trigger.
 */
public class ChairController : MonoBehaviour 
{
	ChairGameRoomController controller;
	
	void OnTriggerEnter(Collider other)
	{
		if (controller != null && other.tag == "PlayerBody")
			controller.ChairEnter();
	}
	
	void OnTriggerExit(Collider other)
	{
		if (controller != null && other.tag == "PlayerBody")
			controller.ChairExit();
	}
	
	public void SetController(ChairGameRoomController controller)
	{
		this.controller = controller;
	}
}