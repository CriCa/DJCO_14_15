using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HopscotchRoomController : MonoBehaviour {

	public GameObject pressurePlate;
	public float plateSize;
	public int planeScale;
	public float spacement;

	//Matrix filled by algorithm
	private int[,] matrix;
	private Vector2 goal;
	private int matrixSize;
	private int goalValue;

	//Path progress
	private int progress = 0;

	//Falling spikes
	private FallingSpikesController spikesController;

	// Use this for initialization
	void Start ()
	{
		float planeSize = planeScale * plateSize;
		matrixSize = (int) (planeSize / plateSize);
		matrix = new int[matrixSize,matrixSize];
		goal = new Vector2 (matrixSize / 2, matrixSize / 2);

		GeneratePath(); //fills matrix
		
		for (int i=0; i < matrix.GetLength(0); i++) {
			for (int j=0; j < matrix.GetLength(1); j++)
			{
				GameObject pressurePlateObj = Instantiate (pressurePlate, Vector3.zero, transform.rotation) as GameObject;

				Vector3 scale = pressurePlateObj.transform.localScale;
				scale.x = plateSize;
				scale.z = plateSize;
				pressurePlateObj.transform.localScale = scale;

				pressurePlateObj.transform.parent = transform;

				float centeringOffset = planeSize/2f - plateSize/2f + (matrixSize-1)*spacement/2f;
				
				Vector3 position = new Vector3();
				position.x = i*plateSize + i*spacement - centeringOffset;
				position.z = j*plateSize + j*spacement - centeringOffset;
				pressurePlateObj.transform.localPosition = position;

				pressurePlateObj.GetComponent<PressurePlateController>().SetInfo(this, matrix[j, i]);
			}
		}

		spikesController = GetComponentInChildren<FallingSpikesController>();
	}

	private void GeneratePath()
	{
		Stack<Vector2> path = new Stack<Vector2>();

		Vector2 goalLeft = new Vector2(goal.x - 1, goal.y);
		Vector2 goalRight = new Vector2(goal.x + 1, goal.y);
		Vector2 goalUp = new Vector2(goal.x, goal.y -1);
		Vector2 goalDown = new Vector2(goal.x, goal.y + 1);
		
		//Choose start point
		Vector2 start = new Vector2();
		
		int side = Random.Range (0, 4);
		int place = Random.Range (0, matrixSize);
		
		start.x = side > 1 ? place : side * (matrixSize - 1);
		start.y = side > 1 ? (side - 2) * (matrixSize - 1) : place;
		
		path.Push(start);
		matrix[(int) start.x, (int) start.y] = 1;
		
		//build path
		while(true)
		{
			Vector2 current = path.Peek();
			
			//Check if around goal
			if(current == goalLeft || current == goalRight || current == goalUp || current == goalDown)
			{
				path.Push(goal);
				break;
			}
			
			//Check neighbours
			List<Vector2> validNext = new List<Vector2>();
			
			Vector2 left = new Vector2(current.x - 1, current.y);
			Vector2 right = new Vector2(current.x + 1, current.y);
			Vector2 up = new Vector2(current.x, current.y + 1);
			Vector2 down = new Vector2(current.x, current.y - 1);
			
			if(CheckIfValid(left)) validNext.Add(left);
			if(CheckIfValid(right)) validNext.Add(right);
			if(CheckIfValid(up)) validNext.Add(up);
			if(CheckIfValid(down)) validNext.Add(down);
			
			if(validNext.Count == 0)
			{
				matrix[(int) current.x, (int) current.y] = -1;
				path.Pop();
				
				//will never enter here (hopefully)
				if(path.Count == 0)
					break;
			}
			else
			{
				Vector2 next = validNext[Random.Range(0, validNext.Count)];
				path.Push(next);
				matrix[(int) next.x, (int) next.y] = 1;  
			}
		}
		
		//Now that we have a nicely computed stack, lets translate it into an actual path
		matrix = new int[matrixSize, matrixSize];
		Vector2[] pathArray = path.ToArray ();
		System.Array.Reverse (pathArray);

		for(int i=0; i<pathArray.Length; i++)
		{
			Vector2 pos = pathArray[i];
			matrix[(int) pos.x, (int) pos.y] = i+1; 
		}

		goalValue = pathArray.Length;
	}

	
	private bool CheckIfValid(Vector2 pos)
	{
		//check if out of range
		if (!checkBounds(pos))
			return false;
		
		//check if already visited
		if (matrix[(int) pos.x, (int) pos.y] != 0)
			return false;
		
		//check if not sticking
		if(GetStickingsCount(pos) > 1)
			return false;
		
		return true;
	}
	
	private int GetStickingsCount(Vector2 pos)
	{
		int stickCount = 0;
		
		Vector2 left = new Vector2(pos.x - 1, pos.y);
		Vector2 right = new Vector2(pos.x + 1, pos.y);
		Vector2 up = new Vector2(pos.x, pos.y + 1);
		Vector2 down = new Vector2(pos.x, pos.y - 1);
		
		if(checkBounds(left) && matrix[(int) left.x, (int) left.y] > 0)
			stickCount++;
		
		if(checkBounds(right) && matrix[(int) right.x, (int) right.y] > 0)
			stickCount++;
		
		if(checkBounds(up) && matrix[(int) up.x, (int) up.y] > 0)
			stickCount++;
		
		if(checkBounds(down) && matrix[(int) down.x, (int) down.y] > 0)
			stickCount++;
		
		return stickCount;
	}
	
	private bool checkBounds(Vector2 pos)
	{
		return !(pos.x < 0 || pos.y < 0 || pos.x > (matrixSize - 1) || pos.y > (matrixSize - 1));
	}

	public void PlatePressed(int value)
	{
		if (value == (progress + 1)) //Good Job!
		{
			progress++;

			if(progress == goalValue)
			{
				GetComponent<DoorsController>().TriggerDoors(true);
				Destroy(this);
			}
		} 
		else
		{
			spikesController.StartSmashProcess();
			progress = 0;
		}

	}
}
