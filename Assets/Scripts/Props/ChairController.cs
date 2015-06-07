using UnityEngine;
using System.Collections;

public class ChairController : MonoBehaviour {
	
	private ChairGameRoomController controller;
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "PlayerBody")
			controller.ChairEnter();
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "PlayerBody")
			controller.ChairExit();
	}
	
	public void SetController(ChairGameRoomController controller)
	{
		this.controller = controller;
	}
}