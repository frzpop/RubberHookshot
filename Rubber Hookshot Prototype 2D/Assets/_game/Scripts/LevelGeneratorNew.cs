using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGeneratorNew : MonoBehaviour {

	public GameObject edgeColPrefab;
	public GameObject meshPrefab;
	GameObject[] edgeCols = new GameObject[6];
	EdgeCollider2D edgeCol;
	EdgeCollider2D edgeCol2;

	GameObject[] meshes = new GameObject[6];
	
	Vector2[] myPoints = new Vector2[7];

	void Start ()
    {


		//Initialize meshes pool
		for (int i = 0; i < meshes.Length; i++)
			meshes[i] = (GameObject)Instantiate( meshPrefab, new Vector3( -1000f, -1000f, 0f ), Quaternion.identity );
	
		//Initialize edgeCols pool
		for (int i = 0; i < edgeCols.Length; i++)
			edgeCols[i] = (GameObject)Instantiate( edgeColPrefab, new Vector3( -1000f, -1000f, 0f ), Quaternion.identity );

		// Get a few pseudo random points
		Vector2[] randomPoints = RandomPoints( Vector2.zero, 300f, myPoints );

		//make points into a curve
		List<Vector2> curveVerts = MultiCurve( randomPoints, 14 );

		//Generate colission using curve vertices
		edgeCol = SpawnCol( curveVerts, Vector2.zero, Quaternion.identity );

		//Generate a 2D mesh that corresponds with the collision
		GenerateMesh2D( edgeCol, edgeCol.transform.position, Quaternion.identity );
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.W))
			RequestEdgeCol();
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

	void GenerateMesh2D( EdgeCollider2D shape, Vector2 pos, Quaternion rot )
	{
		int vertsInShape = shape.pointCount;
		int vertsCount = vertsInShape * 2;
		int triCount = shape.edgeCount * 2;
		int triIndexCount = triCount * 3;
		Vector2 myOffset = new Vector2(0, 30f);
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
}
