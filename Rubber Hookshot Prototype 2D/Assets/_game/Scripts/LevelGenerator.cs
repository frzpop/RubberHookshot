using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {


	public GameObject straightWall;
	public GameObject[] Walls;
	GameObject lastSpawned;

	Vector3 pos = Vector3.zero;
	Quaternion rot = Quaternion.identity;
	float lastHeight;
	float heightDiff;

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{

			if (lastSpawned != null)
			{
				pos += new Vector3(lastSpawned.GetComponentInChildren<Renderer>().bounds.size.x, 0f, 0f);
				
			}
			else
			{
				int randomIndex = Random.Range(0, Walls.Length);

				lastSpawned = (GameObject)Instantiate(Walls[randomIndex], pos, Walls[randomIndex].transform.rotation);
				lastHeight = lastSpawned.GetComponentInChildren<Renderer>().bounds.size.y;
			}

			




		}




		/*
		if ( Input.GetKeyDown(KeyCode.Q) )
		{
			GameObject spawnedWall = (GameObject)Instantiate(wallPrefab, pos, rot);

			pos += new Vector3 (spawnedWall.GetComponentInChildren<Renderer>().bounds.size.x, 0f, 0f);

			spawnedWall = (GameObject)Instantiate(wallPrefab2, pos, wallPrefab2.transform.rotation);

			float floaty = Mathf.Abs( wallPrefab2.GetComponentInChildren<Renderer>().bounds.size.y - wallPrefab.GetComponentInChildren<Renderer>().bounds.size.y );
			print(floaty);

			pos += new Vector3(spawnedWall.GetComponentInChildren<Renderer>().bounds.size.x - 14f, floaty, 0f);

			spawnedWall = (GameObject)Instantiate(wallPrefab, pos, rot);
			

		}*/
	}
}
