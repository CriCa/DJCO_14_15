using UnityEngine;
using System.Collections;

public class PressurePlateController : MonoBehaviour {

	public Color selectedColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
	int pos;
	Material mat;

	// Use this for initialization
	void Start () {
		//mat = GetComponent<Renderer>().materials[0];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetPosition(int pos)
	{
		this.pos = pos;

		if (this.pos > 0) 
		{
			// Debug.Log ("Pos: " + pos);
			//mat.SetColor("_RimColor", selectedColor);
			transform.position += new Vector3(0, 1, 0);
		}
	}
}
