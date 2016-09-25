using CnControls;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class InputManager : MonoBehaviour {

	public GameObject player;
	Player2D playerScript;
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
	bool cameraOff = false;

	Camera cam;
	Plane[] planes;

	void Start ()
	{
		if (Application.platform == RuntimePlatform.Android)
			onMobile = true;
		cam = Camera.main;
		playerScript = player.GetComponent<Player2D>();
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
		UpdateInput ();
	}

	void UpdateInput ()
	{
		if ( !playerScript.dead )
		{
			if ( Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) )	
				player.transform.Rotate( Vector3.forward * Time.deltaTime * 500f );
			else if ( Input.GetKey(KeyCode.RightArrow) )
				player.transform.Rotate( Vector3.back * Time.deltaTime * 500f );

			if ( Input.GetKeyDown(KeyCode.X) || CnInputManager.GetButtonDown("Jump") )
			{
				Vector2 origin = cannon.transform.position;
				RaycastHit2D[] hits = Physics2D.RaycastAll( origin, cannon.transform.right, 1000f );
				Debug.DrawRay( origin, cannon.transform.right * 100f, Color.red, 5f, false);

				if (hits != null)
				{
					for (int i = 0; i < hits.Length; i++)
					{
						if ( hits[i].collider != null )
						{
							if (hits[i].collider.tag == "Anchor")
							{
								planes = GeometryUtility.CalculateFrustumPlanes(cam);
								if (GeometryUtility.TestPlanesAABB(planes, hits[i].collider.bounds)) // check if it's in camera view
								{
									Anchor(hits[i].transform.position);
									activeAnchor = hits[i].collider.gameObject;
									hits[i].collider.enabled = false;
								}
								else
								{
									StartCoroutine(Test(player.transform.GetChild(0).GetComponent<Renderer>().material, Color.red));
								}
								return;
							}
							else if (hits[i].collider.tag == "Wall")
							{
								float dif = Mathf.Abs(player.transform.position.x - hits[i].point.x);
								if (dif < 80f)
								{
									Anchor(hits[i].point);
									activeAnchor = null;
								}
								else
								{
									StartCoroutine(Test(player.transform.GetChild(0).GetComponent<Renderer>().material, Color.blue));
								}
								return;
							}
						}
					}
				}
			}

			if ( Input.GetKeyDown( KeyCode.Z ) || CnInputManager.GetButtonDown("Cancel") )
			{
				ResetAnchor();
				UnAnchor();
			}

			float y = 0f; float x = 0f;
			if (CnInputManager.GetAxisRaw("Horizontal") != 0f || CnInputManager.GetAxisRaw("Vertical") != 0f)
			{
				x = CnInputManager.GetAxisRaw("Horizontal");
				y = CnInputManager.GetAxisRaw("Vertical");
				player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, player.transform.eulerAngles.y, Mathf.Atan2(y, x) * Mathf.Rad2Deg);
			}

			if (CnInputManager.GetButtonDown("Fire3"))
				Application.LoadLevel("LevelGeneratiorTest");
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
	void ResetAnchor ()
	{
		if (activeAnchor)
			activeAnchor.GetComponent<Collider2D>().enabled = true;
	}

}
