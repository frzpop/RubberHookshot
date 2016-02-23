using UnityEngine;
using System.Collections;

public class MeshGenerator : MonoBehaviour {

	Mesh mesh;
	public EdgeCollider2D shape;
	public Vector3[] vertsTest;
	public int[] triangleIndicesTEST;

	void Start ()
	{


		MeshFilter mf = GetComponent<MeshFilter>();
		if (mf.sharedMesh == null)
			mf.sharedMesh = new Mesh();
		mesh = mf.sharedMesh;



		/*Vector3[] vertices = new Vector3[]
			{
				new Vector3( 0, 1, 0 ),
				new Vector3( 0, 0, 0 ),
				new Vector3( 1, 1, 0 ),
				new Vector3( 1, 0, 0 ),
				new Vector3( 2, 1.5f, 0 ),
				new Vector3( 2, 0.5f, 0 )
			};

		Vector3[] normals = new Vector3[]
			{
				Vector3.back,
				Vector3.back,
				Vector3.back,
				Vector3.back,
				Vector3.back,
				Vector3.back
			};

		int[] triangleIndices = new int[]
			{
				0, 2, 3,
				3, 1, 0,
				2, 4, 5,
				5, 3, 2 
			};

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.triangles = triangleIndices;
		Debug.Log("yo");*/
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.M))
			GenerateShape2D(mesh, shape);
	}
	

	void GenerateShape2D ( Mesh mesh, EdgeCollider2D shape )
	{
		int vertsInShape = shape.pointCount;
		int vertsCount = vertsInShape * 2;
		int triCount = shape.edgeCount * 2;
		int triIndexCount = triCount * 3;
		Vector2 myOffset = new Vector2( 0, 30f );
		int[] lines = new int[ shape.edgeCount ];

		int[] triangleIndices	= new int[  triIndexCount ];
		Vector3[] vertices		= new Vector3[ vertsCount ];
		Vector3[] normals		= new Vector3[ vertsCount ];
		//Vector2[] uvs			= new Vector2[ vertsCount ];
		
		//Define vertices
		for ( int i = 0; i < vertsCount; i++ )
		{
			if ( i % 2 == 0 ) // if i is even
			{
				vertices[ i ] = shape.points[ i / 2] ;
				vertices[ i + 1 ] = shape.points[ i / 2] - myOffset;
			}
		}
		vertsTest = vertices;

		/*
			//Define triangles JOCHES SKIT JAG INTE FÅR ATT FUNGERA
			int counter = 0;
			int ti = 0;
			int off = vertsInShape;
			for ( int i = 0; i < shape.edgeCount - 1; i ++ )
			{
				counter += 1;
				int a = off + lines[ i ] + vertsInShape;
				int b = off + lines[ i ];
				int c = off + lines[ i + 1];
				int d = off + lines[i + 1] + vertsInShape;
				triangleIndices[ti] = a;	ti++;
				triangleIndices[ti] = b;	ti++;
				triangleIndices[ti] = c;	ti++;
				triangleIndices[ti] = c;	ti++;
				triangleIndices[ti] = d;	ti++;
				triangleIndices[ti] = a;	ti++;
				print(counter);
			}*/

		//Define triangles
		for (int i = 0; i < shape.edgeCount; i++)
		{
			int a;
			int b;
			int c;
			int d;
			int e;
			int f;

			if (i % 2 == 0) // if index is even
			{
				a = i;
				b = i + 2;
				c = i + 3;
				d = i - 2;

				triangleIndices[ i ] = a;
				triangleIndices[ i + 1 ] = b;
				triangleIndices[ i + 2 ] = c;
				triangleIndices[ i + 3 ] = b;
				triangleIndices[ i + 4 ] = a;
				triangleIndices[ i + 5 ] = d;
			}
			else
			{
				//TODO TODO TODO
				a = i + 1;
				b = i;
				c = i - 2;

				triangleIndices[i] = a;
				triangleIndices[i + 1] = b;
				triangleIndices[i + 2] = c;
			}
		}
		triangleIndicesTEST = triangleIndices;

		//define normals temp
		for (int i = 0; i < vertsCount; i++)
		{
			normals[i] = Vector3.back;
		}

		
		//Assign values to mesh
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.normals = normals;
		//mesh.uv = uvs;
		mesh.triangles = triangleIndices;

	}
}
