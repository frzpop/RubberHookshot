using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {


	public Transform target;

	Vector3 follow;
	float offset; // This is the difference in Y between ground and roof divided by two.

	Vector3 playerVelocity; // To GameManager?
	float velocityX;

	void Start ()
	{
		follow = transform.position;
	}

	void Update ()
	{
        playerVelocity = target.gameObject.GetComponent<Rigidbody2D>().velocity;

		Vector2 origin = new Vector2( target.position.x + 10f, target.position.y );
		RaycastHit2D[] hit = Physics2D.RaycastAll( origin, Vector2.down );

		float center = 0f;
		for ( int i = 0; i < hit.Length; i++ )
		{
			if ( ( hit[i].collider != null ) && hit[i].collider.tag == "Wall" )
			{
				center = hit[i].point.y + offset;
				// X is offset so that the player is on the left of the screen
				// X is also offset based on player velocity so that you see more in the direction you are heading.
				// Y is clamped to center so that you see more of the level
				follow = new Vector3( target.position.x + 20f + velocityX, Mathf.Clamp( target.position.y, center - 10f, center + 10f), transform.position.z );
			}
		}
	}

	void FixedUpdate () 
	{
		velocityX = Mathf.Lerp( velocityX, playerVelocity.x * 0.35f, 0.2f );
		velocityX = Mathf.Clamp( velocityX, -25f, 25f );
		transform.position = Vector3.Lerp( transform.position, follow, 0.2f );
	}

	public void SetOffset ( float myOffset )
	{
		offset = myOffset / 2;
	}
}
