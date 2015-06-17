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
	/* these probably be moved later */
	public KeyCode pauseKey = KeyCode.P; // keypress needed to toggle the menu
	Text output; 
	bool gamePaused;
	/* until here */

	private FirstPersonController fpController;
	private FlashlightController flashController;
	private ShootingController shootController;
	private List<Camera> fpCameras;


	void Start() {
		output = GameObject.FindGameObjectWithTag("GameHint").GetComponent<Text>();
		gamePaused = false;

		fpController = GetComponent<FirstPersonController>();
		flashController = GetComponentInChildren<FlashlightController>();
		shootController = GetComponentInChildren<ShootingController>();
		fpCameras = new List<Camera>();

		foreach (Camera cam in GetComponentsInChildren<Camera>()) {
			fpCameras.Add(cam);
		}
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

	void OnApplicationFocus(bool focusStatus) {
		if (!focusStatus) {
			gamePaused = true;
			DisableControls();
			output.enabled = true;
			output.text = "Currently paused....";
		}
	}
	/* until here */ 


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
}
