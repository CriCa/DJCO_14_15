using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

/* 
 * Network Manager
 * Responsible for sending player movement info over the network.
 * Also includes all info regarding a player's children objects.
 */
public class PlayerNetworkManager : Photon.MonoBehaviour 
{
	public float smoothing = 10f; // lerping movement update "speed"
	public float snappingDistance = 5f; // if distance between updates is too big, we want to snap to the new pos directly

	Vector3 positionGoal = Vector3.zero; // lerping destination
	Quaternion rotationGoal = Quaternion.identity; // we don't want to go directly to the new pos, we want to move towards it

	Light spotlight; // light from the flashlight
	Quaternion spotlightRotationGoal = Quaternion.identity; // flashlight rotation lerping destination

	Animator anim; // animation controller
	bool animWalking = false;
	bool animRunning = false;


	// using Awake, and not Start, because OnPhotonSerializeView may run before Start has finished
	void Awake() 
	{
		// global references (for both local and networked player instances)
		anim = GetComponent<Animator>();
		spotlight = GetComponentInChildren<Light>();

		// if it's a local object, we want to enable all controls
		if(photonView.isMine) 
		{
			// lock cursor and hide it
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			// enable general components
			GetComponent<Rigidbody>().useGravity = true;
			GetComponent<AudioSource>().enabled = true;
			GetComponent<AudioListener>().enabled = true;
			GetComponent<FirstPersonController>().enabled = true;
			GetComponent<PlayerControlsManager>().enabled = true;

			// enable all cameras
			foreach (Camera cam in GetComponentsInChildren<Camera>()) 
			{
				cam.enabled = true;
			}

			// hide own model
			// transform.Find("Model").GetComponent<SkinnedMeshRenderer>().enabled = false;

			// draw items on top of everything (hud-like)
			transform.Find("Items/Flashlight/FlashlightModel").gameObject.layer = 8;
			transform.Find("Items/Weapon/WeaponModel").gameObject.layer = 8;

			// enable all item controllers
			GetComponentInChildren<FlashlightController>().enabled = true;
			GetComponentInChildren<RotationFollower>().enabled = true;
			GetComponentInChildren<ShootingController>().enabled = true;
		}

		// otherwise, we want to update our object with the info we might receive
		else 
		{
			StartCoroutine("UpdateData");
		}
	}


	IEnumerator UpdateData() 
	{
		// continuously update the positions of all players in the game
		while(true) 
		{
			if (Vector3.Distance(transform.position, positionGoal) > snappingDistance) 
			{
				transform.position = positionGoal;
			}
			else 
			{
				transform.position = Vector3.Lerp(transform.position, positionGoal, Time.deltaTime * smoothing);
			}

			transform.rotation = Quaternion.Lerp(transform.rotation, rotationGoal, Time.deltaTime * smoothing);
			spotlight.transform.rotation = Quaternion.Lerp(spotlight.transform.rotation, spotlightRotationGoal, Time.deltaTime * smoothing);

			anim.SetBool("Walking", animWalking);
			anim.SetBool("Running", animRunning);

			yield return null;
		}
	}


	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
	{
		// if the stream is writing, player is local and we should tell others to update according to our info
		if(stream.isWriting) 
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);

			stream.SendNext(spotlight.transform.rotation);
			stream.SendNext(spotlight.intensity);
			stream.SendNext(spotlight.enabled);

			stream.SendNext(anim.GetBool("Walking"));
			stream.SendNext(anim.GetBool("Running"));
		}

		// otherwise, we need to update this player according to its owner
		else 
		{
			positionGoal = (Vector3)stream.ReceiveNext();
			rotationGoal = (Quaternion)stream.ReceiveNext();

			spotlightRotationGoal = (Quaternion)stream.ReceiveNext();
			spotlight.intensity = (float)stream.ReceiveNext();
			spotlight.enabled = (bool)stream.ReceiveNext();

			animWalking = (bool)stream.ReceiveNext();
			animRunning = (bool)stream.ReceiveNext();
		}
	}


	public void TriggerAnimation(string animation) {
		// trigger locally first, so we don't waste time waiting for the rpc
		anim.SetTrigger(animation);

		// warn all others
		photonView.RPC("TriggerAnimation_RPC", PhotonTargets.Others, animation);
	}


	[RPC]
	void TriggerAnimation_RPC(string animation) {
		anim.SetTrigger(animation);
	}
}