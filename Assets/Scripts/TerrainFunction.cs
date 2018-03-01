using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFunction {

	public float[,] Elevation { get; set; }
	public Color32[,] TileColors { get; set; }

	[Range(0f, 10f)]
	public float k = 1f;

	[Range(0f, 50f)]
	public float maxHeight = 10f;

	[Range(-1.0f, 1.0f)]
	public float amplitude1 = 0.25f;
	[Range(0.0f, 10.0f)]
	public float frequency1 = 1f;
	[Range(-3f, 3f)]
	public float offset1 = 0f;
	float offset1Increment;

	[Range(-1.0f, 1.0f)]
	public float amplitude2 = 0.25f;
	float amplitude2Increment;
	[Range(0.0f, 10.0f)]
	public float frequency2 = 2f;
	[Range(-3f, 3f)]
	public float offset2 = 0f;
	float offset2Increment;

	[Range(-1.0f, 1.0f)]
	public float amplitude3 = 0.25f;
	[Range(0.0f, 10.0f)]
	public float frequency3 = 1f;
	[Range(-3f, 3f)]
	public float offset3 = 0.5f;
	float offset3Increment;

	[Range(-1.0f, 1.0f)]
	public float amplitude4 = 0.25f;
	[Range(0.0f, 10.0f)]
	public float frequency4 = 2f;
	[Range(-3f, 3f)]
	public float offset4 = 1f;
	float offset4Increment;

	float Evaluate (float x,TerrainParameter terrainParameter) {
		float y = terrainParameter.amplitude1 * Mathf.Sin (terrainParameter.frequency1 * (terrainParameter.k * x + terrainParameter.offset1 + offset1Increment))
			+ (terrainParameter.amplitude2 + amplitude2Increment) * Mathf.Sin (terrainParameter.frequency2 * (terrainParameter.k * x + terrainParameter.offset2 + offset2Increment))
			+ terrainParameter.amplitude3 * Mathf.Sin (terrainParameter.frequency3 * (terrainParameter.k * x + terrainParameter.offset3 + offset3Increment))
			+ terrainParameter.amplitude4 * Mathf.Sin (terrainParameter.frequency4 * (terrainParameter.k * x + terrainParameter.offset4 + offset4Increment));
		return y;
	}

	public void Create(TerrainParameter terrainParameter) {

		float[,] elevation = new float[terrainParameter.nofTiles + 1, terrainParameter.nofTiles + 1];
		Color32[,] tileColors = new Color32[terrainParameter.nofTiles, terrainParameter.nofTiles];

		float x0 = -Mathf.PI;
		float z0 = -Mathf.PI;
		float dx = (2f * Mathf.PI) / terrainParameter.nofTiles;
		float dz = dx;

		float z = z0;
		for (int v = 0; v < terrainParameter.nofTiles + 1; ++v) {

			float x = x0;

			offset1Increment = Mathf.Sin(z);
			offset2Increment = Mathf.Sin(z);
			offset3Increment = Mathf.Cos(z);
			offset4Increment = Mathf.Sin(z);
			amplitude2Increment = Mathf.Cos(z);

			for (int u = 0; u < terrainParameter.nofTiles + 1; ++u) {

				float y = Evaluate(x, terrainParameter);
				elevation [u, v] = y;

				x += dx;
				if (u > 0 && v > 0) {

					if (y < -0.2) {
						tileColors [u - 1, v - 1] = new Color32 (
							0,
							0,
							(byte)((y + 1f) / 0.8f * 255f),
							255);
					} else if (y < 0.2) {
						
						tileColors [u - 1, v - 1] = new Color32 (
							255,
							255,
							0,
							255);
					} else {
						tileColors [u - 1, v - 1] = new Color32 (
							0,
							(byte)((y -0.2f) / 0.8f * 100 + 155),
							0,
							255);
					}

				}

			} // x

			z += dz;

		} // z

		this.Elevation = elevation;
		this.TileColors = tileColors;

	}

}
