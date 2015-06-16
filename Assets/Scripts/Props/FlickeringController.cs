using UnityEngine;
using System.Collections;

public class FlickeringController : MonoBehaviour {

	public float amplitude = 1f;
	public float frequency = 0.5f;

	private Light light;
	private Color originalColor;

	void Start () {
		light = GetComponent<Light>(); 
		originalColor = light.color;
	}
	
	// Update is called once per frame
	void Update () {

		float x = Time.time * frequency;
		x = x - Mathf.Floor (x); //normalize

		float y = 1 - (Random.value * 2);

		float oscilation = y * amplitude + 1f;

		light.color = originalColor * oscilation;
	}
}
