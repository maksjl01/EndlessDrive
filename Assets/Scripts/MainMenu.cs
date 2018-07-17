using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    const string PaletteFolder = "Palettes";
    string dataPath;

    public ProceduralTexture procTex;
    [Space]
    [Header("UI")]
    public Image MainMenuBackground;
    Color[] cols;

    [Space]
    public bool UsePalettes;


    Texture2D texture;

    private void Start()
    {
        dataPath = Application.dataPath;

        cols = FindPalette();
        ColorPalette.instance.SetColors(cols[0], cols[1], cols[2], cols[3]);

        if (UsePalettes)
            procTex.MainColor = FindColorFromPalettes();
        texture = new Texture2D(procTex.Width, procTex.Height);
        texture = procTex.GetSquaresTexture();
        MainMenuBackground.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    Color FindColorFromPalettes()
    {
        for (int i = 0; i < cols.Length; i++)
        {
            float rand = UnityEngine.Random.Range(0.0f, 1.0f);

            if (rand < 0.25f)
                return cols[i];
            else if (i == cols.Length - 1) i = 0;
        }
        return Color.white;
    }

    Color[] FindPalette()
    {
        var dir = new DirectoryInfo(Path.Combine(dataPath, PaletteFolder));
        string filePath = dir.GetFiles()[0].FullName;

        string[] lines = File.ReadAllLines(filePath);

        Color[] cols = new Color[4];

        List<string> colorLines = new List<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            if(lines[i] != "new")
            {
                colorLines.Add(lines[i]);
            }
        }

        for (int i = 0; i < colorLines.Count; i++)
        {
            float rand = UnityEngine.Random.Range(0.0f, 1.0f);
            if ((i + 1) % 4 == 1 && rand < 0.1f)
            {
                float[] cols0 = LineToFloats(colorLines[i]);
                float[] cols1 = LineToFloats(colorLines[i + 1]);
                float[] cols2 = LineToFloats(colorLines[i + 2]);
                float[] cols3 = LineToFloats(colorLines[i + 3]);

                cols = GetColorsFromLines(cols0, cols1, cols2, cols3);
                break;
            }
            else if (i == colorLines.Count - 1) i = 0;
        }

        return cols;
    }

    float[] LineToFloats(string line)
    {
        string[] f = line.Split(' ');
        float[] cols = new float[3];

        for (int i = 0; i < f.Length; i++)
        {
            cols[i] = float.Parse(f[i]);
        }
        return cols;
    }

    Color[] GetColorsFromLines(float[] ind0, float[] ind1, float[] ind2, float[] ind3)
    {
        Color[] c = new Color[4];
        float[] ind = new float[12];

        Array.Copy(ind0, ind, ind0.Length);
        Array.Copy(ind1, 0, ind, ind1.Length, ind1.Length);
        Array.Copy(ind2, 0, ind, ind1.Length + ind2.Length, ind2.Length);
        Array.Copy(ind3, 0, ind, ind1.Length + ind2.Length + ind3.Length, ind3.Length);

        int index = 0;

        for (int i = 0; i < c.Length; i++)
        {
            c[i] = new Color(ind[index], ind[index + 1], ind[index + 2]);
            index += 3;
        }
        return c;
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("one");
    }
}
