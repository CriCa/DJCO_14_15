using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using System.Collections.Generic;

/* 
 * Player Controller
 * General player controls, utilities and communication with other components.
 */
public class PlayerControlsManager : MonoBehaviour 
{
	/* these should probably be moved later */
	public KeyCode pauseKey = KeyCode.P; // keypress needed to toggle the menu
	Text output; 
	bool gamePaused;
	/* until here */

	public float maxHP = 3f;
	public float healInterval = 2f;
	public float healIncrement = 1f;

	private float currentHP;
	private HudDamageController hudDamageCtrl; // flash indication when player gets shot

	private FirstPersonController fpController;
	private FlashlightController flashController;
	private ShootingController shootController;
	private List<Camera> fpCameras;


	void Start() {
		// pause controls
		output = GameObject.FindGameObjectWithTag("GameHint").GetComponent<Text>();
		gamePaused = false;

		fpController = GetComponent<FirstPersonController>();
		flashController = GetComponentInChildren<FlashlightController>();
		shootController = GetComponentInChildren<ShootingController>();
		fpCameras = new List<Camera>();

		foreach (Camera cam in GetComponentsInChildren<Camera>()) {
			fpCameras.Add(cam);
		}

		// lock cursor and hide it
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		// health controls
		hudDamageCtrl = GameObject.FindGameObjectWithTag("HudDamage").GetComponent<HudDamageController>();
		currentHP = maxHP;
		StartCoroutine("UpdateHealth");
	}


	/* everything pertaining to the pause menu here should also be moved */
	void Update () {
		if (Input.GetKeyDown(pauseKey)) {
			if (!gamePaused) {
				gamePaused = true;
				DisableControls();
				output.enabled = true;
				output.text = "Currently paused....";
			}
			else {
				gamePaused = false;
				EnableControls();
				output.enabled = false;
				output.text = "";
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
	}


	// unity event, will run even with this script disabled
	// as such, before doing anything, we should check if this script is enabled
	void OnApplicationFocus(bool focusStatus) {
		if (this.enabled && !focusStatus) {
			gamePaused = true;
			DisableControls();
			output.enabled = true;
			output.text = "Currently paused....";
		}
	}
	/* until here */


	IEnumerator UpdateHealth() {
		while (true)  {
			yield return new WaitForSeconds(healInterval);
			currentHP = Mathf.Min(++currentHP, maxHP);
		}
	}


	public void EnableControls() {
		if (!gamePaused) {
			fpController.enabled = true;
			flashController.enabled = true;
			shootController.enabled = true;
		}
	}


	public void DisableControls() {
		fpController.enabled = false;
		flashController.enabled = false;
		shootController.enabled = false;
	}


	public void EnableCameras() {
		foreach (Camera cam in fpCameras) {
			cam.enabled = true;
		}
	}


	public void DisableCameras() {
		foreach (Camera cam in fpCameras) {
			cam.enabled = false;
		}
	}


	public void TakeDamage(float damage) {
		hudDamageCtrl.FlashDamage();
		currentHP -= damage;

		if (currentHP <= 0) {
			NetworkManager.instance.RespawnPlayer();
		}
	}


	public void ResetCurrentHP() {
		currentHP = maxHP;
	}
}
