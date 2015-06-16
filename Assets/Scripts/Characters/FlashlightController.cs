using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/* 
 * Flashlight Controller
 * Listens to user input in order to toggle a spotlight.
 */
public class FlashlightController : MonoBehaviour
{
	public Light spotlight; // flashlight's light
	public KeyCode activationKey = KeyCode.E; // keypress needed to toggle the light
	public float drainSpeed = 2.5f; // speed at which charge is depleted

	float maxIntensity; // default spotlight intensity
	float maxCharge = 100f; // default charge level
	float currentCharge; // current charge level
	Image chargeGUI; // charge level bar


	void Start() {
		maxIntensity = spotlight.intensity;
		currentCharge = maxCharge;
		chargeGUI = GameObject.FindGameObjectWithTag("FlashlightCharge").GetComponent<Image>();
	}


	void Update () {
		if (Input.GetKeyDown(activationKey)) {
			spotlight.enabled = !spotlight.enabled;
		}

		if (spotlight.enabled && currentCharge > 0) {
			currentCharge -= drainSpeed * Time.deltaTime;

			spotlight.intensity = (currentCharge * maxIntensity) / maxCharge;
			chargeGUI.fillAmount = currentCharge / 100;
		}
	}
}
