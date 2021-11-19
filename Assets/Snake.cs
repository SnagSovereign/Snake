using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour {

	[Header("Prefabs")]
	[SerializeField] GameObject apple;
	[SerializeField] GameObject tail;

	[SerializeField] float tickRate = 0.1f;

	BoxCollider2D boxCollider;

	List<GameObject> tailObjects = new List<GameObject>();

	Vector2 direction = Vector2.right;
	bool gameOver = false;

	// Use this for initialization
	void Start () 
	{
		boxCollider = GetComponent<BoxCollider2D>();
        for (int i = 0; i < 0; i++)
        {
			LengthenTail();
        }
		StartCoroutine(ApplyMovement());
	}
	
	// Update is called once per frame
	void Update () 
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

	IEnumerator ApplyMovement()
    {
		Vector2 previousPos = transform.position;

		// Move the snake head in a direction
		transform.position = new Vector3(
					 transform.position.x + direction.x,
					 transform.position.y + direction.y);

        for (int index = 0; index < tailObjects.Count; index++)
        {
			Vector2 currentPos = tailObjects[index].transform.position;
			tailObjects[index].transform.position = previousPos;
			previousPos = currentPos;
        }

		yield return new WaitForSeconds(tickRate);
		StartCoroutine(ApplyMovement());
	}

	void LengthenTail()
    {
		Vector2 newTailPos;

		if(tailObjects.Count == 0)
        {
			newTailPos = (Vector2)transform.position - direction;
        }
		else
        {
			Vector3 posDifference;

			if(tailObjects.Count == 1)
            {
				// Subtract the position of the head from the location of the tail
				posDifference = tailObjects[tailObjects.Count - 1].transform.position -
								transform.position;
            }
			else
            {
				// Subtract the position of the second last tail object from the location of the last tail object
				posDifference = tailObjects[tailObjects.Count - 1].transform.position -
										tailObjects[tailObjects.Count - 2].transform.position;
			}
			// Use the posDifference to calculate the position of the new tail
			newTailPos = tailObjects[tailObjects.Count - 1].transform.position + posDifference;
		}

		GameObject newTail = Instantiate(tail, newTailPos, Quaternion.identity);
		tailObjects.Add(newTail);
	}

	void OnTriggerEnter2D(Collider2D col)
    {
		if(boxCollider.IsTouchingLayers(LayerMask.GetMask("Apple")))
        {
			LengthenTail();
			ResetApplePos();
        }
    }

	void ResetApplePos()
    {
		Vector2 randomPos = new Vector2(Random.Range(-8, 9), Random.Range(-8, 9));
		apple.transform.position = randomPos;
    }
}
