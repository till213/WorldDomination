public struct TerrainParameter
{
	public int nofTiles; // Must be a multiple of 'nofPatches'
	public int nofPatches;

	public float weight1;
	public float weight2;
	public float weight3;
	public float weight4;

	// [-1.0, 1.0]
	public float seaLevel;
}
