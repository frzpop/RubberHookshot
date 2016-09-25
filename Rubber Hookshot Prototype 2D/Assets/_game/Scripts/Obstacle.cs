using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	public GameObject spikes;

	void Start ()
	{
		int rndInt = Random.Range ( 1, 7 );
		SizeMe ( rndInt );
	}

	public void Spiked ( float chance )
	{
		chance = RemapValue( chance, 0f, 1f, 1f, 0f );
		if (Random.value > chance)
		{
			spikes.SetActive(true);
		}
	}

	public void SizeMe ( int size )
	{
		Vector3 scale = Vector3.zero;
		if (size == 1)
			scale = new Vector3( 15f, 15f, 0f );
		else if (size == 2)
			scale = new Vector3( 15f, 15f, 0f );
		else if (size == 3)
			scale = new Vector3( 15f, 15f, 0f );
		else if (size == 4)
			scale = new Vector3( 20f, 20f, 0f );
		else if (size == 5)
			scale = new Vector3( 20f, 20f, 0f );
		else if (size == 6)
			scale = new Vector3( 30f, 30f, 0 );
		else if (size == 7)
			scale = new Vector3( 40f, 40f, 0 );
	
		gameObject.transform.localScale = scale;

	}

	float RemapValue ( float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

}
