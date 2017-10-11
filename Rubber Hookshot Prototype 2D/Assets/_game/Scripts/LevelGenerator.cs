using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour {

	public static LevelGenerator lg;

	public CameraFollow cam;
	public GameObject edgeColPrefab;
	public GameObject meshPrefab;
	public GameObject anchorPrefab;
	public GameObject checkPointPrefab;
	public GameObject obstaclePrefab;
	public GameObject[] edgeCols;
	public GameObject[] meshes;
	public Transform levelObjectsParent;
	public Transform backgroundParent;

	Vector2 start = Vector2.zero;
	EdgeCollider2D generatedCol;
	EdgeCollider2D generatedCol2;
	GameObject generatedMesh;
	GameObject generatedMesh2;

	Vector2[] myPoints = new Vector2[7]; // higher number increases "curviness". Also affects edgeCount

	float roofOffset = 85f;
	float meshHeight = 60f;

	public List<Vector3> positions;

	void Awake ()
    {
		lg = this;
		GenerateLevel();
	}

	public void GenerateLevel ()
	{
		// Get a few pseudo random points
		Vector2[] randomPoints = RandomPoints( start, 300f, myPoints );
	
		//make points into a curve
		int res = 20; // Highter number increases edgeCount;
		List<Vector2> curveVerts = MultiCurve( randomPoints, res );
	
		//Generate colission using curve vertices
		generatedCol = SpawnCol ( curveVerts, Vector2.zero, Quaternion.identity );
		Vector3 heightOffset = new Vector3( 0f, roofOffset, 0f) ;
		generatedCol2 = SpawnCol( curveVerts, generatedCol.transform.position + heightOffset, Quaternion.identity );

		//Generate two 2D meshes that correspond with the collision
		generatedMesh = GenerateMesh2D( generatedCol, meshHeight, generatedCol.transform.position, Quaternion.identity );
		Vector3 heightOffsetMesh = new Vector3( 0f, roofOffset + meshHeight, 0f );
		generatedMesh2 = GenerateMesh2D( generatedCol, meshHeight, generatedCol.transform.position + heightOffsetMesh, Quaternion.identity );

		//Generate anchors
		GenerateAnchors( curveVerts, roofOffset );

		//Generate Obstacles
		GenerateObstacles ( curveVerts, roofOffset, 0.05f );

		//Spawn a checkpoint
		Vector3 cpPos = new Vector3( start.x + ( generatedMesh.GetComponent<MeshRenderer>().bounds.size.x * 0.5f ) , start.y, -0.1f );
		Instantiate( checkPointPrefab, cpPos, Quaternion.identity, levelObjectsParent );

		//Set next start position 
		start = generatedCol.points[generatedCol.pointCount - 1];

		cam.SetOffset(roofOffset);
	}

	Vector2[] RandomPoints ( Vector2 startPoint, float width, Vector2[]points )
    {
        Vector2 prevPoint = Vector2.zero;
        float increase = width / ( points.Length - 1 );
        float yDif = width / 18f;
        float yDifNeg = -yDif;
      
        for (int i = 0; i < points.Length; i++)
        {
            if ( i == 0 )
            {
                points[i] = startPoint;
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
    }

	List<Vector2> MultiCurve ( Vector2[] points, int res )
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

				vertices.AddRange( GeneratePoints( start, cp, end, res ) );
			}
			else if (i != 1)
			{
				start = end;
				cp = points[i];
				end = (points[i] + points[i + 1]) / 2;

				vertices.AddRange( GeneratePoints( start, cp, end, res ) );
			}
		}

		start = end;
		cp = points[points.Length - 2];
		end = points[points.Length - 1];

		vertices.AddRange(GeneratePoints( start, cp, end, res + 5) );

		//SpawnCol(vertices);
		return vertices;
	}

	EdgeCollider2D SpawnCol ( Vector2[] points, Vector2 pos, Quaternion rot, string edgeName )
	{
		EdgeCollider2D edge = RequestEdgeCol().GetComponent<EdgeCollider2D>();
		edge.points = points;
		edge.transform.position = pos;
		edge.transform.rotation = rot;
		edge.name = edgeName;
		return edge;
	}
	EdgeCollider2D SpawnCol ( List<Vector2> points, Vector2 pos, Quaternion rot )
    {
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

        for (int i = 0; i < myArray.Length ; i++)
        {
            myArray[i] = QuadraticBezier(start, cp, end, t);
            t += (1f / myArray.Length );
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

	GameObject GenerateMesh2D ( EdgeCollider2D shape, float height, Vector2 pos, Quaternion rot )
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
		Vector2[] uvs	= new Vector2[ vertsCount ];

		//Define vertices
		for (int i = 0; i < vertsCount; i++)
		{
			if (i % 2 == 0) // if index is even
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

		//TODO: Define UVs 
		for (int i = 0; i < vertices.Length; i++)
		{

		}

		//TODO: Define normals
		for (int i = 0; i < vertsCount; i++)
			normals[i] = Vector3.back;

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

	void GenerateAnchors ( List<Vector2> myPoints, float distance )
	{
		float lastY = 0f;
		float lastX = 0f; // might not be needed
		float bottomPadding = 12f;
		float topPadding = 8f;

		for (int i = 0; i < myPoints.Count; i++)
		{
			float minY;
			float maxY;
			int freq = 7; // a higher number will result in fewer anchors

			// Lower
			if (i == 0) // first time
			{
				minY = myPoints[i].y + bottomPadding;
				maxY = myPoints[i].y + (distance / 2) - topPadding;

				float x = myPoints[i].x + Random.Range(-1.75f, 1.75f);
				//float x = myPoints[i].x;
				float y = Random.Range(minY, maxY);
				Vector3 pos = new Vector3(x, y, 0f);
				Instantiate( anchorPrefab, pos, Quaternion.identity, levelObjectsParent );
				lastX = pos.x;
				lastY = pos.y;
			}
			else if (i % freq == 0)
			{
				float x = myPoints[i].x + Random.Range(-1.75f, 1.75f);
				
				minY = myPoints[i].y + bottomPadding;
				maxY = myPoints[i].y + (distance / 2) - topPadding;

				if (lastY > minY + (distance / 4f)) // above
					maxY = lastY - topPadding;
				else // under
					minY = lastY + bottomPadding;

				float y = Random.Range(minY, maxY);
				Vector3 pos = new Vector3(x, y, 0f);
				Instantiate( anchorPrefab, pos, Quaternion.identity, levelObjectsParent );
				lastX = pos.x;
				lastY = pos.y;
			}

			// Upper
			if (i == 0) // first time
			{
				minY = myPoints[i].y + (distance / 2) + bottomPadding;
				maxY = myPoints[i].y + distance - topPadding;

				float x = myPoints[i].x + Random.Range(-1.75f, 1.75f);
				float y = Random.Range(minY, maxY);
				Vector3 pos = new Vector3(x, y, 0f);
				Instantiate( anchorPrefab, pos, Quaternion.identity, levelObjectsParent );
				lastX = pos.x;
				lastY = pos.y;
			}
			else if (i % freq + 1 == 0)
			{
				float x = myPoints[i].x + Random.Range(-1.75f, 1.75f);

				minY = myPoints[i].y + (distance / 2) + bottomPadding;
				maxY = myPoints[i].y + distance - topPadding;

				if (lastY > minY + (distance / 4f)) // above
					maxY = lastY - topPadding;
				else // under
					minY = lastY + bottomPadding;

				float y = Random.Range(minY, maxY);
				Vector3 pos = new Vector3(x, y, 0f);
				Instantiate( anchorPrefab, pos, Quaternion.identity, levelObjectsParent );
				lastX = pos.x;
				lastY = pos.y;
			}
		}
	}
		
	void GenerateObstacles ( List<Vector2> myPoints, float distance, float difi )
	{
		float minY;
		float maxY;
		float padding = 10f;
		int divide = 20;

		int amount = myPoints.Count / divide;
		int myIndex = 0;
		Vector3 myPos;
		GameObject spawnedObs;

		for (int i = 0; i < amount; i++) 
		{
			myIndex += divide;
			minY = myPoints[myIndex].y + padding;
			maxY = myPoints[myIndex].y + distance - padding;

			float x = myPoints[myIndex].x + Random.Range(-10f, 10f);
			float y = Random.Range (minY, maxY);
			myPos = new Vector3 ( x, y, -0.1f );
			spawnedObs = Instantiate( obstaclePrefab, myPos, Quaternion.identity, levelObjectsParent );
			spawnedObs.GetComponent<Obstacle> ().Spiked (difi);
		}	
	}

	Vector3 RandomAround(Vector3 origin, float minDist, float maxDist, float minAngle, float maxAngle)
	{
		Vector3 pos = Quaternion.AngleAxis(Random.Range(minAngle, maxAngle), Vector3.up) * Vector3.forward;
		pos = pos * Random.Range(minDist, maxDist);
		return origin + pos;
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

	public float[] GetColVertsX ()
	{
		float[] myArray = new float[generatedCol.edgeCount];
		for (int i = 0; i < generatedCol.edgeCount; i++) 
			myArray [i] = generatedCol.points[i].x;

		return myArray;
	}

}
