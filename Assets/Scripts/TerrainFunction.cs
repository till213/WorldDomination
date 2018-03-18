using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFunction {

	public float[,] Elevation { get; set; }
	public Color32[,] TileColors { get; set; }

	float Evaluate (float x, ContourParameter contourParameter) {

		float y = contourParameter.amplitude1 * Mathf.Sin (contourParameter.frequency1 * (x + contourParameter.offset1))
			+ contourParameter.amplitude2 * Mathf.Sin (contourParameter.frequency2 * (x + contourParameter.offset2))
			+ contourParameter.amplitude3 * Mathf.Sin (contourParameter.frequency3 * (x + contourParameter.offset3))
			+ contourParameter.amplitude4 * Mathf.Sin (contourParameter.frequency4 * (x + contourParameter.offset4));

		//y = Mathf.Sin (x);

	    return y;
	}

	public void Create(TerrainParameter terrainParameter) {

		int NofPoints = terrainParameter.nofTiles + 1;

		float[,] contourLinesX;
		float[,] contourLinesZ;

		float[,] elevation = new float[NofPoints, NofPoints];
		Color32[,] tileColors = new Color32[terrainParameter.nofTiles, terrainParameter.nofTiles];

		float x0 = -Mathf.PI;
		float z0 = -Mathf.PI;
		float dx = (2f * Mathf.PI) / terrainParameter.nofTiles;
		float dz = dx;

		int nofContourLines = terrainParameter.nofPatches + 1;

		// Contour lines X
		contourLinesX = new float[nofContourLines, NofPoints];
		for (int p = 0; p < nofContourLines - 1; ++p) {

			ContourParameter contourParameter;

			contourParameter.amplitude1 = Random.value;
			contourParameter.frequency1 = 1;
			contourParameter.offset1 = Random.value;

			contourParameter.amplitude2 = Random.value;
			contourParameter.frequency2 = 2;
			contourParameter.offset2 = Random.value;

			contourParameter.amplitude3 = Random.value;
			contourParameter.frequency3 = 4;
			contourParameter.offset3 = Random.value;

			contourParameter.amplitude4 = Random.value;
			contourParameter.frequency4 = 8;
			contourParameter.offset4 = Random.value;

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

			ContourParameter contourParameter;

			contourParameter.amplitude1 = Random.value;
			contourParameter.frequency1 = 1;
			contourParameter.offset1 = Random.value;

			contourParameter.amplitude2 = Random.value;
			contourParameter.frequency2 = 2;
			contourParameter.offset2 = Random.value;

			contourParameter.amplitude3 = Random.value;
			contourParameter.frequency3 = 4;
			contourParameter.offset3 = Random.value;

			contourParameter.amplitude4 = Random.value;
			contourParameter.frequency4 = 8;
			contourParameter.offset4 = Random.value;

			float z = z0;
			for (int v = 0; v < NofPoints; ++v) {

				float y = Evaluate (z, contourParameter);
				contourLinesZ[o, v] = y;

				z += dz;

			} // z

		}

		// Last contour line identical to first one
		for (int u = 0; u < NofPoints; ++u) {
			contourLinesX [nofContourLines - 1, u] = contourLinesZ [0, u];
		} // z

		// Last contour line identical to first one
		for (int v = 0; v < NofPoints; ++v) {
			contourLinesZ [nofContourLines - 1, v] = contourLinesZ [0, v];
		} // z
			
		int nofTilesPerPatch = terrainParameter.nofTiles / terrainParameter.nofPatches;

		for (int p = 0; p < terrainParameter.nofPatches; ++p) {

			for (int v = p * nofTilesPerPatch; v < p * nofTilesPerPatch + nofTilesPerPatch; ++v) {

				for (int o = 0; o < terrainParameter.nofPatches; ++o) {

					float t = (float)(v % nofTilesPerPatch) / (float)nofTilesPerPatch;

					for (int u = o * nofTilesPerPatch; u < o * nofTilesPerPatch + nofTilesPerPatch; ++u) {
						
						float s = (float)(u % nofTilesPerPatch) / (float)nofTilesPerPatch;
					
						float y = contourLinesX [p, u] * (1.0f - t) + contourLinesX [p + 1, u] * t +
						          contourLinesZ [o, v] * (1.0f - s) + contourLinesZ [o + 1, v] * s;

						elevation [u, v] = y;

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

					}

				}

			}

		}

		// Border lines
		for (int u = 0; u < NofPoints; ++u) {
			float y = elevation [u, 0];
			elevation [u, NofPoints - 1] = y;
			if (u > 0) {
				tileColors [u - 1, terrainParameter.nofTiles - 1] = new Color32 (
					0,
					255,
					255,
					255);
			}
		}
		for (int v = 0; v < NofPoints; ++v) {
			float y = elevation [0, v];
			elevation [NofPoints - 1, v] = y;
			if (v > 0) {
				tileColors [terrainParameter.nofTiles - 1, v - 1] = new Color32 (
					255,
					255,
					0,
					255);
			}
		}
			
		this.Elevation = elevation;
		this.TileColors = tileColors;

	}

}
