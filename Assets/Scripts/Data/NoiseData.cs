using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatableData {

	
	public Noise.NormalizeMode normalizeMode;

	public float noiseScale;

	public int octaves;
    public bool randomOctave;
	[Range(0,1)]
	public float persistance;
    public bool randomPersistance;
	public float lacunarity;
    public bool randomLacunarity;

	public int seed;
    public bool randomSeed;

	public Vector2 offset;

    private void OnEnable()
    {
        if (randomSeed)
            seed = Random.Range(0, 10000);
        if (randomPersistance)
            persistance = Random.Range(0.38f, 0.6f);
        if (randomLacunarity)
            lacunarity = Random.Range(1.8f, 2.13f);
        if (randomOctave)
            octaves = Random.Range(4, 6);
    }

#if UNITY_EDITOR

    protected override void OnValidate()
	{
		if (lacunarity < 1)
		{
			lacunarity = 1;
		}
		if (octaves < 0)
		{
			octaves = 0;
		}

		base.OnValidate();
	}

	#endif
}
