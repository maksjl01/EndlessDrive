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
            ColorPalette c = CreateInstance<ColorPalette>();
            System.Random random = new System.Random(Random.Range(0,100));
            
            int num1 = random.Next();

            string path = string.Format("Assets/Palettes/{0}.asset", num1);

            AssetDatabase.CreateAsset(c, path);
            AssetDatabase.SaveAssets();

            ColorPalette inst = (ColorPalette)AssetDatabase.LoadAssetAtPath(path, typeof(ColorPalette));
            inst.SetColors(terrain.Colors[0], terrain.Colors[1], terrain.Colors[2], terrain.Colors[3]);
        }
		base.OnInspectorGUI();
	}

}


//string[] colors = new string[terrain.BaseColorCount];
//string filePath = Path.Combine(Application.dataPath, "Palettes");

//using (StreamWriter writer = File.AppendText(Path.Combine(filePath, "palettes.txt")))
//{
//    writer.WriteLine("new");
//    foreach (Color col in terrain.Colors)
//    {
//        writer.WriteLine(col.r + " " + col.g + " " + col.b);
//    }
//    writer.Close();
//}
//Debug.Log(Path.Combine(filePath, "palettes.txt"));
