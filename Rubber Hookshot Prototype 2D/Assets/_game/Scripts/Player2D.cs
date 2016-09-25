using UnityEngine;
using System.Collections;

public class Player2D : MonoBehaviour {


	public InputManager inputManager;
	public Vector3 		anchorPos = Vector3.zero;
	public GameObject 	smallBallPrefab;
	public bool 		anchored = false;
	public bool 		dead = false;

	Rigidbody2D rb;
	LineRenderer lr;
	Vector3 direction = Vector3.zero;

	// Shield
	static int shieldCharges = 3;
	static float chargeDelay = 0.75f;
	static float chargeTime = 0.85f;
	float chargeTimer = 0f;
	float delayTimer = 0f;
	bool waitingToCharge = false;
	bool charging = false;
	bool charged = true;

	// Movement
	static float 	maxThrust = 8f;
	static float 	maxAcc = 8f;
	static float	maxVel = 40f;
	float 			thrust;
	float			acceleration;
	float			curVelX;
	float			curVelY;


	bool ropeIsReset = false;

	Color color;

	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		lr = GetComponent<LineRenderer>();
		gameObject.GetComponent<Renderer>().material.SetColor( "_EmissionColor", Color.cyan );
	}

	void Update ()
	{
		curVelX = Mathf.Abs(rb.velocity.x);
		curVelY = Mathf.Abs(rb.velocity.y);
		//Debug.Log(new Vector2(curVelX, curVelY));

		if (shieldCharges == 3)
			color = Color.green;
		else if (shieldCharges == 2)
			color = Color.yellow;
		else if (shieldCharges == 1)
			color = Color.red;

			gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);

		if ( anchored && !dead )
		{
			DrawRope();

			direction = anchorPos - transform.position;
			acceleration = Mathf.Clamp(direction.magnitude, 0f, maxAcc) ;
			if ( thrust < maxThrust )
				thrust += acceleration * Time.deltaTime;

			rb.AddForce(direction * thrust * Time.deltaTime * 30f, ForceMode2D.Force); // This might be better in fixed update w/o time.deltatime

			if (curVelX > maxVel || curVelY > maxVel)
			{
				rb.drag += ( Mathf.Max(curVelX, curVelY) ) * Time.deltaTime;
				rb.drag = Mathf.Clamp(rb.drag, 0f, 1.15f);
			}
			else
			{
				rb.drag = 0f;
			}
		}
		else
		{
			rb.drag = 0f;
			if (!ropeIsReset)
			{
				ropeIsReset = true;
				ResetRope();
			}
		}
		//Debug.Log(rb.drag);
			
		//ColorLerp( rb.velocity );

		if ( waitingToCharge )
		{
			delayTimer += Time.deltaTime;
			if ( delayTimer >= chargeDelay )
			{
				delayTimer = 0f;
				waitingToCharge = false;
				charging = true;
			}
		}
		else if ( charging )
		{
			chargeTimer += Time.deltaTime;
			if ( chargeTimer >= chargeTime )
			{
				chargeTimer = 0f;
				shieldCharges++;
				if ( shieldCharges == 3 )
					charging = false;
			}
		}
	}

	void OnCollisionEnter2D ( Collision2D col )
	{
		if (col != null)
		{
			if (col.collider.tag == "Wall")
				inputManager.UnAnchor();
			
			//Debug.Log("Collided with: " + col.transform.tag);
			//Debug.Log( "Collided and VEL WAS:" + Mathf.Abs(rb.velocity.x) + ", " + Mathf.Abs(rb.velocity.y) );
			if ((Mathf.Abs (rb.velocity.x) > 5f) || (Mathf.Abs (rb.velocity.y) > 5f)) 
			{
				ShieldHit();
				if (col.collider.tag == "Wall")
					inputManager.UnAnchor();
			}
				

		}	
	}
	void OnTriggerEnter2D ( Collider2D other )
	{
		//Debug.Log( "Triggered " + other.transform.tag );
		if ( other.tag == "Spikes" )
		{
			Death();
		}
	}

	void ShieldHit ()
	{
		shieldCharges--;
		if (shieldCharges == 0)
			Death ();
		else 
		{
			waitingToCharge = true;
			chargeTimer = 0f;
			charging = false;
		}
	}
		
	void Death()
	{
		if ( !dead )
		StartCoroutine("Deathy");
	}

	IEnumerator Deathy ()
	{
		dead = true;
		BallExplosion (25);
		gameObject.GetComponent<Renderer>().enabled = false;
		gameObject.transform.GetChild(0).GetComponent<Renderer> ().enabled = false;
		gameObject.transform.GetChild(0).GetComponent<LineRenderer> ().enabled = false;
		Time.timeScale = 1f;
		yield return new WaitForSeconds(1.5f);
		shieldCharges = 3;
		Time.timeScale = 1f;
		Application.LoadLevel("LevelGeneratiorTest");
	}

	void BallExplosion ( int amount )
	{
		GameObject spawnedBall;
		for (int i = 0; i < amount; i++) 
		{
			Vector2 force = new Vector2 ( Random.Range( 50f, 100f ), Random.Range( 50f, 100f ) );
			spawnedBall = (GameObject)Instantiate ( smallBallPrefab, gameObject.transform.position, Quaternion.identity );
			spawnedBall.GetComponent<Rigidbody2D>().AddForce ( force, ForceMode2D.Impulse );
		}
	}

	/*public void Anchor (Vector3 anchorPosition)
	{	
		anchored = true;
		anchorPos = anchorPosition;
	}

	public void UnAnchor ()
	{
		if ( anchored )
		{
			anchored = false;
			lr.SetPosition(0, Vector3.zero);
			lr.SetPosition(1, Vector3.zero);
		}
		
	}*/

	void DrawRope()
	{
		if (ropeIsReset)
		{
			ropeIsReset = false;
		}
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, anchorPos);

		float currentDistance = (transform.position - anchorPos).magnitude;
		float clampedDistance = Mathf.Clamp(currentDistance, 2f, 18f);
		float alphaMagnitude = Remap(clampedDistance, 2f, 18f, 1f, 0f);

		float min = gameObject.transform.localScale.x * 0.08f;
		float max = gameObject.transform.localScale.x * 0.35f;

		lr.SetWidth( Mathf.Lerp( min, max, alphaMagnitude), Mathf.Lerp( min, max, alphaMagnitude) );
	}

	void ResetRope ()
	{
		lr.SetPosition(0, Vector3.zero);
		lr.SetPosition(1, Vector3.zero);
	}

	void ColorLerp ( Vector3 velocity )
	{
		float xVel = Mathf.Abs( velocity.x );
		float yVel = Mathf.Abs(velocity.y);
		float vel = 0f;
		float max = 75f;

		if ( xVel >= yVel )
			vel = xVel;
		else
			vel = yVel;

		vel = Mathf.Clamp( vel, 0f, max );
		
		vel = Remap( vel, 0f, max, 0f, 1f );
		
		Color color = Color.Lerp(Color.green, Color.red, vel);
		gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
		
	}

	float Remap ( float value, float from1, float to1, float from2, float to2 )
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

}
