using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {

	public static float MakePositive( this float value )
	{
		return Mathf.Abs( value );
	}

	public static float GetHighestValueY( this Vector2[] vector2s )
	{
		float highest = Mathf.NegativeInfinity;
		for ( int i = 0; i < vector2s.Length; i++ )
		{
			if ( vector2s[i].y > highest )
				highest = vector2s[i].y;
		}
		return highest;
	}

	public static float GetLowestValueY( this Vector2[] vector2s )
	{
		float lowest = Mathf.Infinity;
		for ( int i = 0; i < vector2s.Length; i++ )
		{
			if ( vector2s[i].y < lowest )
				lowest = vector2s[i].y;
		}
		return lowest;
	}

	public static float GetFrustumHeight( this Camera camera, float distance )
	{
		return 2.0f * distance * Mathf.Tan( camera.fieldOfView * 0.5f * Mathf.Deg2Rad );
	}


}
