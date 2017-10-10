using CnControls;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	// Player
	public GameObject player;
	public GameObject cannon;
	Player2D playerScript;
    GameObject activeAnchor;

	// Camera
	Camera cam;
	bool cameraOff = false; // turn camera follow off on player death.

	// Score System
	public LevelGeneratorNew levelGenScr;
	public Text scoreText;
	int score = 0;
	int scoreIndex = 0;
	int scoreMultiplier = 1;
	float[] scoreCheckpoints;
	// Debug (For testing only)

	void Start ()
	{
		cam = Camera.main;
		playerScript = player.GetComponent<Player2D>();
		RequestScoreCheckpoints ();
	}

	IEnumerator Test( Material mat, Color newColor )
	{
		Color orgColor = mat.GetColor("_EmissionColor");
		mat.SetColor("_EmissionColor", newColor);
		yield return new WaitForSeconds(0.5f);
		mat.SetColor("_EmissionColor", orgColor);
	}

	void Update ()
	{
		if (!playerScript.dead) 
		{
			UpdateInput ();
			UpdateScore ( player.transform.position );
		}
		else
		{
			if ( !cameraOff )
			{
				cam.GetComponent<CameraFollow> ().enabled = false;
				cameraOff = true;
			}
		}

	}

	void UpdateInput ()
	{
		if ( Input.GetKey( KeyCode.LeftArrow ) && !Input.GetKey( KeyCode.RightArrow ) )
			RotateBallLeftEditor();
		else if ( Input.GetKey( KeyCode.RightArrow ) )
			RotateBallRightEditor();

		if ( Input.GetKeyDown( KeyCode.Space ) )
			RaycastForAnchor();
		
		if ( Input.GetKeyUp( KeyCode.Space ) )
			UnAnchor();

		if (CnInputManager.GetAxisRaw("Horizontal") != 0f || CnInputManager.GetAxisRaw("Vertical") != 0f)
            RotateBall( CnInputManager.GetAxisRaw("Horizontal"), CnInputManager.GetAxisRaw("Vertical") );

		if (CnInputManager.GetButtonDown("Fire3"))
			Application.LoadLevel("LevelGeneratiorTest");
	}

	void RotateBall( float x, float y )
	{
		player.transform.eulerAngles = new Vector3( player.transform.eulerAngles.x, player.transform.eulerAngles.y, Mathf.Atan2(y, x) * Mathf.Rad2Deg);
	}

	void RotateBallLeftEditor()
	{
		player.transform.Rotate( Vector3.forward* Time.deltaTime * 500f );
	}
	void RotateBallRightEditor()
	{
		player.transform.Rotate( Vector3.back* Time.deltaTime * 500f );
	}

	public void RaycastForAnchor()
	{
		Vector2 origin = cannon.transform.position;
		RaycastHit2D[] hits = Physics2D.RaycastAll( origin, cannon.transform.right, 1000f );
		Debug.DrawRay( origin, cannon.transform.right* 100f, Color.red, 5f, false);

		if (hits != null)
		{
			for (int i = 0; i<hits.Length; i++)
			{
				if ( hits[i].collider != null )
				{
					if (hits[i].collider.tag == "Anchor")
					{
						if ( ColliderIsInCameraView( hits[i].collider ) ) // check if it's in camera view
						{
							Anchor( hits[i].transform.position);
					activeAnchor = hits[i].collider.gameObject;
							hits[i].collider.enabled = false;
						}
						else
						{
							StartCoroutine( Test(player.transform.GetChild(0).GetComponent<Renderer>().material, Color.red));
						}
						return;
					}
					else if (hits[i].collider.tag == "Wall")
					{
						float dif = Mathf.Abs( player.transform.position.x - hits[i].point.x );
						if (dif< 80f)
						{
							Anchor( hits[i].point);
							activeAnchor = null;
						}
						else
						{
							StartCoroutine( Test(player.transform.GetChild(0).GetComponent<Renderer>().material, Color.blue));
						}
						return;
					}
				}
			}
		}
	}

	void Anchor (Vector3 anchorPosition)
	{
		ResetAnchor();
		playerScript.anchored = true;
		playerScript.anchorPos = anchorPosition;
	}

	public void UnAnchor()
	{
		ResetAnchor();
		if ( playerScript.anchored )
		{
			playerScript.anchored = false;
		}
	}

	void ResetAnchor()
	{
		if (activeAnchor)
			activeAnchor.GetComponent<Collider2D>().enabled = true;
	}

	void UpdateScore ( Vector2 playerPos )
	{
		if ( scoreIndex < scoreCheckpoints.Length )
		{
			if ( playerPos.x > scoreCheckpoints[scoreIndex] ) 
			{
				scoreIndex++;
				score += 1 * scoreMultiplier;
				scoreText.text = score.ToString();
			}
		}
		else
			RequestScoreCheckpoints ();

			
	}

	void RequestScoreCheckpoints ()
	{
		scoreCheckpoints = levelGenScr.GetColVertsX ();
	}

	bool ColliderIsInCameraView( Collider2D col )
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
		return GeometryUtility.TestPlanesAABB( planes, col.bounds );
	}
}
