﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGeneratorNew : MonoBehaviour {

	public GameObject edgeColPrefab;
	public GameObject meshPrefab;
	public GameObject anchorPrefab;
	GameObject[] edgeCols = new GameObject[6];
	EdgeCollider2D generatedCol;
	EdgeCollider2D generatedCol2;
	GameObject[] meshes = new GameObject[6];
	GameObject generatedMesh;
	GameObject generatedMesh2;

	Vector2[] myPoints = new Vector2[7];

	float roofOffset = 50f;
	float meshHeight = 30f;

	public List<Vector3> positions;

	void Start ()
    {
		//Initialize meshes pool
		for (int i = 0; i < meshes.Length; i++)
			meshes[i] = (GameObject)Instantiate( meshPrefab, new Vector3( -1000f, -1000f, 0f ), Quaternion.identity );
	
		//Initialize edgeCols pool
		for (int i = 0; i < edgeCols.Length; i++)
			edgeCols[i] = (GameObject)Instantiate( edgeColPrefab, new Vector3( -1000f, -1000f, 0f ), Quaternion.identity );

		GenerateLevel();
	}

	void Update ()
	{
		
	}

	void GenerateLevel ()
	{
		// Get a few pseudo random points
		Vector2[] randomPoints = RandomPoints(Vector2.zero, 300f, myPoints);

		//make points into a curve
		List<Vector2> curveVerts = MultiCurve(randomPoints, 14);

		//Generate colission using curve vertices
		generatedCol = SpawnCol(curveVerts, Vector2.zero, Quaternion.identity);
		Vector3 heightOffset = new Vector3( 0f, roofOffset, 0f) ;
		generatedCol2 = SpawnCol(curveVerts, generatedCol.transform.position + heightOffset, Quaternion.identity);

		//Generate two 2D meshes that corresponds with the collision
		generatedMesh = GenerateMesh2D(generatedCol, meshHeight, generatedCol.transform.position, Quaternion.identity);
		Vector3 heightOffsetMesh = new Vector3( 0f, roofOffset + meshHeight, 0f );
		generatedMesh2 = GenerateMesh2D(generatedCol, meshHeight, generatedCol.transform.position + heightOffsetMesh, Quaternion.identity);

		//generate anchors
		float xMin = generatedCol.transform.position.x;
		float xMax = xMin + generatedMesh.GetComponent<MeshRenderer>().bounds.size.x;
		float yMin = GetLowest( generatedCol.points, false );
		float yMax = generatedCol2.transform.position.y + GetHighest( generatedCol2.points, false );

		GenerateAnchors( 20, 20, xMin, xMax, yMin, yMax );
	
	}

	void GenerateAnchors ( int minObj, int maxObj, float xMin, float xMax, float yMin, float yMax )
	{
		int anchorsAmount = Random.Range( minObj, maxObj );
		GameObject[] anchors = new GameObject[anchorsAmount];
		positions = new List<Vector3>();
		

		for (int i = 0; i < anchors.Length; i++)
		{
			anchors[i] = anchorPrefab;
			
			Vector3 pos = GetRandomPos( xMin, xMax, yMin, yMax, positions );
			positions.Add(pos);
			Instantiate( anchors[i], pos, Quaternion.identity );
		}
		//return positions;*/

	}

	Vector3 GetRandomPos (float xMin, float xMax, float yMin, float yMax, List<Vector3> positions )
	{
		Vector3 pos = Vector3.zero;
		float minDistance = 25f;
		bool loop = true;
		bool isTooClose = false;
		int tries = 0;

		while ( loop )
		{
			float x = Random.Range(xMin, xMax);
			float y = Random.Range(yMin, yMax);
			pos = new Vector3(x, y, 0f);
			tries++;

			if (positions.Count != 0)
			{
				for (int i = 0; i < positions.Count; i++)
				{
					if (Mathf.Abs(Vector3.Distance(pos, positions[i])) < minDistance)
					{
						isTooClose = true;
						break;
					}
					else
						isTooClose = false;
				}

				if (!isTooClose)
					loop = false;
				else if (tries > 5)
				{
					print("I tried and failed");
					return pos;
				}
					

			}
			else
			{
				loop = false;
				print("list was empty so stop loop");
			}
				

			
		}
		print("we did it!");
		return pos;

	
	}

    Vector2[] RandomPoints ( Vector2 start, float width, Vector2[]points )
    {
        Vector2 prevPoint = Vector2.zero;
        float increase = width / ( points.Length - 1 );
        float yDif = width / 10f;
        float yDifNeg = -yDif;
      
        for (int i = 0; i < points.Length; i++)
        {
            if ( i == 0 )
            {
                points[i] = start;
                prevPoint = points[i];           
            }
            else
            {
                float yRandom;
                if (prevPoint.y > 0f)
                    yRandom = Random.Range( yDifNeg, ( yDifNeg * 2 ) );
                else
                    yRandom = Random.Range( yDif, yDif * 2 );

                points[i] = new Vector2( prevPoint.x + increase, prevPoint.y + yRandom );
                prevPoint = points[i];                
            }                        
        }
		return points;
		//MultiCurve( points, 14 );
    }

	List<Vector2> MultiCurve(Vector2[] points, int res)
	{
		Vector2 start = Vector2.zero;
		Vector2 cp = Vector2.zero;
		Vector2 end = Vector2.zero;
		List<Vector2> vertices = new List<Vector2>();

		for (int i = 0; i < points.Length - 2; i++)
		{
			if (i == 0)
			{
				start = points[i];
				cp = points[i + 1];
				end = (cp + points[i + 2]) / 2;

				//SpawnCol( GeneratePoints( start, cp, end, points.Length ) ); // Used for separate objects.
				vertices.AddRange( GeneratePoints( start, cp, end, res ) );
			}
			else if (i != 1)
			{
				start = end;
				cp = points[i];
				end = (points[i] + points[i + 1]) / 2;

				//SpawnCol( GeneratePoints( start, cp, end, points.Length ) ); // Used for separate objects.
				vertices.AddRange( GeneratePoints( start, cp, end, res ) );
			}
		}

		start = end;
		cp = points[points.Length - 2];
		end = points[points.Length - 1];

		//SpawnCol( GeneratePoints( start, cp, end, points.Length ) ); // Used for separate objects.
		vertices.AddRange(GeneratePoints( start, cp, end, points.Length ) );

		//SpawnCol(vertices);
		return vertices;
	}

	EdgeCollider2D SpawnCol ( Vector2[] points, Vector2 pos, Quaternion rot )
	{
		//GameObject spawnedCol = (GameObject)Instantiate( edgeColPrefab, pos, rot); trying pooling for now
		EdgeCollider2D edge = RequestEdgeCol().GetComponent<EdgeCollider2D>();
		edge.points = points;
		edge.transform.position = pos;
		edge.transform.rotation = rot;
		return edge;
	}
	EdgeCollider2D SpawnCol( List<Vector2> points, Vector2 pos, Quaternion rot )
    {
        //GameObject spawnedCol = (GameObject)Instantiate( edgeColPrefab, pos, rot); trying pooling for now
		EdgeCollider2D edge = RequestEdgeCol().GetComponent<EdgeCollider2D>();
		edge.points = points.ToArray();
		edge.transform.position = pos;
		edge.transform.rotation = rot;
		return edge;
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

	GameObject GenerateMesh2D( EdgeCollider2D shape, float height, Vector2 pos, Quaternion rot )
	{
		int vertsInShape = shape.pointCount;
		int vertsCount = vertsInShape * 2;
		int triCount = shape.edgeCount * 2;
		int triIndexCount = triCount * 3;
		Vector2 myOffset = new Vector2(0, height);
		int[] lines = new int[shape.edgeCount];

		int[] triangleIndices = new int[triIndexCount];
		Vector3[] vertices = new Vector3[vertsCount];
		Vector3[] normals = new Vector3[vertsCount];
		//Vector2[] uvs	= new Vector2[ vertsCount ];

		//Define vertices
		for (int i = 0; i < vertsCount; i++)
		{
			if (i % 2 == 0) // if i is even
			{
				vertices[i] = shape.points[i / 2];
				vertices[i + 1] = shape.points[i / 2] - myOffset;
			}
		}

		//Define triangles
		int off = 0;
		int ti = 0;
		for (int i = 0; i < shape.edgeCount; i++)
		{
			int a = i + off; ;
			int b = i + 2 + off;
			int c = i + 3 + off;
			int d = i + 1 + off;

			triangleIndices[ti] = a; ti++;
			triangleIndices[ti] = b; ti++;
			triangleIndices[ti] = c; ti++;

			triangleIndices[ti] = c; ti++;
			triangleIndices[ti] = d; ti++;
			triangleIndices[ti] = a; ti++;

			off += 1;
		}

		//define normals TEMP?
		for (int i = 0; i < vertsCount; i++)
		{
			normals[i] = Vector3.back;
		}

		//Request mesh from pool
		GameObject meshObj = RequestMesh();
		MeshFilter mf = meshObj.GetComponent<MeshFilter>();
		if ( mf.sharedMesh == null )
			mf.sharedMesh = new Mesh();
		Mesh mesh = mf.sharedMesh;

		//Assign values to mesh
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.normals = normals;
		//mesh.uv = uvs;
		mesh.triangles = triangleIndices;

		//Move mesh
		meshObj.transform.position = pos;
		meshObj.transform.rotation = rot;

		return meshObj;
	}

	int ec = 0;
	int ecLoop = 5;
	GameObject RequestEdgeCol ()
	{
		ec++;
		if ( ec > ecLoop )
			ec = 0;
		return edgeCols[ec];
	}

	int mi = 0;
	int miLoop = 5;
	GameObject RequestMesh()
	{
		mi++;
		if ( mi > miLoop )
			mi = 0;

		return meshes[mi];
	}

	float GetHighest ( Vector2[] points, bool RetX )
	{
		float x = points[0].x;
		float y = points[0].y;
		if (RetX)
		{
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].x > x)
				{
					x = points[i].x;
				}
			}
			return x;
		}
		else
		{
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].y > y)
				{
					y = points[i].y;
				}
			}
			return y;
		}
		
	}

	float GetLowest( Vector2[] points, bool RetX )
	{
		float x = points[0].x;
		float y = points[0].y;
		if (RetX)
		{
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].x < x)
				{
					x = points[i].x;
				}
			}
			return x;
		}
		else
		{
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].y < y)
				{
					y = points[i].y;
				}
			}
			return y;
		}

	}
}
