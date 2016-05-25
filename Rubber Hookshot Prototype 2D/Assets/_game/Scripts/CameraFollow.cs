using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {


	public Transform target;
	public GameObject indicator;
	Vector3 playerVelocity; // To GameManager?

	Vector3 vel;
	Vector3 follow;
	float velX;
	float velY;
	float highestVel;

	Camera cam;
	float cameraSizeMin;
	float cameraSizeMax;
	float cameraZoomAmount = 9f;

	float timer = 0f;
	bool zoomIn = false;


	//Vector3 follow;

	void Start ()
	{
		cam = gameObject.GetComponent<Camera>();
		cameraSizeMin = cam.orthographicSize;
		cameraSizeMax = cameraSizeMin + cameraZoomAmount;
	}

	void Update ()
	{
        if (target.gameObject.GetComponent<Rigidbody>() != null)
            playerVelocity = target.gameObject.GetComponent<Rigidbody>().velocity;
        else
            playerVelocity = target.gameObject.GetComponent<Rigidbody2D>().velocity;

        vel = playerVelocity * 0.25f * Time.deltaTime;

		velX = Mathf.Abs(playerVelocity.x);
		velY = Mathf.Abs(playerVelocity.y);
		highestVel = Mathf.Max(velX, velY);

		//	if (target.GetComponent<Player>().grounded)
		//		vel = Vector3.Lerp(vel, new Vector3 (0f, 20f, 0f), 25f * Time.deltaTime);
		//	else
		//		vel = Vector3.Lerp(vel, new Vector3(0f, 20f, 0f), 25f * Time.deltaTime);

		follow = target.position + vel + new Vector3( 6f, 0f, transform.position.z );
		//follow = new Vector3( follow.x, Mathf.Clamp( follow.y, center - 10f, center + 10f ), follow.z );

		GameObject myInd = (GameObject)Instantiate(indicator, new Vector3(follow.x, follow.y, 0f), Quaternion.identity);
		StartCoroutine(DelayedDestroy(myInd));

		timer += Time.deltaTime;
	}

	void FixedUpdate () 
	{
		// Follow on both axises and take into account player velocity 
		transform.position = Vector3.Lerp( transform.position, follow, 0.2f );

		//ZoomCheck();
	}

	void ZoomCheck ()
	{

		float zoomInCd = 0.3f;
		if ( highestVel > 20f )
		{
			zoomIn = false;
		}
		else if ( timer > zoomInCd )
		{
			zoomIn = true;
			timer = 0f;
		}

		Zoom(zoomIn);
	}

	void Zoom (bool inOrOut)
	{
		if ( !inOrOut )
		{
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cameraSizeMax, 0.01f);
		}
		else
		{
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cameraSizeMin, 0.01f);
		}
	}

	IEnumerator DelayedDestroy(GameObject myObj)
	{
		yield return new WaitForSeconds(0.1f);
		Destroy(myObj);
	}
}
