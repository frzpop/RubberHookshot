using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	public GameObject spikes;

	public void Spiked ( float chance )
	{
		chance = Remap( chance, 0f, 1f, 1f, 0f );
		if (Random.value > chance)
		{
			spikes.SetActive(true);
		}
	}

	public void SizeMe ( int size )
	{
		Vector3 scale = Vector3.zero;
		if (size == 1)
			scale = new Vector3( 15f, 15f, 0 );
		else if (size == 2)
			scale = new Vector3( 25f, 25f, 0);
		else if (size == 3)
			scale = new Vector3( 40f, 40f, 0 );
	
		gameObject.transform.localScale = scale;

	}

	float Remap (this float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

}
