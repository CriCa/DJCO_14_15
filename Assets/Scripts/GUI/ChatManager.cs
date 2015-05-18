using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections.Generic;

/* 
 * Chat Controller
 * Responsible for listening to player input and outputing to the GUI.
 * Alerts all players whenever a new message has been set via RPC.
 */
public class ChatManager : MonoBehaviour 
{	
	public static ChatManager instance = null; // singleton object

	public InputField input; // input field
	public Text output; // output field
	public int maxMessages = 4; // maximum number of messages on screen at a time
	public KeyCode activationKey = KeyCode.Y; // keypress needed to activate input
	
	PhotonView photonView; // needed for RPC
	FirstPersonController fpController; // player controls should be disabled when writing a message
	FlashlightController flController;
	string playerName;

	Queue<string> messages;
	bool isSelected; // provides more control over input selection

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject); 
		}
	}
	
	void Start () {
		photonView = GetComponent<PhotonView>();
		fpController = NetworkManager.instance.GetPlayer().GetComponent<FirstPersonController>();
		flController = NetworkManager.instance.GetPlayer().GetComponentInChildren<FlashlightController>();
		playerName = NetworkManager.instance.GetPlayerName();
		
		messages = new Queue<string>();
		isSelected = false;
	}

	void Update () {
		if (!isSelected && Input.GetKeyDown(activationKey)) {
			SelectInput();
		}

		else if (isSelected && Input.GetKeyDown(KeyCode.Return)) {
			if (input.text != "" && input.text.Length <= 80) {
				string message = playerName + ": " + input.text;
				photonView.RPC("AddMessage_RPC", PhotonTargets.All, message);
			}

			DeselectInput();
		}
		
		else if (isSelected && Input.GetKeyDown(KeyCode.Escape)) {
			DeselectInput();
		}
	}

	void SelectInput() {
		fpController.enabled = false;
		flController.enabled = false;
		input.interactable = true;
		input.ActivateInputField();
		input.Select();
		isSelected = true;
	}

	void DeselectInput() {
		fpController.enabled = true;
		flController.enabled = true;
		input.text = "";
		input.interactable = false;
		input.DeactivateInputField();
		isSelected = false;
	}

	[RPC]
	void AddMessage_RPC(string message) {
		AddMessage(message);
	}

	public void AddMessage(string message) {
		messages.Enqueue(message);
		
		if (messages.Count > maxMessages) {
			messages.Dequeue();
		}
		
		output.text = "";
		
		foreach (string m in messages) {
			output.text += m + "\n";
		}
	}	

	public void SetPlayerName(string playerName) {
		this.playerName = playerName;
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

	}
}
