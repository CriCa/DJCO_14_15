using UnityEngine;
using System.Collections;

public class TVScreenController : MonoBehaviour {

	private float speed = 0.1f;
	private Renderer tvRenderer;

	// Use this for initialization
	void Start () {
		tvRenderer = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		float offset = Random.Range (0, Time.time * speed);
		tvRenderer.material.SetTextureOffset ("_MainTex", new Vector2 (offset, 0));
	}
}
