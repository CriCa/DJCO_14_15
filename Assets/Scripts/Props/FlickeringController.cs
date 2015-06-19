using UnityEngine;
using System.Collections;

public class FlickeringController : MonoBehaviour {

	public int delay = 2;
	public float amplitude = 1f;
	public float frequency = 0.5f;

	private Light pointLight;
	private Color originalColor;

	private System.Random rndGenerator;

	void Start () {
		pointLight = GetComponent<Light>(); 
		originalColor = pointLight.color;

		rndGenerator = new System.Random();
		StartCoroutine ("Flicker");
	}

	IEnumerator Flicker()
	{
		while (true)
		{
			yield return new WaitForSeconds(rndGenerator.Next(0, delay));

			float x = Time.time * frequency;
			x = x - Mathf.Floor (x); //normalize

			float rnd = (float)rndGenerator.NextDouble();

			float y = 1 - (rnd * 2);

			float oscilation = y * amplitude + 1f;

			pointLight.color = originalColor * oscilation;
		}
	}
}
