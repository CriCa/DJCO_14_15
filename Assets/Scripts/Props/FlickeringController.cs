using UnityEngine;
using System.Collections;

public class FlickeringController : MonoBehaviour {

	public float amplitude = 1f;
	public float frequency = 0.5f;

	private Light pointLight;
	private Color originalColor;

	void Start () {
		pointLight = GetComponent<Light>(); 
		originalColor = pointLight.color;
	}
	
	// Update is called once per frame
	void Update () {

		float x = Time.time * frequency;
		x = x - Mathf.Floor (x); //normalize

		float y = 1 - (Random.value * 2);

		float oscilation = y * amplitude + 1f;

		pointLight.color = originalColor * oscilation;
	}
}
