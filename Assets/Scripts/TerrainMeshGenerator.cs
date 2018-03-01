using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainMeshGenerator : MonoBehaviour {

	[Range(0f, 10f)]
	public float k = 1f;

	public bool developer = false;

	[Range(0f, 50f)]
	public float maxHeight = 10f;

	[Range(-1.0f, 1.0f)]
	public float amplitude1 = 0.25f;
	[Range(0.0f, 10.0f)]
	public float frequency1 = 1f;
	[Range(-3f, 3f)]
	public float offset1 = 0f;

	[Range(-1.0f, 1.0f)]
	public float amplitude2 = 0.25f;
	[Range(0.0f, 10.0f)]
	public float frequency2 = 2f;
	[Range(-3f, 3f)]
	public float offset2 = 0f;

	[Range(-1.0f, 1.0f)]
	public float amplitude3 = 0.25f;
	[Range(0.0f, 10.0f)]
	public float frequency3 = 1f;
	[Range(-3f, 3f)]
	public float offset3 = 0.5f;

	[Range(-1.0f, 1.0f)]
	public float amplitude4 = 0.25f;
	[Range(0.0f, 10.0f)]
	public float frequency4 = 2f;
	[Range(-3f, 3f)]
	public float offset4 = 1f;

	// 511 tiles in each dimension (127 x 127)
	// Note: maximum triangle count: 65'535
	public const int NofTiles = 127;
	public const int NofPoints = NofTiles + 1;
	public const int TileSize = 1;

	Mesh mesh;

	MeshFilter meshFilter;
	MeshCollider meshCollider;

	void UpdateTerrain() {

		mesh = new Mesh();
		Vector3[] vertices = new Vector3[NofTiles * NofTiles * 4];
		Color32[] colors = new Color32[NofTiles * NofTiles * 4];
	    int[] triangles = new int[NofTiles * NofTiles * 6];
		TerrainFunction terrainFunction = new TerrainFunction ();

		TerrainParameter terrainParameter;

		terrainParameter.amplitude1 = amplitude1;
		terrainParameter.amplitude2 = amplitude2;
		terrainParameter.amplitude3 = amplitude3;
		terrainParameter.amplitude4 = amplitude4;

		terrainParameter.frequency1 = frequency1;
		terrainParameter.frequency2 = frequency2;
		terrainParameter.frequency3 = frequency3;
		terrainParameter.frequency4 = frequency4;

		terrainParameter.offset1 = offset1;
		terrainParameter.offset2 = offset2;
		terrainParameter.offset3 = offset3;
		terrainParameter.offset4 = offset4;

		terrainParameter.k = k;
		terrainParameter.nofTiles = NofTiles;


		terrainFunction.Create (terrainParameter);


		float x0 = -(NofTiles * TileSize / 2f);
		float z0 = x0;
		float dx = TileSize;
		float dz = dx;

		float[,] elevation = terrainFunction.Elevation;

		float z = z0;
		for (int v = 0; v < terrainParameter.nofTiles; ++v) {

			float x = x0;
			for (int u = 0; u < terrainParameter.nofTiles; ++u) {

				float y1 = elevation [u,     v    ];
				float y2 = elevation [u + 1, v    ];
				float y3 = elevation [u + 1, v + 1];
				float y4 = elevation [u,     v + 1];

				int vertexBaseAddress = NofTiles * v * 4;
				vertices [vertexBaseAddress + u * 4    ] = new Vector3 (x,      y1, z);      // Bottom left  (0)
				vertices [vertexBaseAddress + u * 4 + 1] = new Vector3 (x + dx, y2, z);      // Bottom right (1)
				vertices [vertexBaseAddress + u * 4 + 2] = new Vector3 (x + dx, y3, z + dz); // Top right    (2)
				vertices [vertexBaseAddress + u * 4 + 3] = new Vector3 (x,      y4, z + dz); // Top left     (3)

				Color32 color = terrainFunction.TileColors [u, v];
				colors [vertexBaseAddress + u * 4    ] = color;
				colors [vertexBaseAddress + u * 4 + 1] = color;
				colors [vertexBaseAddress + u * 4 + 2] = color;
				colors [vertexBaseAddress + u * 4 + 3] = color;

				int triangleBaseAddress = NofTiles * v * 6;
				//  Lower left triangle
				triangles [triangleBaseAddress + u * 6]     = vertexBaseAddress + u * 4;     // (0)
				triangles [triangleBaseAddress + u * 6 + 1] = vertexBaseAddress + u * 4 + 2; // (2)
				triangles [triangleBaseAddress + u * 6 + 2] = vertexBaseAddress + u * 4 + 1; // (1)

				//  Upper right triangle 
				triangles [triangleBaseAddress + u * 6 + 3] = vertexBaseAddress + u * 4 + 2; // (2)
				triangles [triangleBaseAddress + u * 6 + 4] = vertexBaseAddress + u * 4;     // (0)
				triangles [triangleBaseAddress + u * 6 + 5] = vertexBaseAddress + u * 4 + 3; // (3)

				x += dx;


			} // x

			z += dz;

		} // z

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.colors32 = colors;

		// By setting a larger bound than geometrically required we prevent
		// the discarding of triangles out of the view frustum, specifically
		// when the player crosses the northern boundary of the map
		// @todo Fix bounds (calculate them)
		Bounds bounds = new Bounds (Vector2.zero, new Vector3 (100, 200, 200));
		mesh.bounds = bounds;

		mesh.RecalculateNormals ();

	}

	// Use this for initialization
	void Start () {

		meshFilter = GetComponent<MeshFilter>();
		UpdateTerrain ();
		meshFilter.mesh = mesh;

		// Add collider
		meshCollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		meshCollider.sharedMesh = mesh;
		
	}
	
	// Update is called once per frame
	void Update () {

		if (developer) {
			UpdateTerrain ();
			meshFilter.mesh = mesh;
			meshCollider.sharedMesh = mesh;
		}

	}
}
