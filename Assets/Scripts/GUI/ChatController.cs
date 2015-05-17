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
public class ChatController : MonoBehaviour 
{	
	public Text output; // output field
	public int maxMessages = 4; // maximum number of messages on screen at a time
	public KeyCode activationKey = KeyCode.Y; // keypress needed to activate input
	
	PhotonView photonView; // needed for RPC
	FirstPersonController fpController; // player controls should be disabled when writing a message
	FlashlightController flController;
	string playerName;

	InputField input;
	Queue<string> messages;
	bool isSelected; // provides more control over input selection
	
	void Start () {
		photonView = GetComponent<PhotonView>();
		fpController = NetworkManager.instance.GetPlayer().GetComponent<FirstPersonController>();
		flController = NetworkManager.instance.GetPlayer().GetComponentInChildren<FlashlightController>();
		playerName = NetworkManager.instance.GetPlayerName();

		input = this.GetComponent<InputField>();
		messages = new Queue<string>();
		isSelected = false;
	}

	void Update () {
		if (!isSelected && Input.GetKeyDown(activationKey)) {
			SelectInput();
		}

		else if (isSelected && Input.GetKeyDown(KeyCode.Return)) {
			AddMessage(playerName + ": " + input.text);
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

	void AddMessage(string message) {
		if (message != "" && message.Length <= 80) {
			photonView.RPC("AddMessage_RPC", PhotonTargets.All, message);
		}
	}
	
	[RPC]
	void AddMessage_RPC(string message) {
		messages.Enqueue(message);
		
		if (messages.Count > maxMessages) {
			messages.Dequeue();
		}
		
		output.text = "";
		
		foreach (string m in messages) {
			output.text += m + "\n";
		}
	}
}
