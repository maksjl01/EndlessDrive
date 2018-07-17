using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColorPalette : MonoBehaviour {

    public Color[] CurrentColorPalette;
    public static ColorPalette instance;

    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentColorPalette = new Color[4];
    }

    public void SetColors(Color col1, Color col2, Color col3, Color col4)
    {
        CurrentColorPalette[0] = col1;
        CurrentColorPalette[1] = col2;
        CurrentColorPalette[2] = col3;
        CurrentColorPalette[3] = col4;
    }

    public Color[] GetColors()
    {
        return CurrentColorPalette;
    }
}
