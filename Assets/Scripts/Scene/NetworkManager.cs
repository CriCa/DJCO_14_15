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

	[SerializeField] Text connText;
	[SerializeField] Camera spawnCamera;

	GameObject player; // reference to local player

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
		PhotonNetwork.playerName = "Player " + Random.Range(0, 9999);
		ChatManager.instance.SetPlayerName(PhotonNetwork.playerName);

		RoomOptions ro = new RoomOptions() {isVisible = true, maxPlayers = 5};
		PhotonNetwork.JoinOrCreateRoom("Default", ro, TypedLobby.Default);
	}

	void OnCreatedRoom() {
		// generate map
		MapManager.instance.GenerateMap();
		int[] map = MapManager.instance.GetMinimizedMap();

		ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
		roomProperties.Add("m", map);

		// set map in room properties
		PhotonNetwork.room.SetCustomProperties(roomProperties);
	}

	void OnPhotonCustomRoomPropertiesChanged() {
		// this also gets called when a player joins a room, when the properties are first set

	}

	void OnPhotonCreateRoomFailed(object[] codeAndMsg) {

	}

	void OnJoinedRoom() {
		// get server settings, including map
		ExitGames.Client.Photon.Hashtable roomProperties = PhotonNetwork.room.customProperties;

		// we no longer need connection info, so the field can be used for other stuff
		StopCoroutine("UpdateConnectionText");
		connText.text = "Instantiating map";

		// player that created room could join before creating, so we need to check for null
		if (roomProperties["m"] != null) {
			int[] map = (int[]) roomProperties["m"];

			// we have the map info, so we can instantiate the rooms
			MapManager.instance.FillMap(map);
			MapManager.instance.SpawnMap();

			connText.text = "";
		}
		
		// spawn player
		spawnCamera.enabled = true;
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

	void StartSpawnProcess(float respawnTime) {
		spawnCamera.enabled = true;
		StartCoroutine("SpawnPlayer", respawnTime);
	}
	
	IEnumerator WaitAndSpawn(float respawnTime) {
		yield return new WaitForSeconds(respawnTime);
		SpawnPlayer();
	}

	void SpawnPlayer() {
		// spawn player
		Vector3 pos = new Vector3 (-3 + 2 * PhotonNetwork.room.playerCount, 0.98f, 0f);
		player = PhotonNetwork.Instantiate("PlayerModel", pos, Quaternion.identity, 0);
		
		// disable spawn camera
		spawnCamera.enabled = false;
	}

	public GameObject GetPlayer() {
		return this.player;
	}

	public string GetPlayerName() {
		return PhotonNetwork.player.name;
	}

	public void SetPlayerName(string playerName) {
		PhotonNetwork.playerName = playerName;
	}
}
