using UnityEngine;

public class Player2D : MonoBehaviour {

	public static Player2D player;

	public Transform outline;
	public bool 		isAnchored;
	public bool 		isDead;
	public GameObject 	activeAnchor;

	Rigidbody2D rb;
	LineRenderer lineRend;
	Vector3 anchorPos = Vector3.zero;
	Vector3 direction = Vector3.zero;
	bool ropeIsReset;

	// Shield
	const float chargeDelay = 0.75f;
	const float chargeTime = 0.85f;
	float chargeTimer;
	float delayTimer;
	int shieldCharges = 3;
	bool waitingToCharge;
	bool charging;
	bool charged = true;

	// Movement
	const float 	maxThrust = 8f;
	const float 	maxAcc = 8f;
	const float		maxVel = 40f;
	float 			thrust;
	float			acceleration;
	float			curVelX;
	float			curVelY;


	void Awake()
	{
		player = this;
	}

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		lineRend = GetComponent<LineRenderer>();
	}

	void Update ()
	{
		UpdateVelocity();

		Move();
	
		RechargeHits();
	}

	void UpdateVelocity()
	{
		curVelX = Mathf.Abs( rb.velocity.x );
		curVelY = Mathf.Abs( rb.velocity.y );
	}
		
	void Move()
	{
		if ( isAnchored && !isDead )
		{
			DrawRope();
			direction = anchorPos - transform.position;
			acceleration = Mathf.Clamp( direction.magnitude, 0f, maxAcc );

			if ( thrust < maxThrust )
				thrust += acceleration * Time.deltaTime;
			
			rb.AddForce( direction * thrust * Time.deltaTime * 30f, ForceMode2D.Force );

			if ( curVelX > maxVel || curVelY > maxVel )
			{
				rb.drag += (Mathf.Max( curVelX, curVelY )) * Time.deltaTime;
				rb.drag = Mathf.Clamp( rb.drag, 0f, 1.15f );
			}
			else
			{
				rb.drag = 0f;
			}
		}
		else
		{
			rb.drag = 0f;
			if ( !ropeIsReset )
			{
				ropeIsReset = true;
				ResetRope();
			}
		}
	}

	public void Rotate( float x, float y )
	{
		outline.eulerAngles =
			new Vector3( transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Atan2( y, x ) * Mathf.Rad2Deg );
	}

	public void RotateBallLeftEditor()
	{
		outline.Rotate( Vector3.forward* Time.deltaTime * 500f );
	}
	public void RotateBallRightEditor()
	{
		outline.Rotate( Vector3.back* Time.deltaTime * 500f );
	}

	void RechargeHits()
	{
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
		else
			if ( charging )
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
		if ( col != null )
		{
			if ( col.collider.tag == "Wall" )
				UnAnchor();
			
			//Debug.Log("Collided with: " + col.transform.tag);
			//Debug.Log( "Collided and VEL WAS:" + Mathf.Abs(rb.velocity.x) + ", " + Mathf.Abs(rb.velocity.y) );
			if ((Mathf.Abs (rb.velocity.x) > 5f) || (Mathf.Abs (rb.velocity.y) > 5f)) 
			{
				ShieldHit();
				if ( col.collider.tag == "Wall" )
					UnAnchor();
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

	void ShieldHit()
	{
		shieldCharges--;

		if ( shieldCharges == 0 )
			Death();
		else 
		{
			waitingToCharge = true;
			chargeTimer = 0f;
			charging = false;
		}
	}
		
	void Death()
	{
		shieldCharges = 3;
		Application.LoadLevel("LevelGeneratiorTest");
	}

	public void Anchor( Vector3 anchorPosition )
	{
		ResetAnchor();
		isAnchored = true;
		anchorPos = anchorPosition;
	}

	public void UnAnchor()
	{
		ResetAnchor();
		if ( isAnchored )
			isAnchored = false;
	}

	void ResetAnchor()
	{
		if ( activeAnchor )
			activeAnchor.GetComponent<Collider2D>().enabled = true;
	}

	void DrawRope()
	{
		if ( ropeIsReset )
			ropeIsReset = false;
		
		lineRend.SetPosition( 0, transform.position );
		lineRend.SetPosition( 1, anchorPos );

		float currentDistance = (transform.position - anchorPos).magnitude;
		float clampedDistance = Mathf.Clamp(currentDistance, 2f, 18f);
		float alphaMagnitude = Remap(clampedDistance, 2f, 18f, 1f, 0f);

		float min = gameObject.transform.localScale.x * 0.2f;
		float max = gameObject.transform.localScale.x * 0.6f;
		float width = Mathf.Lerp( min, max, alphaMagnitude );

		lineRend.startWidth = width;
		lineRend.endWidth = width;
	}

	void ResetRope ()
	{
		lineRend.SetPosition(0, Vector3.zero);
		lineRend.SetPosition(1, Vector3.zero);
	}
		
	float Remap ( float value, float from1, float to1, float from2, float to2 )
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

}
