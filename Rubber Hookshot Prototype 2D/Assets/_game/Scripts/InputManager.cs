using CnControls;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

	public static InputManager input;


	// Camera
	Camera cam;
	bool cameraOff;

	void Awake()
	{
		input = this;
	}

	void Start ()
	{
		cam = Camera.main;

	}
		
	void Update ()
	{
		if ( !Player2D.player.isDead ) 
		{
			UpdateInput ();
			//ScoreSystem.ss.UpdateScore();
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
			Player2D.player.RotateBallLeftEditor();
		else if ( Input.GetKey( KeyCode.RightArrow ) )
			Player2D.player.RotateBallRightEditor();

		if ( Input.GetKeyDown( KeyCode.Space ) )
			RaycastForAnchor();
		
		if ( Input.GetKeyUp( KeyCode.Space ) )
			Player2D.player.UnAnchor();

		if (CnInputManager.GetAxisRaw("Horizontal") != 0f || CnInputManager.GetAxisRaw("Vertical") != 0f)
			Player2D.player.Rotate( CnInputManager.GetAxisRaw("Horizontal"), CnInputManager.GetAxisRaw("Vertical") );

		if (CnInputManager.GetButtonDown("Fire3"))
			Application.LoadLevel("LevelGeneratiorTest");
	}
		
	public void RaycastForAnchor()
	{
		Player2D player = Player2D.player;
		Vector2 origin = player.transform.position;
		RaycastHit2D[] hits = Physics2D.RaycastAll( origin, player.outline.right, 1000f );
		Debug.DrawRay( origin, player.outline.right * 100f, Color.red, 5f, false);

		if ( hits == null )
			return;


		for (int i = 0; i < hits.Length; i++) 
		{
			if ( hits[i].collider.tag != "Anchor" && hits[i].collider.tag != "Wall" )
				continue;

			if ( hits[i].collider.tag == "Anchor" && ColliderIsInCameraView( hits[i].collider ) )
			{
				Player2D.player.Anchor( hits[i].transform.position );
				Player2D.player.activeAnchor = hits[i].collider.gameObject;
				hits[i].collider.enabled = false;
				return;
			}

			if ( hits[i].collider.tag == "Wall" )
			{
				float dif = Mathf.Abs( Player2D.player.transform.position.x - hits[i].point.x );
				if ( dif < 80f)
				{
					Player2D.player.Anchor( hits[i].point);
					Player2D.player.activeAnchor = null;
					return;
				}
			}
		}
	}
		
	bool ColliderIsInCameraView( Collider2D col )
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
		return GeometryUtility.TestPlanesAABB( planes, col.bounds );
	}
}
