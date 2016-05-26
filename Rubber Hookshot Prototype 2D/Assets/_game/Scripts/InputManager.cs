using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class InputManager : MonoBehaviour {

	public GameObject player;
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
	
	void Update () 
	{
		if ( Input.GetMouseButton(0) )
		{
			swithcMoveTimer -= Time.deltaTime;
			if ( swithcMoveTimer <= 0f && !switched )
			{
				useNewMove = !useNewMove;
				switched = true;
				if ( useNewMove )
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
			


		if ( useNewMove )
			NewMove();
		else
			OldMove();
	}

	void NewMove()
	{
		if (Input.GetMouseButtonDown(0) && activeAnchor == null)
		{
			if (spawnedIndicator)
				Destroy(spawnedIndicator);

			difs.Clear();

			Vector2 origin = Camera.main.ScreenToWorldPoint( Input.mousePosition );
			RaycastHit2D[] hits = Physics2D.RaycastAll( origin, Vector2.zero );

			/*Vector2 origin = new Vector2( Camera.main.sc (Input.mousePosition) );
			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down);*/

			spawnedIndicator = (GameObject)Instantiate(clickIndicator, origin, Quaternion.identity);

			int index = 0;
			float shortest= 999f;
			for (int i = 0; i < hits.Length; i++)
			{
				if (hits[i].collider && hits[i].collider.tag == "Anchor" && hits[i].collider.gameObject != activeAnchor && hits[i].collider.gameObject.GetComponentInChildren<Renderer>().isVisible)
				{
					float dif =  Mathf.Abs( Vector2.Distance(origin, hits[i].collider.transform.position) );
					difs.Add(dif);

					if ( hits[i].collider )
						print("collided with " + hits[i].collider.tag);
					else
						print("didn't collide with anything");

				}
			}

			for (int i = 0; i < difs.Count; i++)
			{
				if ( shortest > difs[i])
				{
					shortest = difs[i];
					index = i;
				}
			}

			
			// HOOK HERE
			player.GetComponent<Player2D>().Anchor( hits[index].transform.position );
			activeAnchor = hits[index].collider.gameObject;
		}
		else if ( Input.GetMouseButtonDown(0) && activeAnchor != null )
		{
			//UNHOOK HERE
			player.GetComponent<Player2D>().UnAnchor();
			activeAnchor = null;

			
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

	void OldMove()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(ray, out hit);

			// Click indicator
			//if (spawnedIndicator)
				//Destroy(spawnedIndicator);

			/*if (hit.collider)
				print("collided with " + hit.collider.tag);
			else
				print("didn't collide with anything");*/

			if (hit.collider && hit.collider.tag == "Anchor" && hit.collider.gameObject != activeAnchor && hit.collider.gameObject.GetComponentInChildren<Renderer>().isVisible)
			{
				// HOOK HERE
				if (player.GetComponent<Player>() != null)
					player.GetComponent<Player>().Hook(hit.transform.position);
				else
					player.GetComponent<Player2D>().Anchor(hit.transform.position);

				/*if (player2.GetComponent<Player>() != null)
                    player2.GetComponent<Player>().Hook(hit.transform.position);
                else
                    player2.GetComponent<Player2D>().Hook(hit.transform.position);*/

				activeAnchor = hit.collider.gameObject;
			}
		}

		if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && !Input.GetMouseButton(3))
		{
			//UNHOOK HERE
			if (player.GetComponent<Player>() != null)
				player.GetComponent<Player>().UnHook();
			else
				player.GetComponent<Player2D>().UnAnchor();

			activeAnchor = null;

			restartTimer -= Time.deltaTime;
			if (restartTimer <= 0f)
			{
				Application.LoadLevel("LevelGeneratiorTest");

			}
				
		}
		else if (Input.touchCount == 3)
		{
			Application.Quit();
		}

		if (Input.GetMouseButtonUp(0))
		{
			restartTimer = 0.5f;
		}
	}
}
