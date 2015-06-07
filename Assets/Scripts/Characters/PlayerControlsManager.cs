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

	FirstPersonController fpController;
	FlashlightController flController;
	List<Camera> fpCameras;
	
	void Start() {
		output = GameObject.FindGameObjectWithTag("GameHint").GetComponent<Text>();
		gamePaused = false;

		fpController = GetComponent<FirstPersonController>();
		flController = NetworkManager.instance.GetPlayer().GetComponentInChildren<FlashlightController>();
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
			}
		}
	}

	public void EnableControls() {
		if (!gamePaused) {
			fpController.enabled = true;
			flController.enabled = true;
		}
	}
	
	public void DisableControls() {
		fpController.enabled = false;
		flController.enabled = false;
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
