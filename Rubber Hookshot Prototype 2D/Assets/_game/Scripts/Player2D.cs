using UnityEngine;
using System.Collections;

public class Player2D : MonoBehaviour {

	Rigidbody2D rb;
	LineRenderer lr;
	public InputManager inputManager;
	public Vector3 anchorPos = Vector3.zero;
	Vector3 direction = Vector3.zero;
	public bool anchored = false;

	float thrust;
	float maxThrust = 10f;
	float maxAcc = 25f;
	float acceleration;

	float curVelX;
	float curVelY;
	float maxVel = 50f;

	bool ropeIsReset = false;

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

		if (anchored)
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
			
		ColorLerp( rb.velocity );
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		if (col != null)
		{
			Debug.Log("Collided with: " + col.transform.tag);
			/*Debug.Log( "Collided and VEL WAS:" + Mathf.Abs(rb.velocity.x) + ", " + Mathf.Abs(rb.velocity.y) );
			if ( ( Mathf.Abs(rb.velocity.x) > 5f ) || ( Mathf.Abs(rb.velocity.y) > 5f) )
				Death();*/

			//Death ();
			if (col.collider.tag == "Wall")
				inputManager.UnAnchor();
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

	void Death()
	{
		StartCoroutine("Deathy");
	}

	IEnumerator Deathy ()
	{
		gameObject.GetComponent<Renderer>().enabled = false;
		Time.timeScale = 0.6f;
		yield return new WaitForSeconds(0.8f);
		Application.LoadLevel("LevelGeneratiorTest");
		Time.timeScale = 1f;
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

	float Remap ( this float value, float from1, float to1, float from2, float to2 )
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

}
