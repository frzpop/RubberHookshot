using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {


	public Transform target;
	public GameObject indicator;
	public bool old;
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

	float offset; // This is the difference in Y between ground and roof.


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

        vel = playerVelocity * 0.08f;
	
		velX = Mathf.Abs(playerVelocity.x);
		velY = Mathf.Abs(playerVelocity.y);
		highestVel = Mathf.Max(velX, velY);
	

		timer += Time.deltaTime;

		if (old)
		{
			follow = target.position + vel + new Vector3(12f, 0f, transform.position.z);
		}
		else
		{
			Vector2 origin = new Vector2(target.position.x + 10f, target.position.y);
			RaycastHit2D[] hit = Physics2D.RaycastAll(origin, Vector2.down);

			float center = 0f;
			for (int i = 0; i < hit.Length; i++)
			{
				if ((hit[i].collider != null) && hit[i].collider.tag == "Wall")
				{
					//Debug.Log("collided with: " + hit[i].collider.tag);
					center = hit[i].point.y + offset;
					follow = target.position + vel + new Vector3(12f, 0f, transform.position.z);
					follow = new Vector3(follow.x, Mathf.Clamp(follow.y, center - 10f, center + 10f), follow.z);
				}
				
			}
		}
		/*GameObject myInd = (GameObject)Instantiate(indicator, new Vector3(follow.x, follow.y, 0f), Quaternion.identity);
		StartCoroutine(DelayedDestroy(myInd));*/

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
	public void SetOffset ( float myOffset )
	{
		offset = myOffset / 2;
	}
}
