using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using TMPro;

public class GameManager : MonoBehaviour {

    public static bool playerDead;
    public static float playerHealth;

    public Meteors meteors;
    public EndlessTerrain endlessTerrain;
    [Space]
    GameObject player;
    Controller_Rover playerRover;
    public GameObject cam;
    public Transform cameraStartPos;
    public Transform cameraDeadScreenPos;
    [Space]

    private Player playerComponent;
    public CarSelect[] carSelects;
    public float carDistance = 20;
    public float lerpTime = 2;
    GameObject[] cars;
    GameObject[] basePlates;
    public Transform CarStartPos;
    public Button selectButton;
    public Button buyButton;
    public GameObject coinsOnMenu;
    TextMeshProUGUI coinsOnMenuText;

    public static int Coins { get; private set; }

    private int currentCar;
    private int carAvailability;
    private int controlSettings;
    private float highScore;

    public static GameManager instance;

    [Space]

    [Header("UI")]
    public GameObject controlPanel;
    public Canvas menuCanvas;
    [Space]
    public GameObject ScoreGameObject;
    TextMeshProUGUI scoreText;
    [Space]
    public Slider healthSlider;
    public Sprite[] AnimatedHealthImages;
    public Image HeartImage;
    [Space]
    public GameObject CoinGameObject;
    public Image CoinImage;
    TextMeshProUGUI coinText;
    [Space]
    public GameObject LoadingPanelGameObject;
    public Slider LoadingSlider;
    public GameObject LoadingTextGameObject;
    public Image LoadingBackground;
    public GameObject DeadScreenGameObject;
    public GameObject PauseScreenPanel;
    public GameObject PriceGameObject;
    [Space]
    public Image[] controlButtonImages;
    public GameObject HighScoreGameObject;
    public GameObject ScoreEndScreenGameObject;

    [Header("Cameras")]
    public Camera UICamera;

    private Vector3 basePlateOffset;
    GameObject parent;

    private void Start()
    {
        basePlateOffset = (Vector3.up * -0.75f) - (Vector3.forward * 1.25f);

        QualitySettings.shadowDistance = 12;

        instance = this;

        if (Time.timeScale != 1) Time.timeScale = 1;

#if UNITY_EDITOR
        EditorApplication.ExecuteMenuItem("Edit/Graphics Emulation/No Emulation");
#endif
        GetKeys();
        InstantiateCars();

        playerComponent = player.GetComponent<Player>();
        playerRover = player.GetComponent<Controller_Rover>();
        meteors.enabled = playerComponent.enabled = false;

        SetupUI();

        cam.GetComponent<CameraControl>().enabled = false;
        cam.GetComponent<CameraControl>().target = player;
        cam.transform.position = cameraStartPos.position;

        meteors.player = player;
        endlessTerrain.viewer = player.transform;

        player.tag = "Car";

        playerDead = false;
    }

    void SetupUI()
    {
        if (PlayerPrefs.HasKey("Coins"))
        {
            Coins = PlayerPrefs.GetInt("Coins");
        }
        else
        {
            Coins = 0;
            PlayerPrefs.SetInt("Coins", Coins);
        }

        if(controlPanel.activeSelf)
            controlPanel.SetActive(false);
        PauseScreenPanel.SetActive(false);
        LoadingPanelGameObject.SetActive(true);
        DeadScreenGameObject.SetActive(false);
        UICamera.enabled = false;

        LoadingSlider.value = 0;
        LoadingSlider.maxValue = endlessTerrain.chunkInviewDST;

        if (ColorPalettePersistance.instance != null)
            LoadingBackground.color = ColorPalettePersistance.instance.MenuColor;

        scoreText = ScoreGameObject.GetComponent<TextMeshProUGUI>();
        coinText = CoinGameObject.GetComponent<TextMeshProUGUI>();
        coinsOnMenuText = coinsOnMenu.GetComponent<TextMeshProUGUI>();

        PriceGameObject.GetComponent<TextMeshProUGUI>().text = "";

        playerHealth = playerComponent.StartingHealth;
        healthSlider.maxValue = playerComponent.MaxHealth;
        coinText.text = coinsOnMenuText.text = Coins.ToString();
        scoreText.text = "0";

        coinText.alpha = 0;
        CoinImage.enabled = false;

        if(controlSettings == 0)
        {
            for (int i = 0; i < controlButtonImages.Length; i++)
            {
                controlButtonImages[i].color = new Color(0, 0, 0, 0);
            }
        }
        else if(controlSettings == 1)
        {
            for (int i = 0; i < controlButtonImages.Length; i++)
            {
                controlButtonImages[i].color = new Color(1, 1, 1, 0.1f);
                if(i > 1)
                    controlButtonImages[i].preserveAspect = true;
            }
        }
    }

    void InstantiateCars()
    {
        cars = new GameObject[carSelects.Length];
        basePlates = new GameObject[carSelects.Length];
        parent = new GameObject("Car Select");
        
        for (int i = 0; i < cars.Length; i++)
        {
            int index = (i+currentCar) % cars.Length;
            cars[index] = Instantiate(carSelects[index].car, CarStartPos.position + cam.transform.right * 25 * (i), Quaternion.identity, parent.transform); 
            cars[index].GetComponent<Rigidbody>().isKinematic = true;

            basePlates[index] = Instantiate(carSelects[index].basePlate, CarStartPos.position + basePlateOffset + cam.transform.right * 25 * (i), Quaternion.identity, parent.transform);

            cars[index].transform.parent = basePlates[index].transform;
        } 

        player = cars[currentCar];

        selectButton.interactable = false;
        buyButton.interactable = false;
    }

    void ResetupCars()
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            RecursiveSetactive(parent.transform);
            if(parent.transform.GetChild(i).transform.childCount == 0)
            {
                player.transform.parent = parent.transform.GetChild(i).transform;
            }
        }
    }

    void RecursiveSetactive(Transform parent)
    {
        parent.gameObject.SetActive(true);
        foreach (Transform child in parent)
        {
            RecursiveSetactive(child);
        }
    }

    void GetKeys()
    {
        if (PlayerPrefs.HasKey("CarAvailability"))
        {
            carAvailability = PlayerPrefs.GetInt("CarAvailability");
        }
        else
        {
            carAvailability = 1;
            PlayerPrefs.SetInt("CarAvailability", 1);
        }

        if (PlayerPrefs.HasKey("Coins"))
        {
            Coins = PlayerPrefs.GetInt("Coins");
        }
        else
        {
            Coins = 0;
            PlayerPrefs.SetInt("Coins", 0);
        }

        if (PlayerPrefs.HasKey("Car"))
        {
            currentCar = PlayerPrefs.GetInt("Car");
        }
        else
        {
            PlayerPrefs.SetInt("Car", 0);
            currentCar = 0;
        }

        if (PlayerPrefs.HasKey("ControlSettings"))
        {
            controlSettings = PlayerPrefs.GetInt("ControlSettings");
        }
        else
        {
            PlayerPrefs.SetInt("ControlSettings", 0);
            controlSettings = 0;
        }

        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetFloat("HighScore");
        }
        else
        {
            PlayerPrefs.SetFloat("HighScore", 0);
            highScore = 0;
        }
    }

    public void ReadyScene()
    {
        LoadingPanelGameObject.SetActive(false);
        UICamera.enabled = true;
    }

    public void StartGame()
    {
        playerComponent = player.GetComponent<Player>();
        playerRover = player.GetComponent<Controller_Rover>();
        QualitySettings.shadowDistance = 86;

        for (int i = 0; i < cars.Length; i++)
        {
            if(cars[i] == player)
            {
                basePlates[i].transform.GetChild(0).parent = null;
                basePlates[i].gameObject.SetActive(false);
            }
            else
            {
                basePlates[i].gameObject.SetActive(false);
                cars[i].gameObject.SetActive(false);
            }
        }

        menuCanvas.enabled = false;
        UICamera.gameObject.SetActive(false);

        player.GetComponent<Rigidbody>().isKinematic = false;
        playerComponent.enabled = meteors.enabled = playerRover.enabled = true;
        healthSlider.value = playerComponent.StartingHealth;

        endlessTerrain.viewer = player.transform;

        cam.GetComponent<CameraControl>().enabled = true;
        cam.GetComponent<CameraControl>().target = player;

        meteors.player = player;

        controlPanel.SetActive(true);
        DeadScreenGameObject.SetActive(false);
    }

    public void ChangeCar()
    {
        UICamera.enabled = true;
        DeadScreenGameObject.SetActive(false);
        controlPanel.SetActive(false);
        menuCanvas.enabled = true;

        Destroy(player);
        Destroy(parent);
        InstantiateCars();

        QualitySettings.shadowDistance = 12;

        meteors.player = player;
        endlessTerrain.viewer = player.transform;
        player.tag = "Car";

        cam.transform.position = cameraStartPos.position;
        cam.transform.rotation = cameraStartPos.rotation;

        playerComponent.enabled = meteors.enabled = false;
        cam.GetComponent<CameraControl>().enabled = false;
    }

    public void RestartGame()
    {
        player.transform.position = CarStartPos.position;
        playerComponent.enabled = meteors.enabled = playerRover.enabled = true;
        healthSlider.value = playerComponent.StartingHealth;
        playerComponent.Dead = false;
        playerComponent.RestartGame();

        UICamera.gameObject.SetActive(false);

        controlPanel.SetActive(true);

        DeadScreenGameObject.SetActive(false);
        cam.GetComponent<CameraControl>().enabled = true;

        Controller_Rover.startScoring = false;
    }

    public void TogglePause()
    {
        Time.timeScale = (Time.timeScale == 1) ? 0 : 1;
        if (Time.timeScale == 0)
            PauseScreenPanel.SetActive(true);
        else
            PauseScreenPanel.SetActive(false);
    }

    public void HomeScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void UpdateHealth(float _health)
    {
        healthSlider.value = _health;
        float percent = healthSlider.value / healthSlider.maxValue;
        Color lerped = Color.Lerp(new Color(0.2f, 1.0f, 0), new Color(1.0f, 0.27f, 0), 1 - percent);
        healthSlider.fillRect.GetComponent<Image>().color = lerped;

        float no = percent * 10;
        no = Mathf.Round(no);
        int index = 10 - (int)no;
        if (index == 10)
            index = 9;
        HeartImage.sprite = AnimatedHealthImages[index];
    }

    public void UpdateScore(float _score)
    {
        scoreText.text = _score.ToString("f1");
    }

    public void UpdateCoins(int add)
    {
        Coins += add;
        coinText.text = Coins.ToString();
        StartCoroutine(FlashCoins());
    }

    public void UpdateLoader()
    {
        LoadingSlider.value += 1;
        LoadingTextGameObject.GetComponent<TextMeshProUGUI>().text = (LoadingSlider.value/LoadingSlider.maxValue * 100).ToString("f0") + "%";
    }

    IEnumerator FlashCoins()
    {
        coinText.alpha = 1;
        CoinImage.enabled = true;
        yield return new WaitForSeconds(2.5f);
        coinText.alpha = 0;
        CoinImage.enabled = false;
    }

    public IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1.5f);
        PlayerPrefs.SetInt("Coins", Coins);
        playerRover.enabled = false;
        cam.GetComponent<CameraControl>().enabled = false;
        meteors.enabled = false;
        controlPanel.SetActive(false);
        meteors.StopAllCoroutines();

        HighScoreGameObject.GetComponent<TextMeshProUGUI>().text = highScore.ToString("f1");

        UICamera.gameObject.SetActive(true);

        StartCoroutine(LerpCameraToEndScreenPos(2.5f, cam.transform.position, cam.transform.rotation));
        p_t = 0;
    }

    public void CheckHighScore(float _score)
    {
        if(_score > highScore)
        {
            highScore = _score;
            PlayerPrefs.SetFloat("HighScore", _score);
        }
        ScoreEndScreenGameObject.GetComponent<TextMeshProUGUI>().text = _score.ToString("f1");
    }

    private float p_t;
    IEnumerator LerpCameraToEndScreenPos(float lerpTime, Vector3 camStartPos, Quaternion camStartRotation)
    {
        yield return new WaitForSeconds(1.5f);
        while(cam.transform.position != cameraDeadScreenPos.position && cam.transform.rotation != cameraDeadScreenPos.rotation)
        {
            p_t += Time.deltaTime;
            float t = p_t / lerpTime;
            t = t * t * (3 - 2 * t);
            cam.transform.position = Vector3.Lerp(camStartPos, cameraDeadScreenPos.position, t);
            cam.transform.rotation = Quaternion.Lerp(camStartRotation, cameraDeadScreenPos.rotation, t);
            yield return null;
        }
        DeadScreenGameObject.SetActive(true);
    }

    public void BuyCar()
    {
        if (!((carAvailability & 1 << currentCar) == 1 << currentCar))
        {
            if (Coins > carSelects[currentCar].price)
            {
                carAvailability += (int)Mathf.Pow(2, currentCar);
                buyButton.interactable = false;
                selectButton.interactable = false;
                player = cars[currentCar];
                Coins -= carSelects[currentCar].price;
                coinsOnMenuText.text = Coins.ToString();

                SetKeys();
            }
        }
    }

    void SetKeys()
    {
        PlayerPrefs.SetInt("CarAvailability", carAvailability);
        PlayerPrefs.SetInt("Car", currentCar);
        PlayerPrefs.SetInt("Coins", Coins);
    }

    public void ChooseCar()
    {
        if((carAvailability & 1 << currentCar) == 1 << currentCar)
        {
            player = cars[currentCar];
            selectButton.interactable = false;
            PlayerPrefs.SetInt("Car", currentCar);
        }
    }

    public void SwapCar(SelectCar.swipes swipe)
    {
        if (!lerping)
        {
            if(swipe == SelectCar.swipes.left)
            {
                SetCar(1);
                CarTransition(1);
            }
            else
            {
                SetCar(-1);
                CarTransition(-1);
            }
        }
    }

    public void SetCar(int add)
    {
        if(currentCar == 0 && add == -1)
        {
            currentCar = cars.Length - 1;
        }
        else
        {
            currentCar += add;
            currentCar = currentCar % cars.Length;
        }
    }

    SelectCar.swipes extra;
    void CarTransition(int add)
    {
        Vector3 offsetPos = CarStartPos.position + cam.transform.right * carDistance;
        Vector3 offsetPosLt = CarStartPos.position + -cam.transform.right * carDistance;

        if (currentCar == cars.Length - 1 && add == -1)
        {
            StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position + basePlateOffset, offsetPos, basePlates[0]));
        }
        else if (currentCar == 0 && add == 1)
        {
            StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position + basePlateOffset, offsetPosLt, basePlates[cars.Length - 1]));
        }
        else
        {
            if (add == 1)
            {
                StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position + basePlateOffset, offsetPosLt, basePlates[(currentCar - add) % cars.Length]));
            }
            else
            {
                StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position + basePlateOffset, offsetPos, basePlates[(currentCar - add) % cars.Length]));
            }
        }

        if (add == 1)
        {
            StartCoroutine(SmoothLerp(lerpTime, offsetPos, CarStartPos.position + basePlateOffset, basePlates[currentCar]));
        }
        else
        {
            StartCoroutine(SmoothLerp(lerpTime, offsetPosLt, CarStartPos.position + basePlateOffset, basePlates[currentCar]));
        }

        if (player == cars[currentCar])
        {
            PriceGameObject.GetComponent<TextMeshProUGUI>().text = "";
            selectButton.interactable = false;
            buyButton.interactable = false;
        }
        else
        {
            if ((carAvailability & 1 << currentCar) == 1 << currentCar)
            {
                PriceGameObject.GetComponent<TextMeshProUGUI>().text = "";
                buyButton.interactable = false;
                selectButton.interactable = true;
            }
            else
            {
                PriceGameObject.GetComponent<TextMeshProUGUI>().text = "(" + carSelects[currentCar].price + ")";
                buyButton.interactable = true;
                selectButton.interactable = false;
            }
        }
    }

    float m_t;
    bool lerping;
    IEnumerator SmoothLerp(float time, Vector3 startingGameObejctPositionSet, Vector3 targetPos, GameObject gameobj)
    {
        m_t = 0;
        gameobj.transform.position = startingGameObejctPositionSet;
        lerping = true;

        while (gameobj.transform.position != targetPos)
        {
            gameobj.transform.position = Vector3.Lerp(startingGameObejctPositionSet, targetPos, m_t / time);
            m_t += Time.deltaTime;
            yield return null;
        }
        lerping = false;
    }

    void ChangeChildrenLayerRecursive(Transform trns, int layer)
    {
        trns.gameObject.layer = layer;
        foreach (Transform child in trns)
        {
            ChangeChildrenLayerRecursive(child, layer);
        }
    }

    [System.Serializable]
    public class CarSelect
    {
        public GameObject car;
        public GameObject basePlate;
        public int price;
    }
}
