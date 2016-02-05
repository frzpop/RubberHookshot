using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	public GameObject player;
	GameObject activeHook;

	public float restartTimer = 0.5f;
	
	void Start () 
	{
		
	}
	
	void Update () 
	{
		UpdateInput();
	}

	void UpdateInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(ray, out hit);

			/*if (hit.collider)
				print("collided with " + hit.collider.tag);
			else
				print("didn't collide with anything");*/

			if ( hit.collider && hit.collider.tag == "Hook" && hit.collider.gameObject != activeHook )
			{
				// HOOK HERE
				player.GetComponent<Player>().Hook(hit.transform.position);
				activeHook = hit.collider.gameObject;
			}
		}

		if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && !Input.GetMouseButton(3))
		{
			//UNHOOK HERE
			player.GetComponent<Player>().UnHook();
			activeHook = null;

			restartTimer -= Time.deltaTime;
			if (restartTimer <= 0f)
			{
				Application.LoadLevel("Test");
			}
		}
		else if (Input.touchCount == 3)
		{
			Application.Quit();
		}


		

	}
}
