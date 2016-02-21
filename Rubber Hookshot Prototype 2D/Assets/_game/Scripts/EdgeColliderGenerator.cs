using UnityEngine;
using System.Collections;
using UnityEditor;

public class EdgeColliderGenerator : MonoBehaviour {

    public EdgeCollider2D edge;
    public EdgeCollider2D edge2;
    public Vector2[] test;
    Vector2 myStart = new Vector2(0f, 0f);
    Vector2 myCp = new Vector2(5f, 5f);
    Vector2 myEnd = new Vector2(10f, 0f);

    Vector2 myStart2 = new Vector2(10f, 0f);
    Vector2 myCp2 = new Vector2(15f, -5f);
    Vector2 myEnd2 = new Vector2(20f, 0f);


    void Start ()
    {
        print(test.Length);
        test = GeneratePoints( myStart, myCp, myEnd, test );
        edge.points = test;

        test = GeneratePoints( myStart2, myCp2, myEnd2, test );
        edge2.points = test;
    }

	void Update ()
    {
        if ( Input.GetKeyDown(KeyCode.P) )
        {
            
        }
	
	}

    Vector2[] GeneratePoints ( Vector2 start, Vector2 cp, Vector2 end, Vector2[] points )
    {
        Vector2[] myArray = new Vector2[points.Length];

        float t = 0f;

        for (int i = 0; i < points.Length -1 ; i++)
        {
            myArray[i] = quadraticBezier(start, cp, end, t);
            t += (1f / points.Length);
        }
        myArray[myArray.Length - 1] = end;
        return myArray;
    }

    Vector2 quadraticBezier (Vector2 start, Vector2 cp, Vector2 end, float t)
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

    void GenerateCurve ( Vector2[]points )
    {
        Vector2 p0;
        Vector2 p1;
        float midX;
        float midY;

        for (int i = 0; i < points.Length - 2; i++)
        {
            p0 = points[i];
            p1 = points[i + 1];
            midX = ( p0.x + p1.x ) / 2;
            midY = (p0.y + p1.y) / 2;

            //Handles.DrawBezier(p0, p1);
        }

        p0 = points[points.Length - 2];
        p1 = points[points.Length - 1];
    }
}
