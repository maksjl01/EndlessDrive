using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorPalette : ScriptableObject {

    public List<Color> Colors = new List<Color>();

    private Color AmbientColor;

    public ColorPalette(Color c0, Color c1, Color c2, Color c3)
    {
        Colors.Add(c0);
        Colors.Add(c1);
        Colors.Add(c2);
        Colors.Add(c3);
    }

    public ColorPalette (Color all)
    {
        for (int i = 0; i < 4; i++)
        {
            Colors.Add(all);
        }
    }

    public Color[] GetColors()
    { 
        return Colors.ToArray();
    }

    public void SetRandom()
    {
        Colors = new List<Color>(4);
        Colors.Add(Color.blue);
        Colors.Add(Color.cyan);
        Colors.Add(Color.green);
        Colors.Add(Color.grey);
    }

    public void SetColors(Color c0, Color c1, Color c2, Color c3)
    {
        Colors = new List<Color>(4);
        if (Colors.Count < 4)
        {
            Colors.Add(c0);
            Colors.Add(c1);
            Colors.Add(c2);
            Colors.Add(c3);
        }
        else
        {
            Colors[0] = c0;
            Colors[1] = c1;
            Colors[2] = c2;
            Colors[3] = c3;
        }
    }

    public Color GetRandomColor()
    {
        int r = Random.Range(0, 4);
        return Colors[r];
    }
}
