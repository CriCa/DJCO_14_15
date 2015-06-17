using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/* 
 * Chat Controller
 * Responsible for listening to player input and outputing to the GUI.
 * Alerts all players whenever a new message has been set via RPC.
 * May also be used by other components to inform the player whenever a given event takes place.
 */
public class ChatManager : MonoBehaviour 
{	
	public static ChatManager instance = null; // singleton object

	public InputField input; // input field
	public Text output; // output field
	public int maxMessages = 4; // maximum number of messages on screen at a time
	public KeyCode activationKey = KeyCode.Y; // keypress needed to activate input
	
	private PhotonView photonView; // needed for RPC
	private PlayerControlsManager playerControls; // player controls should be disabled when writing a message

	private Queue<string> messages;
	private string playerName;
	private bool isSelected; // provides more control over input selection
	private int maxMessageLength = 80;
	private string infoColor = "#2EE62E";
	private string warningColor = "#F72929";


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
		playerControls = NetworkManager.instance.GetPlayerControls();
		
		messages = new Queue<string>();
		playerName = NetworkManager.instance.GetPlayerName();
		isSelected = false;
	}


	void Update () {
		if (!isSelected && Input.GetKeyDown(activationKey)) {
			SelectInput();
		}

		else if (isSelected && Input.GetKeyDown(KeyCode.Return)) {
			if (input.text != "" && input.text.Length <= maxMessageLength) {
				string message = BoldText(playerName) + ": " + input.text;
				photonView.RPC("AddMessage_RPC", PhotonTargets.All, message);
			}

			DeselectInput();
		}
		
		else if (isSelected && Input.GetKeyDown(KeyCode.Escape)) {
			DeselectInput();
		}
	}


	void SelectInput() {
		playerControls.DisableControls();
		input.interactable = true;
		input.ActivateInputField();
		input.Select();
		isSelected = true;
	}


	void DeselectInput() {
		playerControls.EnableControls();
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


	public void AddInfoMessage(string message) {
		message = ColorText(message, infoColor);
		message = BoldText(message);
		AddMessage(message);
	}


	public void AddWarningMessage(string message) {
		message = ColorText(message, warningColor);
		message = BoldText(message);
		AddMessage(message);
	}


	public void SetPlayerName(string playerName) {
		this.playerName = playerName;
	}


	string BoldText(string text) {
		return "<b>" + text + "</b>";
	}


	string ColorText(string text, string color) {
		return "<color=" + color + ">" + text + "</color>";
	}


	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

	}
}
