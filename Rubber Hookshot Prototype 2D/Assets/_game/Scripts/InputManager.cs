using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	public GameObject player;
    //public GameObject player2;
    GameObject activeHook;

	public Material[] materials;
	public GameObject clickIndicator;
	GameObject spawnedIndicator;

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
		if (Input.GetMouseButtonDown(0) && activeHook == null)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(ray, out hit);

			// Click indicator
		//	if ( spawnedIndicator )
		//		Destroy( spawnedIndicator );

		//	spawnedIndicator = (GameObject)Instantiate(clickIndicator, ray.GetPoint(1f), Quaternion.identity );

			/*if (hit.collider)
				print("collided with " + hit.collider.tag);
			else
				print("didn't collide with anything");*/

			if (hit.collider && hit.collider.tag == "Hook" && hit.collider.gameObject != activeHook && hit.collider.gameObject.GetComponentInChildren<Renderer>().isVisible)
			{
				// HOOK HERE
				if (player.GetComponent<Player>() != null)
					player.GetComponent<Player>().Hook(hit.transform.position);
				else
					player.GetComponent<Player2D>().Hook(hit.transform.position);

				/*if (player2.GetComponent<Player>() != null)
                    player2.GetComponent<Player>().Hook(hit.transform.position);
                else
                    player2.GetComponent<Player2D>().Hook(hit.transform.position);*/

				activeHook = hit.collider.gameObject;
			}
		}
		else if ( Input.GetMouseButtonDown(0) && activeHook != null )
		{
			//UNHOOK HERE
			if (player.GetComponent<Player>() != null)
				player.GetComponent<Player>().UnHook();
			else
				player.GetComponent<Player2D>().UnHook();

			activeHook = null;

			
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
			if (spawnedIndicator)
				Destroy(spawnedIndicator);

			/*if (hit.collider)
				print("collided with " + hit.collider.tag);
			else
				print("didn't collide with anything");*/

			if (hit.collider && hit.collider.tag == "Hook" && hit.collider.gameObject != activeHook && hit.collider.gameObject.GetComponentInChildren<Renderer>().isVisible)
			{
				// HOOK HERE
				if (player.GetComponent<Player>() != null)
					player.GetComponent<Player>().Hook(hit.transform.position);
				else
					player.GetComponent<Player2D>().Hook(hit.transform.position);

				/*if (player2.GetComponent<Player>() != null)
                    player2.GetComponent<Player>().Hook(hit.transform.position);
                else
                    player2.GetComponent<Player2D>().Hook(hit.transform.position);*/

				activeHook = hit.collider.gameObject;
			}
		}

		if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && !Input.GetMouseButton(3))
		{
			//UNHOOK HERE
			if (player.GetComponent<Player>() != null)
				player.GetComponent<Player>().UnHook();
			else
				player.GetComponent<Player2D>().UnHook();

			activeHook = null;

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
