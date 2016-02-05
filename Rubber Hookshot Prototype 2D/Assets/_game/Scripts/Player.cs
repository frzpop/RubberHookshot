using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	Rigidbody rb;
	LineRenderer lr;

	Vector3 hookPos = Vector3.zero;
	Vector3 direction = Vector3.zero;
	bool hooked = false;
	public bool grounded = false;
	bool colliding = false;
	bool test = false;

	const float BASE_THRUST = 8f;
	float thrust;
	float distanceMultiplier;
	
	float maxThrust = 8f;
	float acceleration = 0f;
	float deaceleration = 20f;



	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		lr = GetComponent<LineRenderer>();
		thrust = BASE_THRUST;
		
	}

	void Update ()
	{

		if (hooked)
		{
			Rope();

			acceleration = direction.magnitude;
			if (thrust < maxThrust)
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
			distanceMultiplier = Mathf.Clamp( direction.magnitude, 0f, 5f );
			
			//rb.AddForce(direction.normalized * ( thrust * distanceMultiplier ), ForceMode.Acceleration);
			rb.AddForce(direction * thrust, ForceMode.Acceleration);

		}
		
	}

	void OnCollisionEnter (Collision col)
	{
		if (col != null)
		{
			Debug.Log( "Collided and VEL WAS:" + Mathf.Abs(rb.velocity.x) + ", " + Mathf.Abs(rb.velocity.y) );
			if ( ( Mathf.Abs(rb.velocity.x) > 5f ) || ( Mathf.Abs(rb.velocity.y) > 5f) )
				Death();
			else
			colliding = true;
		}
			
	}

	void Rope()
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
		Destroy(gameObject);
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
