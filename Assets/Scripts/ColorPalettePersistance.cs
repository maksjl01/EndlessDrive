using System.Collections.Generic;
using UnityEngine;

public class ColorPalettePersistance : MonoBehaviour{

    public List<ColorPalette> ColorPalettes = new List<ColorPalette>();
    public Color MenuColor;

    public ColorPalette CurrentColorPalette;

    public static ColorPalettePersistance instance;

    private void Start()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

    } 

    public ColorPalette FindRandomPalette()
    {
        ColorPalette c = ScriptableObject.CreateInstance<ColorPalette>();
        for (int i = 0; i < ColorPalettes.Count; i++)
        {
            if(Random.Range(0.0f,1.0f) < 0.1f)
            {
                c = ColorPalettes[i];
                break;
            }
            if (i == ColorPalettes.Count - 1) i = 0;
        }
        return c;
    }

    public void SetCurrentColorPalette(ColorPalette c)
    {
        CurrentColorPalette = c;
    }

    public Color RandomColorFromCurrentColorPalette()
    {
        return CurrentColorPalette.GetRandomColor();
    }

}
