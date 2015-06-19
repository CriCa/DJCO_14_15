using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using System.Collections.Generic;

/* 
 * Player Controls Manager
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

	private bool isPlayerMonster = false;
	private float currentHP;
	private HudDamageController hudDamageCtrl; // flash indication when player gets shot

	private FirstPersonController fpController;
	private FlashlightController flashController;
	private ShootingController shootController;
	private List<Camera> fpCameras;

	private Transform playerModel;
	private Transform monsterModel;
	private SkinnedMeshRenderer playerMeshRenderer; // all these are necessary for when turning into a monster
	private SkinnedMeshRenderer monsterMeshRenderer;
	private Animator playerAnimator;
	private Animator monsterAnimator;


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
				output.text = "Currently paused...";
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
			output.text = "Currently paused...";
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
			// if monster, maintain items disabled
			if (isPlayerMonster) {
				fpController.enabled = true;
			}

			// otherwise, we're good to go
			else {
				fpController.enabled = true;
				flashController.enabled = true;
				shootController.enabled = true;
			}
		}
	}

	public void DisableControls() {
		fpController.enabled = false;
		flashController.enabled = false;
		shootController.enabled = false;
	}


	public void EnableCameras() {
		// if monster, enable only first person camera
		if (isPlayerMonster) {
			fpCameras[1].enabled = true;
		}

		// otherwise, enable all cameras including items
		else {
			foreach (Camera cam in fpCameras) {
				cam.enabled = true;
			}
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
			//PhotonNetwork.Instantiate("DeadParticles", transform.position, transform.rotation, 0);
			NetworkManager.instance.RespawnPlayer();
		}
	}


	public void ResetCurrentHP() {
		currentHP = maxHP;
	}


	public void TransformIntoMonster() {
		// should only transform once
		if (isPlayerMonster) {
			return;
		}

		// set new state
		isPlayerMonster = true;

		// change appearance
		TransformIntoMonsterAppearance();

		// reset monster model back to a visible layer
		monsterModel.transform.Find("Cube.001").gameObject.layer = 0;

		// update colliders' sizes to fit monster
		BoxCollider playerCollider = playerModel.GetComponent<BoxCollider>();
		Vector3 colliderCenter = playerCollider.center;
		Vector3 colliderSize = playerCollider.size;

		colliderCenter.y = -0.62f;
		colliderSize.y = 3.29f;
		colliderSize.z = 5.37f;

		playerCollider.center = colliderCenter;
		playerCollider.size = colliderSize;

		// enable new attack controller, but only locally
		if (GetComponent<PlayerNetworkManager>().photonView.isMine) {
			monsterModel.GetComponent<BoxCollider>().enabled = true;
		}

		// hide all items and disable respective controllers
		Transform items = transform.Find("Items");
		items.GetComponentInChildren<Camera>().enabled = false;
		items.GetComponentInChildren<Light>().enabled = false;
		items.GetComponentInChildren<FlashlightController>().enabled = false;
		items.GetComponentInChildren<ShootingController>().enabled = false;

		// disable item mesh renderers
		items.Find("Flashlight/FlashlightModel").GetComponent<MeshRenderer>().enabled = false;
		items.Find("Weapon/WeaponModel").GetComponent<MeshRenderer>().enabled = false;
	}


	public void TransformIntoMonsterAppearance() {
		// if this is null, this is the first time the player is transforming and we should grab all references
		if (playerModel == null) {
			playerAnimator = GetComponent<Animator>();

			playerModel = transform.Find("PlayerModel");
			playerMeshRenderer = playerModel.GetComponent<SkinnedMeshRenderer>();

			monsterModel = transform.Find("MonsterModel");
			monsterModel.gameObject.SetActive(true);
			monsterAnimator = monsterModel.GetComponent<Animator>();
			monsterMeshRenderer = monsterModel.GetComponentInChildren<SkinnedMeshRenderer>();
		}

		// hide player model
		playerMeshRenderer.enabled = false;
		
		// set monster visible
		monsterModel.gameObject.SetActive(true);
		monsterMeshRenderer.enabled = true;
		monsterAnimator.enabled = true;

		// start updating monster's animation according to player's
		StartCoroutine("UpdateMonsterAnimator");
	}


	public void TransformIntoHumanAppearance() {
		// stop monster's animation
		StopCoroutine("UpdateMonsterAnimator");

		// hide monster model
		monsterAnimator.enabled = false;
		monsterMeshRenderer.enabled = false;
		monsterModel.gameObject.SetActive(false);
		
		// set player visible
		playerMeshRenderer.enabled = true;
	}


	IEnumerator UpdateMonsterAnimator() {
		while (true) {
			bool isRunning = playerAnimator.GetBool("Walking") || playerAnimator.GetBool("Running");

			monsterAnimator.SetBool("Running", isRunning);

			yield return null;
		}
	}
}
