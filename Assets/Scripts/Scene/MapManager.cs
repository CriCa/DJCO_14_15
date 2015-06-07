using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
 * Map Manager
 * Holds all the rooms in the map in a matrix of ints in order to improve server performance.
 * As the server will be holding the map in said matrix, it's easier to keep it in this format locally to avoid convertions.
 * This script is also responsible for the generation of the initial map (by the server creator) and rooms spawning (by all players).
 */
public class MapManager : MonoBehaviour 
{
	public static MapManager instance = null; // singleton object	

	public int mapSize; // nr of rooms, equal in both sides
	public GameObject[] roomTypes;
	public GameObject bridge;

	GameObject world; // where to instantiate the rooms
	float roomSpacement = 38.2f; // space between each room
	float bridgesSpacement = 19f; // space between bridge and room origin
	List<Vector2> mapPositions; // list of all possible positions on the map
	int[,] map; // grid with all rooms

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
	}

	public void GenerateMap() {
		InitializeMap();
	
		// EXAMPLE: force first room to be of type 1
		// mapPositions.RemoveAt(0);
		// map[0,0] = 0;

		// EXAMPLE: force at least three rooms of type 1
		// FillWithObject(1, 3);

		// forcing one type of each room, in order
		for (int i = 0; i < roomTypes.Length; i++) {
			mapPositions.RemoveAt(0);
			map[i/mapSize, i%mapSize] = i;
		}

		// all others can be anything
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
		int randomIndex = Random.Range(0, mapPositions.Count);
		Vector2 randomPos = mapPositions[randomIndex];

		// remove position from list so it can't be re-used
		mapPositions.RemoveAt(randomIndex);
		
		// return the position we got
		return randomPos;
	}

	// fills map with numberNeeded instances of objectType
	void FillWithObject(int objectType, int numberNeeded) {
		for (int i = 0; i < numberNeeded; i++) {
			Vector2 randomPos = GetRandomPosition();

			map[(int)randomPos.x, (int)randomPos.y] = objectType;
		}
	}

	void FillRemainingPositions() {
		// for each remaining position in the list, fill map with (TODO) random object
		foreach(Vector2 pos in mapPositions) {
			map[(int)pos.x, (int)pos.y] = 0;
		}

		// all positions have been filled at this point, so we can clear the positions list
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
		float floorHeight = 0;

		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				int type = map[i, j];
				Vector3 pos = new Vector3 (i * roomSpacement, floorHeight, j * roomSpacement);
				GameObject bridgeObj;

				GameObject roomObj = Instantiate(roomTypes[type], pos, Quaternion.identity) as GameObject;
				roomObj.transform.parent = world.transform;

				if (j != mapSize - 1) {
					pos = new Vector3 (i * roomSpacement, floorHeight, j * roomSpacement + bridgesSpacement);
					Quaternion rot = Quaternion.Euler(0, 90, 0);
					bridgeObj = Instantiate(bridge, pos, rot) as GameObject;
					bridgeObj.transform.parent = world.transform;
				}

				if (i != mapSize - 1) {
					pos = new Vector3 (i * roomSpacement + bridgesSpacement, floorHeight, j * roomSpacement);
					bridgeObj = Instantiate(bridge, pos, Quaternion.identity) as GameObject;
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
