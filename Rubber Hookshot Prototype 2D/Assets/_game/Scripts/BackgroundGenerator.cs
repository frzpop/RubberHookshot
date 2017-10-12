using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGImporter;

public class BackgroundGenerator : MonoBehaviour {

	public static BackgroundGenerator bg;

	public SVGRenderer[] smallMountains;
	public SVGRenderer[] mediumMountains;
	public SVGRenderer[] tallMountains;

	MountainIndexes mountainIndexes;

	const float yRange = 2f;
	const float zRange = 2f;
	const float scaleRange = 0.1f;

	float xPos;
	float yPos;
	float zPos;
	float xyScale;

	void Awake()
	{
		bg = this;
	}

	public void GenerateBackground( float start, float end )
	{
		xPos = start;
		yPos = 0f;
		zPos = 3.5f;

		while ( xPos < end )
		{
			yPos += Random.Range( -yRange, yRange );
			zPos += Random.Range( -zRange, zRange );
			xyScale = 1f + Random.Range( -scaleRange, scaleRange );

			yPos = Mathf.Clamp( yPos, -5f, 5f );
			zPos = Mathf.Clamp( zPos, 1f, 8f );

			Vector3 pos = new Vector3( xPos, yPos, zPos );
			Vector3 scale = new Vector3( xyScale, xyScale, 1f );

			SVGRenderer mountain = RequestRandomMountain();

			mountain.transform.position = pos;
			mountain.transform.localScale = scale;

			float width = mountain.meshRenderer.bounds.extents.magnitude;

			xPos += Random.Range( width / 2, width );
		}

	}

	SVGRenderer RequestRandomMountain()
	{
		int rng = Random.Range( 0, 3 );

		SVGRenderer randomMountain = null;

		switch ( rng )
		{
		case 0:
			randomMountain = smallMountains[mountainIndexes.smallIndex];
			mountainIndexes.smallIndex++;
			if ( mountainIndexes.smallIndex >= smallMountains.Length - 1 )
				mountainIndexes.smallIndex = 0;
			break;

		case 1:
			randomMountain = mediumMountains[mountainIndexes.mediumIndex];
			mountainIndexes.mediumIndex++;
			if ( mountainIndexes.mediumIndex >= mediumMountains.Length - 1 )
				mountainIndexes.mediumIndex = 0;
			break;

		case 2:
			randomMountain = tallMountains[mountainIndexes.tallIndex];
			mountainIndexes.tallIndex++;
			if ( mountainIndexes.tallIndex >= tallMountains.Length - 1 )
				mountainIndexes.tallIndex = 0;
			break;
		}

		return randomMountain;
	}

	struct MountainIndexes
	{
		public int smallIndex;
		public int mediumIndex;
		public int tallIndex;
	}

}
