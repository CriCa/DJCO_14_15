using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
 * Map Manager
 * Holds all the rooms in the map.
 * Allows the generation and spawning of a new map.
 */
public class MapManager : MonoBehaviour 
{
	public static MapManager instance = null; // singleton object	
	public int mapSize; // nr of rooms, equal in both sides
	public GameObject[] roomTypes;
	public GameObject bridge;

	GameObject world; // where to instantiate the rooms
	float roomSpacement = 36f; // space between each room
	List<Vector2> mapPositions; // list of all possible positions on the map
	int[,] map; // grid with all rooms

	// we need to know which prefabs to instantiate, so we need a list of matches
	Dictionary<int, GameObject> mapMatches;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject); 
		}
	}

	void Start () {
		world = GameObject.FindGameObjectWithTag("World");
		mapPositions = new List<Vector2>();
		map = new int[mapSize, mapSize];
		mapMatches = new Dictionary<int, GameObject>();

		for (int i = 0; i < roomTypes.Length; i++) {
			mapMatches.Add(i, roomTypes[i]);
		}
	}

	public void GenerateMap() {
		InitializeMap();
		FillWithObject(0, 1);
		FillWithObject(1, 1);
		FillWithObject(2, 2);
		FillRemainingPositions();
	}

	void InitializeMap() {
		mapPositions.Clear();

		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				mapPositions.Add(new Vector2(i, j));
			}
		}
	}

	Vector2 GetRandomPosition () {
		// grab a random list position (which represents a possible position on the map)
		int randomIndex = Random.Range (0, mapPositions.Count);
		Vector2 randomPos = mapPositions[randomIndex];

		// remove position from list so it can't be re-used
		mapPositions.RemoveAt(randomIndex);
		
		// return the position we got
		return randomPos;
	}

	void FillWithObject(int objectType, int numberNeeded) {
		for (int i = 0; i < numberNeeded; i++) {
			Vector2 randomPos = GetRandomPosition();

			map[(int)randomPos.x, (int)randomPos.y] = objectType;
		}
	}

	void FillRemainingPositions() {
		// for each remaining position in the list, fill map with random object
		foreach(Vector2 pos in mapPositions) {
			map[(int)pos.x, (int)pos.y] = 0;
		}

		// all positions have been filled at this point, so we can clear this
		mapPositions.Clear();
	}

	// returns a simple array, not a matrix, because Photon is unable to serialize a matrix
	public int[] GetMinimizedMap() {
		int[] minimizedMap = new int[mapSize * mapSize];
		int counter = 0;
		
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				minimizedMap[counter] = map[i,j];
				counter++;
			}
		}
		
		return minimizedMap;
	}

	public void FillMap(int[] newMap) {
		mapPositions.Clear();

		for (int i = 0; i < newMap.Length; i++) {
			map[i/mapSize, i%mapSize] = newMap[i];

		}

	}

	public void SpawnMap() {
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				Vector3 pos = new Vector3 (i * roomSpacement, 12, j * roomSpacement);
				GameObject bridgeObj;

				GameObject roomObj = Instantiate(roomTypes[0], pos, Quaternion.identity) as GameObject;
				roomObj.transform.parent = world.transform;

				if (j != mapSize - 1) {
					pos = new Vector3 (i * roomSpacement, 12, j * roomSpacement + 18);
					bridgeObj = Instantiate(bridge, pos, Quaternion.identity) as GameObject;
					bridgeObj.transform.parent = world.transform;
				}

				if (i != mapSize - 1) {
					pos = new Vector3 (i * roomSpacement + 18, 12, j * roomSpacement);
					Quaternion rot = Quaternion.Euler(0, 90, 0);
					bridgeObj = Instantiate(bridge, pos, rot) as GameObject;
					bridgeObj.transform.parent = world.transform;
				}
			}
		}	     
	}

	public void PrintMap() {
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				Debug.Log("Position [" + i + "," + j + "] has value " + map[i,j] + ".");
			}
		}
	}
}
