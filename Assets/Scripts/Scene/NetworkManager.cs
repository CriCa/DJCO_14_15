using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* 
 * Network Manager
 * Responsible for initial network connection and general methods.
 */
public class NetworkManager : MonoBehaviour 
{
	public static NetworkManager instance = null; // singleton object

	public string VERSION; // current game version
	public int secondsToStart = 30; // seconds to start game once enough players have joined
	public float respawnTime = 5f; // interval between player death and respawn
	public int minPlayers = 4;

	[SerializeField] private Text connText;
	[SerializeField] private Camera spawnCamera;

	private int roomSeed; // stored on the server, used as the seed for random values
	private GameObject player; // reference to local player
	private PlayerControlsManager playerControls; // reference to local player controls
	private bool playerAlive = true;


	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject); 
		}
	}


	void Start() {
		PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
		PhotonNetwork.ConnectUsingSettings(VERSION);

		StartCoroutine("UpdateConnectionText");
	}


	IEnumerator UpdateConnectionText() {
		while (true) {
			connText.text = PhotonNetwork.connectionStateDetailed.ToString();
			yield return null;
		}
	}


	void OnJoinedLobby() {
		System.Random rndGenerator = new System.Random();

		PhotonNetwork.playerName = "Player " + rndGenerator.Next(1, 10000);
		ChatManager.instance.SetPlayerName(PhotonNetwork.playerName);

		RoomOptions ro = new RoomOptions() {isVisible = true, maxPlayers = 5};
		PhotonNetwork.JoinOrCreateRoom("Default", ro, TypedLobby.Default);
	}


	void OnCreatedRoom() {
		// generate map
		MapManager.instance.GenerateMap();
		int[] map = MapManager.instance.GetMinimizedMap();

		// generate random room seed
		roomSeed = Random.Range(0, 1000);

		ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
		roomProperties.Add("map", map);
		roomProperties.Add("started", false);
		roomProperties.Add("seed", roomSeed);

		// set custom properties
		PhotonNetwork.room.SetCustomProperties(roomProperties);
	}


	// this also gets called when a player joins a room, when the properties are first set
	void OnPhotonCustomRoomPropertiesChanged() {

	}


	void OnPhotonCreateRoomFailed(object[] codeAndMsg) {

	}


	void OnJoinedRoom() {
		// get server settings
		ExitGames.Client.Photon.Hashtable roomProperties = PhotonNetwork.room.customProperties;

		// we no longer need connection info, so the field can be used for other stuff
		StopCoroutine("UpdateConnectionText");
		connText.text = "Instantiating map";

		// player that created room could join before creating, so we need to check for null
		// should place a failsafe here
		if (roomProperties["map"] != null && roomProperties["seed"] != null) {
			int[] map = (int[]) roomProperties["map"];
			roomSeed = (int) roomProperties["seed"];

			// set seed for room generation
			Random.seed = roomSeed;

			// we have the map info, so we can instantiate the rooms
			MapManager.instance.FillMap(map);
			MapManager.instance.SpawnMap();

			connText.text = "";
		}
		
		// spawn player
		SpawnPlayer();

		// enable chat
		GetComponent<ChatManager>().enabled = true;
	}


	void OnPhotonJoinRoomFailed(object[] codeAndMsg) {

	}


	void OnPhotonRandomJoinFailed(object[] codeAndMsg) {

	}


	void OnPhotonPlayerConnected(PhotonPlayer player) {
		ChatManager.instance.AddInfoMessage(player.name + " just connected.");
	}


	void OnPhotonPlayerDisconnected(PhotonPlayer player) {
		ChatManager.instance.AddWarningMessage(player.name + " has left the game.");
	}


	void OnPhotonMaxCccuReached() {

	}


	void SpawnPlayer() {
		// disable spawn camera
		spawnCamera.enabled = false;

		// spawn player
		int playerNum = PhotonNetwork.room.playerCount;
		Vector3 pos = new Vector3 (-3 + 2 * PhotonNetwork.room.playerCount, 0.98f, 0f);
		player = PhotonNetwork.Instantiate("HybridPlayer" + playerNum, pos, Quaternion.identity, 0);
		playerControls = player.GetComponent<PlayerControlsManager>();
	}


	public void RespawnPlayer() {
		// if the player is already being spawned, we should ignore new requests
		if (!playerAlive) {
			return;
		}

		// move player model to a temporary location
		player.transform.position = new Vector3(0f, -2000f, 0f);

		// disable all controls
		playerAlive = false;
		playerControls.DisableControls();
		playerControls.DisableCameras();

		// enable spawn camera
		spawnCamera.enabled = true;

		// warn player, if needed
		ChatManager.instance.AddWarningMessage("You died. get gud, scrub.");

		// and start the actual spawn process
		StartCoroutine("StartRespawnProcess");
	}


	IEnumerator StartRespawnProcess() {
		yield return new WaitForSeconds(respawnTime);

		// get current spawn point and use it to spawn player
		Vector3 spawnRoom = GameObject.FindGameObjectWithTag("SpawnRoom").transform.position;
		player.transform.position = spawnRoom + new Vector3(-3 + 2 * PhotonNetwork.room.playerCount, 0.98f, 0f);

		// reset HP
		playerControls.ResetCurrentHP();

		// disable spawn camera
		spawnCamera.enabled = false;

		// re-enable controls
		playerAlive = true;
		playerControls.EnableCameras();
		playerControls.EnableControls();
	}


	public GameObject GetPlayer() {
		return this.player;
	}


	public PlayerControlsManager GetPlayerControls() {
		return this.playerControls;
	}


	public string GetPlayerName() {
		return PhotonNetwork.player.name;
	}


	public void SetPlayerName(string playerName) {
		PhotonNetwork.playerName = playerName;
	}


	public int getRoomSeed() {
		return this.roomSeed;
	}
}
