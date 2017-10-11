using UnityEngine;

public class CheckPoint : MonoBehaviour {


	void OnTriggerEnter2D ( Collider2D other )
	{
		if ( other.tag == "Player" )
		{
			LevelGenerator.lg.GenerateLevel();	
			Destroy(gameObject);
		}
	}
}
