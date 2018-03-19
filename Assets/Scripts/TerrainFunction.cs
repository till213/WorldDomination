using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFunction
{
	ContourParameter contourParameter;
	float[,] contourLinesX;
	float[,] contourLinesZ;

	public float[,] Elevation { get; set; }
	public Color32[,] TileColors { get; set; }

	float Evaluate (float x, ContourParameter contourParameter)
	{
		float y = contourParameter.weight1 * (contourParameter.amplitude1 * Mathf.Sin (contourParameter.frequency1 * (x + contourParameter.offset1)))
			+ contourParameter.weight2 * (contourParameter.amplitude2 * Mathf.Sin (contourParameter.frequency2 * (x + contourParameter.offset2)))
			+ contourParameter.weight3 * (contourParameter.amplitude3 * Mathf.Sin (contourParameter.frequency3 * (x + contourParameter.offset3)))
			+ contourParameter.weight4 * (contourParameter.amplitude4 * Mathf.Sin (contourParameter.frequency4 * (x + contourParameter.offset4)));

	    return y;
	}

	void randomiseContour()
	{
		contourParameter.amplitude1 = Random.value;
		contourParameter.frequency1 = 1;
		contourParameter.offset1 = Random.value * Mathf.PI * 2;

		contourParameter.amplitude2 = Random.value;
		contourParameter.frequency2 = 2;
		contourParameter.offset2 = Random.value * Mathf.PI * 2;

		contourParameter.amplitude3 = Random.value;
		contourParameter.frequency3 = 4;
		contourParameter.offset3 = Random.value * Mathf.PI * 2;

		contourParameter.amplitude4 = Random.value;
		contourParameter.frequency4 = 8;
		contourParameter.offset4 = Random.value * Mathf.PI * 2;
	}

	void CreateContours(TerrainParameter terrainParameter)
	{
		int NofPoints = terrainParameter.nofTiles + 1;
		float x0 = -Mathf.PI;
		float z0 = -Mathf.PI;
		float dx = (2f * Mathf.PI) / terrainParameter.nofTiles;
		float dz = dx;

		int nofContourLines = terrainParameter.nofPatches + 1;

		contourParameter.weight1 = terrainParameter.weight1;
		contourParameter.weight2 = terrainParameter.weight2;
		contourParameter.weight3 = terrainParameter.weight3;
		contourParameter.weight4 = terrainParameter.weight4;

		// Contour lines X
		contourLinesX = new float[nofContourLines, NofPoints];
		for (int p = 0; p < nofContourLines - 1; ++p) {

			randomiseContour ();

			float x = x0;
			for (int u = 0; u < NofPoints; ++u) {

				float y = Evaluate (x, contourParameter);
				contourLinesX [p, u] = y;

				x += dx;

			} // x

		}

		// Contour lines Z
		contourLinesZ = new float[nofContourLines, NofPoints];
		for (int o = 0; o < nofContourLines - 1; ++o) {

			randomiseContour ();

			float z = z0;
			for (int v = 0; v < NofPoints; ++v) {

				float y = Evaluate (z, contourParameter);
				contourLinesZ[o, v] = y;

				z += dz;

			} // z

		}

		// Last contour line identical to first one
		for (int u = 0; u < NofPoints; ++u) {
			contourLinesX [nofContourLines - 1, u] = contourLinesX [0, u];
		}

		// Last contour line identical to first one
		for (int v = 0; v < NofPoints; ++v) {
			contourLinesZ [nofContourLines - 1, v] = contourLinesZ [0, v];
		}

	}

	void Elevate(TerrainParameter terrainParameter)
	{
		
		int NofPoints = terrainParameter.nofTiles + 1;

		float[,] elevation = new float[NofPoints, NofPoints];


		int nofTilesPerPatch = terrainParameter.nofTiles / terrainParameter.nofPatches;

		// Interpolate contour lines across X- and Z-direction (Y is up)
		for (int p = 0; p < terrainParameter.nofPatches; ++p) {
			for (int v = p * nofTilesPerPatch; v < p * nofTilesPerPatch + nofTilesPerPatch; ++v) {
				for (int o = 0; o < terrainParameter.nofPatches; ++o) {					
					float t = (float)(v % nofTilesPerPatch) / (float)nofTilesPerPatch;
					for (int u = o * nofTilesPerPatch; u < o * nofTilesPerPatch + nofTilesPerPatch; ++u) {
						float s = (float)(u % nofTilesPerPatch) / (float)nofTilesPerPatch;

						elevation [u, v] = contourLinesX [p, u] * (1.0f - t) + contourLinesX [p + 1, u] * t +
							               contourLinesZ [o, v] * (1.0f - s) + contourLinesZ [o + 1, v] * s;
					}
				}
			}
		}

		// Border lines
		for (int u = 0; u < NofPoints; ++u) {
			elevation [u, NofPoints - 1] = elevation [u, 0];
		}
		for (int v = 0; v < NofPoints; ++v) {
			elevation [NofPoints - 1, v] = elevation [0, v];
		}
		this.Elevation = elevation;
	}

	void Colorise(TerrainParameter terrainParameter)
	{
		Color32[,] tileColors = new Color32[terrainParameter.nofTiles, terrainParameter.nofTiles];
		float[,] elevation = this.Elevation;

		for (int v = 0; v < terrainParameter.nofTiles; ++v) {
			for (int u = 0; u < terrainParameter.nofTiles; ++u) {
				if (elevation [u, v] < terrainParameter.seaLevel) {
					tileColors [u, v] = new Color32 (0, 0, 255, 255);
				} else if (elevation [u, v] < terrainParameter.seaLevel + 0.4f) {
					tileColors [u, v] = new Color32 (255, 255, 0, 255);
				} else {
					tileColors [u, v] = new Color32 (0, 255, 0, 255);
				}
					
			}
		}

		this.TileColors = tileColors;
	}

	public void Create(TerrainParameter terrainParameter)
	{
		CreateContours (terrainParameter);
		Elevate (terrainParameter);
		Colorise (terrainParameter);


	}

}
