using UnityEngine;
using System.Collections;
using UnityEditor;

public class EdgeColliderGenerator : MonoBehaviour {

    public EdgeCollider2D edge;
   //public EdgeCollider2D edge2;
	public GameObject edgePrefab;

	public Vector2[] test;
	public Vector2[] multiTest;
    Vector2 myStart = new Vector2(0f, 0f);
    Vector2 myCp = new Vector2(5f, 5f);
    Vector2 myEnd = new Vector2(10f, 0f);

    Vector2 myStart2 = new Vector2(10f, 0f);
    Vector2 myCp2 = new Vector2(15f, -5f);
    Vector2 myEnd2 = new Vector2(20f, 0f);


    void Start ()
    {
		/*print(test.Length);
        test = GeneratePoints( myStart, myCp, myEnd, test );
        edge.points = test;

        test = GeneratePoints( myStart2, myCp2, myEnd2, test );
        edge2.points = test;*/

		//multiTest = edge.points;
		//edge.points = MultiCurve(edge.points);
		//edge.points = GeneratePoints(myStart2, myCp2, myEnd2, test);



	}

	void Update ()
    {
        if ( Input.GetKeyDown(KeyCode.P) )
        {
			MultiCurve(multiTest);

		}
	
	}

	void SpawnCol ( Vector2[] points )
	{
		GameObject spawnedCol = (GameObject)Instantiate( edgePrefab, Vector2.zero, Quaternion.identity );
		spawnedCol.GetComponent<EdgeCollider2D>().points = points;
		
	}

    Vector2[] GeneratePoints ( Vector2 start, Vector2 cp, Vector2 end, int amountOfPoints )
    {
        Vector2[] myArray = new Vector2[amountOfPoints];

        float t = 0f;

        for (int i = 0; i < myArray.Length -1 ; i++)
        {
            myArray[i] = QuadraticBezier(start, cp, end, t);
            t += (1f / myArray.Length);
        }
        myArray[myArray.Length - 1] = end;
		return myArray;
    }

    Vector2 QuadraticBezier (Vector2 start, Vector2 cp, Vector2 end, float t)
    {
        Vector2 pFinal = Vector2.zero;

        pFinal.x = Mathf.Pow(1 - t, 2) * start.x +
                   (1 - t) * 2 * t * cp.x +
                   t * t * end.x;
        pFinal.y = Mathf.Pow(1 - t, 2) * start.y +
                   (1 - t) * 2 * t * cp.y +
                   t * t * end.y;

        return pFinal;
    }

   void MultiCurve ( Vector2[]points )
    {
		Vector2[] myArray = new Vector2[points.Length];
	
		Vector2 start = Vector2.zero;
		Vector2 cp = Vector2.zero;
		Vector2 end = Vector2.zero;

		for (int i = 0; i < points.Length - 2; i++)
        {
			if ( i == 0 )
			{
				start = points[i];
				cp = points[i + 1];
				end = (cp + points[i + 2]) / 2;

				SpawnCol( GeneratePoints( start, cp, end, points.Length ) );


			}
			else if ( i != 1 )
			{
				start = end;
				cp = points[i];
				end = ( points[i] + points[i + 1] ) / 2;

				SpawnCol( GeneratePoints( start, cp, end, points.Length ) );
			} 
		}

		start = end;
        cp = points[points.Length - 2];
		end = points[points.Length - 1];

		SpawnCol( GeneratePoints( start, cp, end, points.Length ) );

	}
}
