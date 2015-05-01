using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public string VERSION;
	[SerializeField] Text connText;
	[SerializeField] Camera spawnCamera;

	GameObject player;

	// Use this for initialization
	void Start () {
		PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
		PhotonNetwork.ConnectUsingSettings (VERSION);
	}
	
	// Update is called once per frame
	void Update () {
		connText.text = PhotonNetwork.connectionStateDetailed.ToString();
	}

	void OnJoinedLobby() {
		RoomOptions ro = new RoomOptions () {isVisible = true, maxPlayers = 5};
		PhotonNetwork.JoinOrCreateRoom ("Default", ro, TypedLobby.Default);
	}

	void OnJoinedRoom() {
		StartSpawnProcess (0f);
	}

	void StartSpawnProcess (float respawnTime)
	{
		spawnCamera.enabled = true;
		StartCoroutine ("SpawnPlayer", respawnTime);
	}
	
	IEnumerator SpawnPlayer(float respawnTime)
	{
		yield return new WaitForSeconds(respawnTime);

		Vector3 pos = new Vector3 (-3 + 2*PhotonNetwork.room.playerCount,
		                          0.98f,
		                          0f);

		player = PhotonNetwork.Instantiate ("PlayerModel", 
		                                    pos,
		                                    Quaternion.identity,
		                                    0);

		spawnCamera.enabled = false;
	}
}
