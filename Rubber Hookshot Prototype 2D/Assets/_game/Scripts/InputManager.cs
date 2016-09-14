using CnControls;
using UnityEngine;
using System.Collections.Generic;

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
	bool onMobile = false;

	void Start ()
	{
		if (Application.platform == RuntimePlatform.Android)
			onMobile = true;
	}

	void Update()
	{
		if ( Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) )	
			player.transform.Rotate( Vector3.forward * Time.deltaTime * 500f );
		else if ( Input.GetKey(KeyCode.RightArrow) )
			player.transform.Rotate( Vector3.back * Time.deltaTime * 500f );

		if ( Input.GetKeyDown(KeyCode.X) || CnInputManager.GetButtonDown("Jump") )
		{
			Vector2 origin = cannon.transform.position;
			RaycastHit2D hit = Physics2D.Raycast(origin, cannon.transform.right, 100f);

			//Debug.DrawRay( origin, cannon.transform.right * 100f, Color.red, 100f, false);
			if (hit)
			{
				if (hit.collider.tag == "Anchor")
				{
					player.GetComponent<Player2D>().Anchor(hit.transform.position);
					activeAnchor = hit.collider.gameObject;
				}
				else if (hit.collider.tag == "Wall")
				{
					player.GetComponent<Player2D>().Anchor(hit.point);
					activeAnchor = null;
				}
			}
			
		}

		if ( Input.GetKeyDown( KeyCode.Z ) || CnInputManager.GetButtonDown("Cancel") )
			player.GetComponent<Player2D>().UnAnchor();

		if (onMobile)
		{
			float y = 0f; float x = 0f;
			if (CnInputManager.GetAxisRaw("Horizontal") != 0f || CnInputManager.GetAxisRaw("Vertical") != 0f)
			{
				x = CnInputManager.GetAxisRaw("Horizontal");
				y = CnInputManager.GetAxisRaw("Vertical");
				player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, player.transform.eulerAngles.y, Mathf.Atan2(y, x) * Mathf.Rad2Deg);
			}
		}
		

		if (CnInputManager.GetButtonDown("Fire3"))
			Application.LoadLevel("LevelGeneratiorTest");
	}

}
