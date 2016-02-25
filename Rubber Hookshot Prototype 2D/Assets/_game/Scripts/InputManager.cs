using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	public GameObject player;
    //public GameObject player2;
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

			if ( hit.collider && hit.collider.tag == "Hook" && hit.collider.gameObject != activeHook && hit.collider.gameObject.GetComponentInChildren<Renderer>().isVisible )
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

            /*if (player2.GetComponent<Player>() != null)
                player2.GetComponent<Player>().UnHook();
            else
                player2.GetComponent<Player2D>().UnHook();*/

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
