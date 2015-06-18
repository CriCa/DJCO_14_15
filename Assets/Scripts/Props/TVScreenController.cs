using UnityEngine;
using System.Collections;

public class TVScreenController : MonoBehaviour {
	private Renderer tvRenderer;
	private System.Random rndGenerator;

	// Use this for initialization
	void Start () {
		tvRenderer = GetComponent<Renderer> ();
		rndGenerator = new System.Random();
	}
	
	// Update is called once per frame
	void Update () {
		float offset = (float)rndGenerator.Next();
		tvRenderer.material.SetTextureOffset ("_MainTex", new Vector2 (offset, 0));
	}
}