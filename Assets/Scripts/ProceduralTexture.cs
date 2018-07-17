using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTexture : MonoBehaviour {

    public Color MainColor;
    [Range(0, 1)]
    public float Chance;
    public int Width;
    public int Height;
    public int MaxSize;

    Texture2D tex;

    float[,] colors;

    void Setup()
    {
        tex = new Texture2D(Width, Height);
        colors = new float[Width, Height];
    }

    void CalculateSquares(bool forwards)
    {
        int w = (forwards) ? Width : -1;
        int h = (forwards) ? Height : -1;

        for (int x = (forwards) ? 0 : Width-1; x < w; x++)
        {
            for (int y = (forwards) ? 0 : Height-1; y < h; y++)
            {
                colors[x, y] = 0.5f;
                float random = Random.Range(0.0f, 1.0f);
                if(random < Chance)
                {
                    int size = Random.Range(0, MaxSize);
                    float r = Random.Range(0.0f, 1.0f);

                    for (int m = -size/2; m < size/2; m++)
                    {
                        for (int n = -size/2; n < size/2; n++)
                        {
                            if(x+m < Width && x+m > 0 && y+n < Height && y+n > 0)
                            {
                                colors[x + m, y + n] = r;
                            }
                        }
                    }
                }
            }
        }


        for (int i = 0; i < 6; i++)
        {
            int point = Random.Range(0, Width);
            int pointh = Random.Range(0, Height);

            int size = Random.Range(0, MaxSize);
            float col = Random.Range(0.0f, 1.0f);

            for (int m = -size / 2; m < size / 2; m++)
            {
                for (int n = -size / 2; n < size / 2; n++)
                {
                    if (point + m < Width && point + m > 0 && pointh + n < Height && pointh + n > 0)
                    {
                        colors[point + m, pointh + n] = col;
                    }
                }
            }
        }
    }

    void ApplyColor()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float colVal = colors[x, y];
                tex.SetPixel(x, y, new Color(colVal, colVal, colVal) * MainColor);
            }
        }
        tex.Apply();
        tex.filterMode = FilterMode.Point;
    }

    public void GenerateSquares()
    {
        Setup();
        CalculateSquares(true);
        ApplyColor();
    }

    public Texture2D GetSquaresTexture()
    {
        Setup();
        CalculateSquares(true);
        CalculateSquares(false);
        ApplyColor();
        return tex;
    }
}
