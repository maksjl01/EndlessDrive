using System.Collections.Generic;
using UnityEngine;

public class Terrain_Flat : MonoBehaviour {

	[Header("TERRAIN")]
	public GameObject terrain;
	public bool multiMesh;
    public bool setForEndless;
	public Material material;
	public bool autoUpdate;
	MeshFilter meshFilter;
	MeshFilter[] meshFilters;
	List<Vector3> vertices;

	[Space]

	[Header("APPEARANCE")]
	public Texture texture;
	[Range(0,1)]
	public float LightAttenuation;
	[Range(0,1)]
	public float Randomness;
	[Range(0,2.999f)]
	public float Shininess;
	public float SpecularMultiplier;
	public Color color;
	public Color SpecularColor;
	public int highlightType;
	[Space]

	[Range(-5, 1)]
	public float ShadowDarkness;
	public Color ShadowColor;
	[Range(0,10)]
	public float ShadowColorMultiplier;
	[Tooltip("1 for highlight in front, 0 for no highlight, -1 for highlight behind")]

	float minHeight;
	float maxHeight;
	public float yOffset;

	[Space]

	[Header("HEIGHT AND BLEND")]
	public int BaseColorCount;
	public Color[] Colors;
	[Range(0,1)]
	public float[] Heights;
	[Range(0,1)]
	public float[] BlendLevels;

    bool set;
    public bool randomOnStart;

    public static Terrain_Flat instance;

    private void Start()
    {
        instance = this;
        if (randomOnStart)
        {
            for (int i = 0; i < BaseColorCount; i++)
            {
                Colors[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

                float heightval;
                if (i == 0)
                    heightval = Random.Range(0,0.05f);
                else
                    heightval = Random.Range(0f, (float)1 / BaseColorCount) + ((float)i / BaseColorCount)/1.5f;

                Heights[i] = heightval;
                BlendLevels[i] = Random.Range(0.1f, 0.7f);
            }
        }
    }

    void Update()
    {
        if (setForEndless && !set)
        {
            EndlessSetup();
            SetMaterial();
            set = true;
        }
    }

    void EndlessSetup()
    {
        minHeight = FindObjectOfType<MapGenerator>().terrainData.minHeight;
        maxHeight = FindObjectOfType<MapGenerator>().terrainData.maxHeight;
    }

    void Setup(){
		if (multiMesh) {
			meshFilters = new MeshFilter[terrain.transform.childCount];
			vertices = new List<Vector3>(terrain.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.vertices.Length * terrain.transform.childCount);
			float index = 0;
			for (int i = 0; i < meshFilters.Length; i++) {
				meshFilters [i] = terrain.transform.GetChild (i).GetComponent<MeshFilter> ();
				vertices.AddRange(terrain.transform.GetChild (i).GetComponent<MeshFilter> ().sharedMesh.vertices); 
				index += meshFilters [i].sharedMesh.vertices.Length;
			}
		} else {
			meshFilter = terrain.GetComponent<MeshFilter> ();
		}
		SetMaterial();
	}

	public void Generate(){
        if (setForEndless)
        {
            EndlessSetup();
            SetMaterial();
        }
        else
        {
		    Setup ();
		    SetMaterial ();
        }
	}

	void FindHeights()
	{
		minHeight = float.MaxValue;
		maxHeight = float.MinValue;
		for (int i = 0; i < vertices.Count; i++)
		{ 
			Vector3 vert = terrain.transform.TransformPoint(vertices[i]);
			if (vert.y < minHeight)
				minHeight = vert.y;
			if (vert.y > maxHeight)
				maxHeight = vert.y;
		}
	} 

	public void SetMaterial()
	{
        if (!setForEndless)
        {
		    FindHeights();
		    if (multiMesh) {
			    for (int i = 0; i < meshFilters.Length; i++) {
				    meshFilters [i].gameObject.GetComponent<MeshRenderer> ().sharedMaterial = material;
			    }
		    } else {
			    meshFilter.GetComponent<MeshRenderer> ().sharedMaterial = material;
		    }
        }

		if(material != null)
		{
			material.SetFloat("baseColorCount", BaseColorCount);
			material.SetColorArray("baseColors", Colors);
			material.SetFloatArray("baseStartHeights", Heights);
			material.SetFloatArray("baseBlends", BlendLevels);
			material.SetFloat("yOffset", yOffset);
			material.SetFloat("minHeight", minHeight);
			material.SetFloat("maxHeight", maxHeight);
			material.SetFloat("randomness", Randomness);
			material.SetTexture("tex", texture);
			material.SetFloat("shininess", 3-Shininess);
			material.SetColor("color", color);
			material.SetColor("SpecularColor", SpecularColor);
			material.SetFloat("attenuation", LightAttenuation);
			material.SetFloat("highlightType", highlightType);
			material.SetFloat("SpecularMultiplier", SpecularMultiplier);
			material.SetFloat("ShadowDarkness", ShadowDarkness);
			material.SetColor("ShadowColor", ShadowColor);
			material.SetFloat("ShadowColorMultiplier", ShadowColorMultiplier);
		}
	}

	private void OnValidate()
	{
		if(autoUpdate)
			SetMaterial();
	}

}
