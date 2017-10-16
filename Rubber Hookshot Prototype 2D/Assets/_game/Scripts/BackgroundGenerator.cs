using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGImporter;

public class BackgroundGenerator : MonoBehaviour {

	public static BackgroundGenerator bg;


	public SVGRenderer[] bgObjects;
	public SVGAsset[] mountainSVGs;
	public SVGAsset[] cloudSVGs;

	Range mountainScaleRange = new Range( 1f, 3f );
	Range mountainScaleRangeTall = new Range( 0.8f, 1.4f );
	Range mountainZRange = new Range( 3f, 33f );

	Range cloudScaleRange = new Range( 4f, 6f );
	Range cloudYRange = new Range( 8f, 90f );
	Range cloudZRange = new Range( 7f, 40f );

	void Awake()
	{
		bg = this;
	}

	public void GenerateBackground( float start, float end, float floor )
	{
		GenerateMountains( start, end, floor );

		GenerateClouds( start, end );
	}

	void GenerateMountains( float start, float end, float floor )
	{
		float xPos = start + 15f;
		float yPos;
		float zPos;

		float distanceFromCam;
		Camera cam = CameraFollow.cf.camera;

		while ( xPos < end )
		{
			SVGRenderer mountain = RequestRandomMountain();
			float width = mountain.meshRenderer.bounds.extents.magnitude;
			zPos = Random.Range( mountainZRange.min, mountainZRange.max );
			distanceFromCam = (cam.transform.position.z - zPos).MakePositive();
			xPos += Random.Range( width * 0.75f, width * 1.5f  );
			yPos = floor - ( distanceFromCam * 0.08f );
			Vector3 pos = new Vector3( xPos, yPos, zPos );
			mountain.transform.position = pos;
		}
	}

	void GenerateClouds( float start, float end )
	{
		float xPos = start;
		float yPos;
		float zPos;
		while ( xPos < end )
		{
			xPos += Random.Range( 30f, 60f );
			yPos = Random.Range( cloudYRange.min, cloudYRange.max );
			zPos = Random.Range( cloudZRange.min, cloudZRange.max );
			Vector3 pos = new Vector3( xPos, yPos, zPos );
			SVGRenderer cloud = RequestRandomCloud();
			cloud.transform.position = pos;
		}
	}

	int poolIndex;
	SVGRenderer RequestRandomMountain()
	{
		SVGRenderer mountain = bgObjects[poolIndex];

		int rng = Random.Range( 0, mountainSVGs.Length );

		mountain.vectorGraphics = mountainSVGs[rng];

		float scale;

		if ( rng > 5 )
			scale = Random.Range( mountainScaleRangeTall.min, mountainScaleRangeTall.max );
		else
			scale = Random.Range( mountainScaleRange.min, mountainScaleRange.max );
		
		mountain.transform.localScale = new Vector3( scale, scale, 0f );

		ChangePoolIndex();

		return mountain;
	}

	SVGRenderer RequestRandomCloud()
	{
		SVGRenderer cloud = bgObjects[poolIndex];

		int rng = Random.Range( 0, cloudSVGs.Length );

		cloud.vectorGraphics = cloudSVGs[rng];

		float scale = Random.Range( cloudScaleRange.min, cloudScaleRange.max );

		cloud.transform.localScale = new Vector3( scale, scale, 0f );

		ChangePoolIndex();

		return cloud;
	}

	void ChangePoolIndex()
	{
		poolIndex++;

		if ( poolIndex >= bgObjects.Length - 1 )
			poolIndex = 0;
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
