using UnityEngine;
using System.Collections;

public class TVScreenController : MonoBehaviour {

	private float speed = 0.5f;
	private Renderer renderer;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		float offset = Random.Range (0, Time.time * speed);
		renderer.material.SetTextureOffset ("_MainTex", new Vector2 (offset, 0));
	}
}
