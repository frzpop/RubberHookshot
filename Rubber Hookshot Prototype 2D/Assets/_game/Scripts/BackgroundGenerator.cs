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

	//const float yRange = 3f;
	Range scaleRange = new Range( 1f, 3f );
	Range tallScaleRange = new Range( 0.8f, 1.4f );
	Range zRange = new Range( 1f, 30f );

	float xPos;
	float yPos;
	float zPos;
	float xyScale;


	void Awake()
	{
		bg = this;
	}

	public void GenerateBackground( float start, float end, float floor )
	{
		xPos = start;

		float distanceFromCam;
		Camera cam = CameraFollow.cf.camera;

		while ( xPos < end )
		{
			//yPos += Random.Range( -yRange, yRange );
			zPos = Random.Range( zRange.min, zRange.max );

			distanceFromCam = (cam.transform.position.z - zPos).MakePositive();
			//float frustumHeight = cam.GetFrustumHeight( distanceFromCam );
			yPos = floor - ( distanceFromCam * 0.08f );
		
			Vector3 pos = new Vector3( xPos, yPos, zPos );

			SVGRenderer mountain = RequestRandomMountain();

			mountain.transform.position = pos;

			float width = mountain.meshRenderer.bounds.extents.magnitude;

			xPos += Random.Range( width * 0.75f, width * 1.5f  );
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
			xyScale = Random.Range( scaleRange.min, scaleRange.max );
			randomMountain.transform.localScale = new Vector3( xyScale, xyScale, 1f );

			mountainIndexes.smallIndex++;
			if ( mountainIndexes.smallIndex >= smallMountains.Length - 1 )
				mountainIndexes.smallIndex = 0;
			break;

		case 1:
			
			randomMountain = mediumMountains[mountainIndexes.mediumIndex];
			xyScale = Random.Range( scaleRange.min, scaleRange.max );
			randomMountain.transform.localScale = new Vector3( xyScale, xyScale, 1f );

			mountainIndexes.mediumIndex++;
			if ( mountainIndexes.mediumIndex >= mediumMountains.Length - 1 )
				mountainIndexes.mediumIndex = 0;
			break;

		case 2:
			
			randomMountain = tallMountains[mountainIndexes.tallIndex];
			xyScale = Random.Range( tallScaleRange.min, tallScaleRange.max );
			randomMountain.transform.localScale = new Vector3( xyScale, xyScale, 1f );

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

	struct Range
	{
		public float min;
		public float max;

		public Range( float min, float max )
		{
			this.min = min;
			this.max = max;
		}
	}


}
