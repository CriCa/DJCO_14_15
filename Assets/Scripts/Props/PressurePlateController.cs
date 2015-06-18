using UnityEngine;
using System.Collections;

public class PressurePlateController : MonoBehaviour {

	public Color selectedColor;
	
	private int value;
	private Material material;
	private Color originalColor;

	private HopscotchRoomController controller;
	private static int downCount;

	// Use this for initialization
	void Awake () {
		material = GetComponent<Renderer>().material;
		originalColor = material.GetColor("_Color");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (this.value <= 0)
			return;

		if (downCount > 0) //at least 1 pressure plate is down
			material.SetColor ("_Color", originalColor);
		else //if no pressure plates are down, show path
			material.SetColor ("_Color", selectedColor);
	}

	public void SetInfo(HopscotchRoomController controller, int value)
	{
		this.controller = controller;
		this.value = value;
	}

	public void OnTriggerEnter(Collider other)
	{
		downCount++;

		if(controller != null)
			controller.PlatePressed(value);
	}

	public void OnTriggerExit(Collider other)
	{
		downCount--;
	}
}