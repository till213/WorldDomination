using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFunction
{

	// Palette
	static Color32 seaColorBottom = new Color32(0, 0, 28, 255);
	static Color32 seaColorTop = new Color32(0, 0, 255, 255);

	// As measured from sea level
	const float BeachLevel = 0.15f;

	ContourParameter contourParameter;
	TerrainParameter terrainParameter;
	float[,] contourLinesX;
	float[,] contourLinesZ;

	public float[,] VertexElevations { get; set; }
	public Color32[,] TileColors { get; set; }

	float minElevation, maxElevation;

	float Evaluate (float x)
	{
		float y = contourParameter.weight1 * (contourParameter.amplitude1 * Mathf.Sin (contourParameter.frequency1 * (x + contourParameter.offset1)))
			+ contourParameter.weight2 * (contourParameter.amplitude2 * Mathf.Sin (contourParameter.frequency2 * (x + contourParameter.offset2)))
			+ contourParameter.weight3 * (contourParameter.amplitude3 * Mathf.Sin (contourParameter.frequency3 * (x + contourParameter.offset3)))
			+ contourParameter.weight4 * (contourParameter.amplitude4 * Mathf.Sin (contourParameter.frequency4 * (x + contourParameter.offset4)));

	    return y;
	}
		
	static float CubicInterpolate(
		float y0,float y1,
		float y2,float y3,
		float mu)
	{
		// http://paulbourke.net/miscellaneous/interpolation/
		float a0, a1, a2, a3, mu2;

		mu2 = mu * mu;
//		a0 = y3 - y2 - y0 + y1;
//		a1 = y0 - y1 - a0;
//		a2 = y2 - y0;
//		a3 = y1;

		// Catmull-Rom spline
		a0 = -0.5f*y0 + 1.5f*y1 - 1.5f*y2 + 0.5f*y3;
		a1 = y0 - 2.5f*y1 + 2f*y2 - 0.5f*y3;
		a2 = -0.5f*y0 + 0.5f*y2;
		a3 = y1;

		return (a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
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

	void CreateContours()
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
				contourLinesX [p, u] = Evaluate (x);
				x += dx;
			} // x

		}

		// Contour lines Z
		contourLinesZ = new float [nofContourLines, NofPoints];
		for (int o = 0; o < nofContourLines - 1; ++o) {

			randomiseContour ();

			float z = z0;
			for (int v = 0; v < NofPoints; ++v) {
				contourLinesZ [o, v] = Evaluate (z);
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

	void Elevate()
	{
		int nofPoints = terrainParameter.nofTiles + 1;
		float[,] vertexElevations = new float[nofPoints, nofPoints];
		int nofTilesPerPatch = terrainParameter.nofTiles / terrainParameter.nofPatches;
		int nofContourLines = terrainParameter.nofPatches + 1;

		maxElevation = float.MinValue;
		minElevation = float.MaxValue;
		// Interpolate contour lines across X- and Z-direction (Y is up)
		for (int p = 0; p < terrainParameter.nofPatches; ++p) {
			for (int v = p * nofTilesPerPatch; v < p * nofTilesPerPatch + nofTilesPerPatch; ++v) {
				for (int o = 0; o < terrainParameter.nofPatches; ++o) {					
					float t = (float)(v % nofTilesPerPatch) / (float)nofTilesPerPatch;
					for (int u = o * nofTilesPerPatch; u < o * nofTilesPerPatch + nofTilesPerPatch; ++u) {
						float s = (float)(u % nofTilesPerPatch) / (float)nofTilesPerPatch;
				
						int p1 = ((p - 1) % nofContourLines + nofContourLines) % nofContourLines;
						float y1 = CubicInterpolate (contourLinesX [p1, u],
							           contourLinesX [p, u],
							           contourLinesX [p + 1, u],
							           contourLinesX [(p + 2) % nofContourLines, u],
							           t);
						p1 = ((o - 1) % nofContourLines + nofContourLines) % nofContourLines;
						float y2 = CubicInterpolate (contourLinesZ [p1, v],
							contourLinesX [o, v],
							contourLinesX [o + 1, v],
							contourLinesX [(o + 2) % nofContourLines, v],
							s);
						float y = y1 + y2;
						vertexElevations [u, v] = y;
						if (y > maxElevation) {
							maxElevation = y;
						} else if (y < minElevation) {
							minElevation = y;
						}
					}
				}
			}
		}

		// Border lines
		for (int u = 0; u < nofPoints; ++u) {
			vertexElevations [u, nofPoints - 1] = vertexElevations [u, 0];
		}
		for (int v = 0; v < nofPoints; ++v) {
			vertexElevations [nofPoints - 1, v] = vertexElevations [0, v];
		}
		this.VertexElevations = vertexElevations;
	}

	void Normalise()
	{
		int nofPoints = terrainParameter.nofTiles + 1;

		if (minElevation < -1.0f || maxElevation > 1.0f) {

			float[,] vertexElevations = this.VertexElevations;
			float max = Mathf.Max (Mathf.Abs (minElevation), Mathf.Abs (maxElevation));

			for (int v = 0; v < nofPoints; ++v) {
				for (int u = 0; u < nofPoints; ++u) {
					vertexElevations [u, v] /= max;
				}
			}

			this.VertexElevations = vertexElevations;
		}

	}

    float Elevation (int s, int t)
	{
		float[,] vertexElevations = VertexElevations;
		float sum = 
			vertexElevations [s, t] +
			vertexElevations [(s + 1) % terrainParameter.nofTiles, t] +
			vertexElevations [(s + 1) % terrainParameter.nofTiles, (t + 1) % terrainParameter.nofTiles] +
			vertexElevations [s, (t + 1) % terrainParameter.nofTiles];
		return sum / 4f;
	}

	void Colorise()
	{
		Color32[,] tileColors = new Color32[terrainParameter.nofTiles, terrainParameter.nofTiles];
		float[,] vertexElevations = this.VertexElevations;

		float difference = maxElevation - minElevation;
		for (int t = 0; t < terrainParameter.nofTiles; ++t) {
			for (int s = 0; s < terrainParameter.nofTiles; ++s) {

				if (vertexElevations [s, t] < terrainParameter.seaLevel) {
					// Sea
					float d = ((Elevation (s, t) - minElevation) / difference) / ((terrainParameter.seaLevel + 1.0f) / 2.0f);
					tileColors [s, t] = Color32.Lerp (seaColorBottom, seaColorTop, d);
				} else if (vertexElevations [s, t] < terrainParameter.seaLevel + BeachLevel) {
					// Beach
					tileColors [s, t] = Random.ColorHSV(0.15f, 0.16f, 1.0f, 1.0f, 1.0f, 1.0f);
				} else {
					// Land
					tileColors [s, t] = Random.ColorHSV(0.31f, 0.35f, 1.0f, 1.0f, 0.9f, 1.0f);
				}
					
			}
		}

		this.TileColors = tileColors;
	}

	public void Create(TerrainParameter terrainParameter)
	{
		this.terrainParameter = terrainParameter;
		CreateContours();
		Elevate();
		Normalise();
		Colorise();
	}

}
