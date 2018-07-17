using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Terrain_Flat))]
public class EditorShader : Editor {

	public override void OnInspectorGUI()
	{
		Terrain_Flat terrain = (Terrain_Flat)target;

		if (GUILayout.Button("Update"))
		{
			terrain.Generate();
		}

        if(GUILayout.Button("Keep colors"))
        {
            string[] colors = new string[terrain.BaseColorCount];
            string filePath = Path.Combine(Application.dataPath, "Palettes");

            using (StreamWriter writer = File.AppendText(Path.Combine(filePath, "palettes.txt")))
            {
                writer.WriteLine("new");
                foreach (Color col in terrain.Colors)
                {
                    writer.WriteLine(col.r + " " + col.g + " " + col.b);
                }
                writer.Close();
            }
            Debug.Log(Path.Combine(filePath, "palettes.txt"));
        }
		base.OnInspectorGUI();
	}

}
