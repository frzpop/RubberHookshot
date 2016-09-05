using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class InputManager : MonoBehaviour {

	public GameObject player;
	public GameObject cannon;
    //public GameObject player2;
    GameObject activeAnchor;

	public Material[] materials;
	public GameObject clickIndicator;
	GameObject spawnedIndicator;

	public List<float> difs;

	float restartTimer = 0.5f;
	float swithcMoveTimer = 1f;
	bool switched = false;
	bool useNewMove = true;


	void Start () 
	{
	}

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			swithcMoveTimer -= Time.deltaTime;
			if (swithcMoveTimer <= 0f && !switched)
			{
				useNewMove = !useNewMove;
				switched = true;
				if (useNewMove)
					player.GetComponent<Renderer>().material = materials[0];
				else
					player.GetComponent<Renderer>().material = materials[1];

			}

		}
		else
		{
			swithcMoveTimer = 1f;
			switched = false;
		}

		NewMove();

		if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && !Input.GetMouseButton(3))
		{
			restartTimer -= Time.deltaTime;
			if (restartTimer <= 0f)
				Application.LoadLevel("LevelGeneratiorTest");
		}
		else if (Input.touchCount == 3)
		{
			Application.Quit();
		}

		if (Input.GetMouseButtonUp(0))
		{
			restartTimer = 0.5f;
		}

		if ( Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) )	
		{
			player.transform.Rotate( Vector3.forward * Time.deltaTime * 500f );
		}
		else if ( Input.GetKey(KeyCode.RightArrow) )
		{
			player.transform.Rotate( Vector3.back * Time.deltaTime * 500f );
		}

		if ( Input.GetKeyDown(KeyCode.Space) )
		{
			Vector2 origin = cannon.transform.position;
			RaycastHit2D hit = Physics2D.Raycast(origin, cannon.transform.right, 100f);

			Debug.DrawRay( origin, cannon.transform.right * 100f, Color.red, 100f, false);

			Debug.Log(hit.collider.tag);

			if ( hit.collider.tag == "Anchor" )
			{
				player.GetComponent<Player2D>().Anchor(hit.transform.position);
				activeAnchor = hit.collider.gameObject;
			}
			else if ( hit.collider.tag == "Wall" )
			{
				player.GetComponent<Player2D>().Anchor(hit.point);
				activeAnchor = null;
			}
		}

	}
		
	void NewMove()
	{
		if ( Input.GetMouseButtonDown(0)  )
		{
			if (spawnedIndicator)
				Destroy(spawnedIndicator);

			difs.Clear();
			MyRaycast();
			/*if ( Camera.main.WorldToScreenPoint(Input.mousePosition).x < 1300f  )
			{
				if ( Camera.main.WorldToScreenPoint(Input.mousePosition ).y > 900f )
				{
					//yes
					MyRaycast();
				}
			}
			else
			{
				MyRaycast();
			}*/
			
			
		}
		else if ( Input.GetMouseButton(0) && Input.GetMouseButton(1) && !Input.GetMouseButton(3) )
		{
			restartTimer -= Time.deltaTime;
			if (restartTimer <= 0f)
				Application.LoadLevel("LevelGeneratiorTest");
		}
		else if (Input.touchCount == 3)
		{
			Application.Quit();
		}

		if ( Input.GetMouseButtonUp(0) )
		{
			restartTimer = 0.5f;
		}
	}

	void MyRaycast()
	{
		//Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.zero);
		bool hitUI = false;

		/*Vector2 origin = new Vector2( Camera.main.sc (Input.mousePosition) );
		RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down);*/

		spawnedIndicator = (GameObject)Instantiate(clickIndicator, origin, Quaternion.identity);

		int index = 0;
		float shortest = 999f;

		for (int i = 0; i < hits.Length; i++)
		{
			//Debug.Log(hits[i].transform.gameObject.tag);
			if (hits[i].transform.gameObject.tag == "UI")
				hitUI = true;
				break;
		}
		for (int i = 0; i < hits.Length; i++)
		{

			if (hits[i].collider && hits[i].collider.tag == "Anchor" &&
				hits[i].collider.gameObject != activeAnchor &&
				hits[i].collider.gameObject.GetComponentInChildren<Renderer>().isVisible)
			{
				float dif = Mathf.Abs(Vector2.Distance(origin, hits[i].collider.transform.position));
				difs.Add(dif);

				if (hits[i].collider)
					print("collided with " + hits[i].collider.tag);
				else
					print("didn't collide with anything");

			}
		}

		for (int i = 0; i < difs.Count; i++)
		{
			if (shortest > difs[i])
			{
				shortest = difs[i];
				index = i;
			}
		}

		// HOOK HERE
		if ( hits.Length != 0 && !hitUI )
		{
			player.GetComponent<Player2D>().Anchor(hits[index].transform.position);
			activeAnchor = hits[index].collider.gameObject;

		}
	}

}
