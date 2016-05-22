using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraOnRails : MonoBehaviour {

	public GameObject player;
	public GameObject indicator;

	List<float>	difs = new List<float>();
	Vector2[]	points;
	float		distance;
	float		lowestValue = 999f;
	int			lowestIndex;
	Vector3		target;
	Vector3		playerVelocity;
	Vector3		vel;

	void Update ()
	{
		if ( points != null )
		CameraMovement();
	}

	void CameraMovement ()
	{
		if (player.gameObject.GetComponent<Rigidbody>() != null)
			playerVelocity = player.gameObject.GetComponent<Rigidbody>().velocity;
		else
			playerVelocity = player.gameObject.GetComponent<Rigidbody2D>().velocity;

		vel = playerVelocity * 0.50f;

		float playerX = player.transform.position.x;
		float targetX;

		if ( vel.x >= -0.9f )
			targetX = playerX + vel.x + 15f;
		else
			targetX = playerX + ( vel.x - 6f );



		for ( int i = 0; i < points.Length; i++ )
		{
			float dif = Mathf.Abs(points[i].x - targetX);
			difs.Add(dif);
		}

		for (int i = 0; i < difs.Count; i++)
		{
			if ( lowestValue > difs[i] )
			{
				lowestValue = difs[i];
				lowestIndex = i;
			}
		}
		float targetY = points[lowestIndex].y;

		target = new Vector3( targetX, targetY + ( distance / 2 ), transform.position.z );
		GameObject myInd = (GameObject)Instantiate(indicator, new Vector3 ( target.x, target.y, 0f ), Quaternion.identity  );
		StartCoroutine( DelayedDestroy(myInd) );


		transform.position = Vector3.Lerp(transform.position, target, 0.5f * Time.deltaTime);
		difs.Clear();
		lowestValue = 999f;
	}


	public void SetPoints ( List<Vector2> myPoints, float myDistance )
	{
		Vector2[] pnts = new Vector2[myPoints.Count];
		for (int i = 0; i < pnts.Length; i++)
		{
			pnts[i] = myPoints[i];
		}
		points = pnts;
		distance = myDistance;
	}

	IEnumerator DelayedDestroy ( GameObject myObj )
	{
		yield return new WaitForSeconds(0.1f);
		Destroy(myObj);
	}
}
