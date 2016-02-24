using UnityEngine;
using System.Collections;

// THIS CLASS IS NOT NEEDED ANYMORE
public class LevelGenerator : MonoBehaviour {


	public GameObject straightWall;
	public GameObject[] Walls;
	GameObject lastSpawned;

	Vector3 pos = Vector3.zero;
	Quaternion rot = Quaternion.identity;
	float lastBlockHeight;
    float lastBlockWidth;
    float newBlockHeight;

    float heightDiff;


	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
      
            if (lastSpawned != null)
			{
                int randomIndex = Random.Range(0, Walls.Length);
                newBlockHeight = Walls[randomIndex].GetComponentInChildren<Renderer>().bounds.size.y;

                /*  if (lastBlockHeight != newBlockHeight)
                  {
                      heightDiff = Mathf.Abs(lastBlockHeight - newBlockHeight);

                      pos += new Vector3(lastSpawned.GetComponentInChildren<Renderer>().bounds.size.x - 14f, heightDiff, 0f);
                  }
                  else
                  {
                      pos += new Vector3(lastSpawned.GetComponentInChildren<Renderer>().bounds.size.x, 0f, 0f);
                  }*/

                pos += new Vector3(lastBlockWidth, 0f, 0f);
                lastSpawned = (GameObject)Instantiate(Walls[randomIndex], pos, Walls[randomIndex].transform.rotation);
                lastBlockHeight = lastSpawned.GetComponentInChildren<Renderer>().bounds.size.y;
            }
			else
			{
				int randomIndex = Random.Range(0, Walls.Length);

                lastSpawned = (GameObject)Instantiate(Walls[randomIndex], pos, Walls[randomIndex].transform.rotation);

				lastBlockHeight = lastSpawned.GetComponentInChildren<Renderer>().bounds.size.y;
                lastBlockWidth = lastSpawned.GetComponentInChildren<Renderer>().bounds.size.x;

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
