using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {


	public Transform target;
	Vector3 playerVelocity; //GameManager?
	Vector3 followY;
	Vector3 followXY;

	Vector3 vel;
	Vector3 follow;


	//Vector3 follow;

	void Update ()
	{
		playerVelocity = target.gameObject.GetComponent<Rigidbody>().velocity;
		vel = playerVelocity * 0.25f * Time.deltaTime;
	
	//	if (target.GetComponent<Player>().grounded)
	//		vel = Vector3.Lerp(vel, new Vector3 (0f, 20f, 0f), 25f * Time.deltaTime);
	//	else
	//		vel = Vector3.Lerp(vel, new Vector3(0f, 20f, 0f), 25f * Time.deltaTime);

		follow = target.position + vel + new Vector3( 6f, 0f, transform.position.z );

	}

	void FixedUpdate () 
	{
		followY = new Vector3( transform.position.x, target.position.y, transform.position.z );
		followXY = new Vector3( target.position.x, target.position.y, transform.position.z );
		
		// only follow on Y axis
		//transform.position = Vector3.Lerp( transform.position, followY, 5f );

		// follow on both axises
		//transform.position = Vector3.Lerp( transform.position, followXY, 5f );

		// Follow on both axises and take into account player velocity 
		transform.position = Vector3.Lerp( transform.position, follow, 0.2f );
		
	}
}
