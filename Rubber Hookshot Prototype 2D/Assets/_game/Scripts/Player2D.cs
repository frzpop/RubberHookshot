using UnityEngine;
using System.Collections;

public class Player2D : MonoBehaviour {

	Rigidbody2D rb;
	LineRenderer lr;

	Vector3 hookPos = Vector3.zero;
	Vector3 direction = Vector3.zero;
	bool hooked = false;
	public bool grounded = false;
	bool colliding = false;

	float thrust = 0f;
	float distanceMultiplier;

	float maxThrust = 10f;
	float acceleration = 0f;

	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		lr = GetComponent<LineRenderer>();
	}

	void Update ()
	{

		if (hooked)
		{
			DrawRope();
		
			acceleration = direction.magnitude * 2f;
			if ( thrust < maxThrust )
			{
				thrust += acceleration * Time.deltaTime;
			}

			grounded = false;
			colliding = false;

		}
		else if (colliding && rb.velocity.x < 0.2f && rb.velocity.y < 0.2f)
			grounded = true;
		else
		{
			thrust = 0f;
			grounded = false;
			colliding = false;
		}

		//Debug.Log( Mathf.Abs(rb.velocity.x) + " " + Mathf.Abs(rb.velocity.y) );
	}

	void FixedUpdate () 
	{
		if (hooked)
		{
			direction = hookPos - transform.position;
	
			rb.AddForce(direction * thrust, ForceMode2D.Force);
		}
		
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		if (col != null)
		{
			Debug.Log( "Collided and VEL WAS:" + Mathf.Abs(rb.velocity.x) + ", " + Mathf.Abs(rb.velocity.y) );
			//if ( ( Mathf.Abs(rb.velocity.x) > 5f ) || ( Mathf.Abs(rb.velocity.y) > 5f) )
			//	Death();
			//else
			//colliding = true;
			Death ();
		}
			
	}

	void DrawRope()
	{
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, hookPos);

		float currentDistance = (transform.position - hookPos).magnitude;
		float clampedDistance = Mathf.Clamp(currentDistance, 2f, 18f);
		float alphaMagnitude = Remap(clampedDistance, 2f, 18f, 1f, 0f);

		lr.SetWidth(Mathf.Lerp(0.08f, 0.6f, alphaMagnitude),
					 Mathf.Lerp(0.08f, 0.6f, alphaMagnitude));
	}

	void Death()
	{
		//Destroy(gameObject);
		Application.LoadLevel("Test");

	}

	public void Hook (Vector3 hookPosition)
	{
		hooked = true;
		hookPos = hookPosition;
	}

	public void UnHook ()
	{
		hooked = false;
		lr.SetPosition(0, Vector3.zero);
		lr.SetPosition(1, Vector3.zero);
	}
 
	float Remap (float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	}

}
