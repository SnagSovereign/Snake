using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour {

	// 0 = top wall
	// 1 = bottom wall
	// 2 = left wall
	// 3 = right wall
	[SerializeField] GameObject[] walls;

	[Header("Prefabs")]
	[SerializeField] GameObject apple;
	[SerializeField] GameObject tail;

	[Header("Game Parameters")]
	[SerializeField] float tickRate = 0.1f;
	[SerializeField] int startingLength;
	[SerializeField] Vector2 startingPos;
	[SerializeField] Vector2 gridSize;

	Vector2 direction = Vector2.right;
	bool gameOver = false;

	List<GameObject> tailObjects = new List<GameObject>();

	// Use this for initialization
	void Start()
	{
		// Scale the walls
		walls[0].transform.localScale = walls[1].transform.localScale = new Vector3(gridSize.x, 1f, 1f);
		walls[2].transform.localScale = walls[3].transform.localScale = new Vector3(1f, gridSize.y + 2f, 1f);
		// Position the walls
		walls[0].transform.position = walls[1].transform.position = new Vector3(0f, (gridSize.y + 1f) / 2f, 0f);
		walls[1].transform.position *= -1f;
		walls[2].transform.position = walls[3].transform.position = new Vector3((gridSize.x + 1f) / 2f, 0f, 0f);
		walls[2].transform.position *= -1f;
		// Set the size of the camera
		Camera.main.orthographicSize = 0.5f * gridSize.y + 1f;

		// Setup the head of the snake
		tailObjects.Add(Instantiate(tail, startingPos, Quaternion.identity));

		for (int i = 0; i < startingLength - 1; i++)
		{
			LengthenTail();
		}
		StartCoroutine(NextTick());
	}

	// Update is called once per frame
	void Update()
	{
		if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && direction != Vector2.down)
		{
			direction = Vector2.up;
		}
		else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && direction != Vector2.right)
		{
			direction = Vector2.left;
		}
		else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && direction != Vector2.up)
		{
			direction = Vector2.down;
		}
		else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && direction != Vector2.left)
		{
			direction = Vector2.right;
		}
	}

	IEnumerator NextTick()
	{
		// Calculate the next position of the snake head
		Vector2 nextHeadPos = new Vector3(tailObjects[0].transform.position.x + direction.x,
										  tailObjects[0].transform.position.y + direction.y);

		// Check if the snake is about to hit an apple
		if (tailObjects[0].transform.position == apple.transform.position)
		{
			EatApple();
		}

		// Check if the snake is about to head out of bounds
		else if (nextHeadPos.x >= (gridSize.x + 1f) / 2f || nextHeadPos.x <= -(gridSize.x + 1f) / 2f 
		 || nextHeadPos.y >= (gridSize.y + 1f) / 2f || nextHeadPos.y <= -(gridSize.y + 1f) / 2f)
		{
			print("Out of bounds");
			yield break;
        }

		ApplyMovement(nextHeadPos);

		yield return new WaitForSeconds(tickRate);
		StartCoroutine(NextTick());
	}

	void ApplyMovement(Vector2 nextHeadPos)
    {
		// Set the position of the end tail to the position of the head
		tailObjects[tailObjects.Count - 1].transform.position = nextHeadPos;
		// Move the index of the end tail to be 0
		tailObjects.Insert(0, tailObjects[tailObjects.Count - 1]);
		tailObjects.RemoveAt(tailObjects.Count - 1);
	}

	void LengthenTail()
    {
		Vector2 newTailPos;

		// If there is just a head
		if(tailObjects.Count == 1)
        {
			newTailPos = (Vector2)tailObjects[0].transform.position - direction;
        }
		else
        {
			Vector3 posDifference;

			// Subtract the position of the second last tail object from the location of the last tail object
			posDifference = tailObjects[tailObjects.Count - 1].transform.position -
							tailObjects[tailObjects.Count - 2].transform.position;
			
			// Use the posDifference to calculate the position of the new tail
			newTailPos = tailObjects[tailObjects.Count - 1].transform.position + posDifference;
		}

		GameObject newTail = Instantiate(tail, newTailPos, Quaternion.identity);
		tailObjects.Add(newTail);
	}

    void EatApple()
    {
		// Lengthen the snake
		LengthenTail();
		// generate a randoPos for the apple
		Vector2 randomPos = new Vector2(Random.Range((int)-gridSize.x/2, (int)gridSize.x/2 + 1), 
										Random.Range((int)-gridSize.y/2, (int)gridSize.y/2 + 1));
		// set apple to randomPos
		apple.transform.position = randomPos;
    }
}
