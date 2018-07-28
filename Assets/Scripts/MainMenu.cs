using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    public ProceduralTexture procTex;
    [Space]
    [Header("UI")]
    public Image MainMenuBackground;
    public GameObject ShopPanel;
    public GameObject OptionsPanel;
    [Space]
    public Toggle ControlButtonsToggle;
    ColorPalette cp;
    [Space]
    public GameObject SplashScreen;

    [Space]
    public bool UsePalettes;

    Camera cam;
    Texture2D texture;

    ColorPalettePersistance colorPalettePersistance;
    MusicPlayer musicPlayer;

    private void Awake()
    {
        colorPalettePersistance = FindObjectOfType<ColorPalettePersistance>();
        musicPlayer = FindObjectOfType<MusicPlayer>();
    }

    private void Start()
    {
        cam = FindObjectOfType<Camera>();

        cp = colorPalettePersistance.FindRandomPalette();
        colorPalettePersistance.SetCurrentColorPalette(cp);

        if (UsePalettes)
        {
            Color col;
            if (cp.Colors.Count > 3)
                col = colorPalettePersistance.RandomColorFromCurrentColorPalette();
            else
                col = Color.cyan;
            procTex.MainColor = col;
            colorPalettePersistance.MenuColor = col;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = col;
        }
        texture = new Texture2D(procTex.Width, procTex.Height);
        texture = procTex.GetSquaresTexture(SplashScreen);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        MainMenuBackground.sprite = sprite;

        ShopPanel.SetActive(false);
        OptionsPanel.SetActive(false);
    }


    public void SetControlButtons()
    {
        if (ControlButtonsToggle.isOn)
        {
            PlayerPrefs.SetInt("ControlSettings", 1);
        }
        else
        {
            PlayerPrefs.SetInt("ControlSettings", 0);
        }
    }

    public void ShopView()
    {
        ShopPanel.SetActive(!ShopPanel.activeSelf);
    }

    public void OptionsView()
    {
        OptionsPanel.SetActive(!OptionsPanel.activeSelf);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("one");
    }

    public void ToggleMusic(bool on)
    {
        musicPlayer.ToggleOn(on);
    }

    public void ChangeMusicVolume(float _volume)
    {
        musicPlayer.ChangeVolume(_volume);
    }
}



//Color FindColorFromPalettes()
//{
//    for (int i = 0; i < cols.Length; i++)
//    {
//        float rand = UnityEngine.Random.Range(0.0f, 1.0f);

//        if (rand < 0.25f)
//            return cols[i];
//        else if (i == cols.Length - 1) i = 0;
//    }
//    return Color.white;
//}

//var dir = new DirectoryInfo(Path.Combine(dataPath, PaletteFolder));
//string filePath = dir.GetFiles()[0].FullName;

//string[] lines = File.ReadAllLines(filePath);

//Color[] cols = new Color[4];

//List<string> colorLines = new List<string>();

//for (int i = 0; i < lines.Length; i++)
//{
//    if(lines[i] != "new")
//    {
//        colorLines.Add(lines[i]);
//    }
//}

//float[] LineToFloats(string line)
//{
//    string[] f = line.Split(' ');
//    float[] cols = new float[3];

//    for (int i = 0; i < f.Length; i++)
//    {
//        cols[i] = float.Parse(f[i]);
//    }
//    return cols;
//}

//for (int i = 0; i < colorLines.Count; i++)
//{
//    float rand = UnityEngine.Random.Range(0.0f, 1.0f);
//    if ((i + 1) % 4 == 1 && rand < 0.1f)
//    {
//        float[] cols0 = LineToFloats(colorLines[i]);
//        float[] cols1 = LineToFloats(colorLines[i + 1]);
//        float[] cols2 = LineToFloats(colorLines[i + 2]);
//        float[] cols3 = LineToFloats(colorLines[i + 3]);

//        cols = GetColorsFromLines(cols0, cols1, cols2, cols3);
//        break;
//    }
//    else if (i == colorLines.Count - 1) i = 0;
//}
//return cols;


//Color[] GetColorsFromLines(float[] ind0, float[] ind1, float[] ind2, float[] ind3)
//{
//    Color[] c = new Color[4];
//    float[] ind = new float[12];

//    Array.Copy(ind0, ind, ind0.Length);
//    Array.Copy(ind1, 0, ind, ind1.Length, ind1.Length);
//    Array.Copy(ind2, 0, ind, ind1.Length + ind2.Length, ind2.Length);
//    Array.Copy(ind3, 0, ind, ind1.Length + ind2.Length + ind3.Length, ind3.Length);

//    int index = 0;

//    for (int i = 0; i < c.Length; i++)
//    {
//        c[i] = new Color(ind[index], ind[index + 1], ind[index + 2]);
//        index += 3;
//    }
//    return c;
//}