using UnityEngine;

public class OsawariConditions
{
	public float TimeCount;

	public float Speed;

	public float MovedDistance;

	public bool IsPistonMoving;

	public Vector3 Move;

	public bool IsKissing;

	public static OsawariConditions Empty => new OsawariConditions(0f, 0f, 0f, isPistonMoving: false, Vector3.zero);

	public OsawariConditions(float time, float speed, float movedDistance, bool isPistonMoving, Vector3 move)
	{
		TimeCount = time;
		Speed = speed;
		MovedDistance = movedDistance;
		IsPistonMoving = isPistonMoving;
		Move = move;
		IsKissing = false;
	}

	public void UpdateTime()
	{
		TimeCount += Time.deltaTime;
	}

	public void ResetTime()
	{
		TimeCount = 0f;
	}

	public override string ToString()
	{
		return $"Speed is {Speed}, MovedDistance is {MovedDistance}, IsPistonMoving is {IsPistonMoving}, TimeCount is {TimeCount}, move is {Move}";
	}
}
