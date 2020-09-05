using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum DoorState
{
	OPEN,
	CLOSED,
	MOVING
}
[RequireComponent(typeof(NavMeshObstacle))]
public class SlidingDoor : MonoBehaviour
{
	[SerializeField]
	private float doorSpeed;
	[SerializeField]
	private DoorState doorState;
	[SerializeField]
	private Vector3 openPosition;
	[SerializeField]
	private Vector3 closePosition;
	[SerializeField]
	private bool carve;

	private Transform cachedTransform;
	private NavMeshObstacle obstacle;

	void Start()
    {
		cachedTransform = transform;
		obstacle = GetComponent<NavMeshObstacle>();
	}

    void Update()
    {
		if(Input.GetKeyUp(KeyCode.Space))
		{
			if(doorState == DoorState.OPEN)
			{
				StartCoroutine(MoveDoor(closePosition));
			}
			else if(doorState == DoorState.CLOSED)
			{
				StartCoroutine(MoveDoor(openPosition));
			}
		}
    }

	IEnumerator MoveDoor(Vector3 moveToPosition)
	{

		while (Vector3.Distance(cachedTransform.position, moveToPosition) > 0f)
		{
			cachedTransform.position = Vector3.MoveTowards(cachedTransform.position, moveToPosition, Time.deltaTime * doorSpeed);
			yield return null;
		}

		if(doorState == DoorState.OPEN)
		{
			doorState = DoorState.CLOSED;
		}
		else if (doorState == DoorState.CLOSED)
		{
			doorState = DoorState.OPEN;
		}
	}
}
