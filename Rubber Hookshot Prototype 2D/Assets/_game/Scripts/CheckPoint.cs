using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {

	LevelGeneratorNew levelGenerator;

	void OnTriggerEnter2D ( Collider2D other )
	{
		//Debug.Log("Collided with " + other.tag);
		if ( other.tag == "Player" )
		{
			levelGenerator.GenerateLevel();	
			Destroy(gameObject);
		}
	}

	public void SetLevelGenerator ( LevelGeneratorNew levelGen )
	{
		levelGenerator = levelGen;
	}
}
