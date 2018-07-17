using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System;

public class GameManager : MonoBehaviour {

    public static bool playerDead;
    public static float playerHealth;
    bool gameStarted;

    public Meteors meteors;
    public EndlessTerrain endlessTerrain;
    [Space]
    GameObject player;
    Controller_Rover playerRover;
    public GameObject cam;
    public Transform cameraStartPos;
    

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

    public static GameManager instance;

    [Space]

    [Header("UI")]
    public GameObject controlPanel;
    public GameObject menuPanel;
    [Space]
    public GameObject ScoreGameObject;
    TextMeshProUGUI scoreText;
    [Space]
    public Slider healthSlider;
    [Space]
    public GameObject CoinGameObject;
    public Image CoinImage;
    TextMeshProUGUI coinText;
    [Space]
    public GameObject LoadingPanelGameObject;
    public Slider LoadingSlider;


    private void OnEnable()
    {
        instance = this;
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
        cam.transform.position = cameraStartPos.position;

        meteors.player = player;
        endlessTerrain.viewer = player.transform;

        player.tag = "Car";

        gameStarted = false;
        playerDead = false;
    }

    void SetupUI()
    {
        if(controlPanel.activeSelf)
            controlPanel.SetActive(false);

        scoreText = ScoreGameObject.GetComponent<TextMeshProUGUI>();
        coinText = CoinGameObject.GetComponent<TextMeshProUGUI>();
        coinsOnMenuText = coinsOnMenu.GetComponent<TextMeshProUGUI>();

        playerHealth = playerComponent.StartingHealth;
        healthSlider.maxValue = playerComponent.MaxHealth;
        coinText.text = coinsOnMenuText.text = Coins.ToString();

        coinText.alpha = 0;
        CoinImage.CrossFadeAlpha(0, 0, true);
    }

    void InstantiateCars()
    {
        if (PlayerPrefs.HasKey("Car"))
        {
            currentCar = PlayerPrefs.GetInt("Car");
        }
        else
        {
            currentCar = 0;
            PlayerPrefs.SetInt("Car", currentCar);
        }

        cars = new GameObject[carSelects.Length];
        basePlates = new GameObject[carSelects.Length];
        GameObject parent = new GameObject("Car Select");
        for (int i = 0; i < cars.Length; i++)
        {
            int index = (i+currentCar) % cars.Length;
            cars[index] = Instantiate(carSelects[index].car, CarStartPos.position + cam.transform.right * 25 * (i), Quaternion.identity, parent.transform); 
            cars[index].GetComponent<Rigidbody>().isKinematic = true;
            basePlates[index] = Instantiate(carSelects[index].basePlate, CarStartPos.position + cam.transform.right * 25 * (i), Quaternion.identity, parent.transform);
            basePlates[index].GetComponent<BasePlate>().car = cars[index].transform;
        } 

        player = cars[currentCar];

        selectButton.interactable = false;
        buyButton.interactable = false;
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
    }

    public void ReadyScene()
    {
        LoadingPanelGameObject.SetActive(false);
    }

    public void StartGame()
    {
        playerComponent = player.GetComponent<Player>();
        playerRover = player.GetComponent<Controller_Rover>();

        for (int i = 0; i < cars.Length; i++)
        {
            if(cars[i] == player)
            {
                basePlates[i].gameObject.SetActive(false);
            }
            else
            {
                basePlates[i].gameObject.SetActive(false);
                cars[i].gameObject.SetActive(false);
            }
        }

        player.GetComponent<Rigidbody>().isKinematic = false;
        playerComponent.enabled = meteors.enabled = playerRover.enabled = true;
        healthSlider.value = playerComponent.StartingHealth;

        endlessTerrain.viewer = player.transform;

        cam.GetComponent<CameraControl>().enabled = true;
        cam.GetComponent<CameraControl>().target = player;

        menuPanel.SetActive(false);
        controlPanel.SetActive(true);
        gameStarted = true;
    }

    public void UpdateHealth(float _health)
    {
        healthSlider.value += _health;
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
        //flash in and then back out so it is not always on
    }

    IEnumerator FlashCoins()
    {
        coinText.alpha = 1;
        CoinImage.CrossFadeAlpha(1, 0.5f, false);
        yield return new WaitForSeconds(1);
        coinText.alpha = 0;
        CoinImage.CrossFadeAlpha(0, 0.5f, false);
    }

    public void EndGame()
    {
        playerRover.enabled = false;
        PlayerPrefs.SetInt("Coins", Coins);   
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
            StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position, offsetPos, cars[0]));
            StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position, offsetPos, basePlates[0]));
        }
        else if (currentCar == 0 && add == 1)
        {
            StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position, offsetPosLt, cars[cars.Length - 1]));
            StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position, offsetPosLt, basePlates[cars.Length - 1]));
        }
        else
        {
            if (add == 1)
            {
                StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position, offsetPosLt, cars[(currentCar - add) % cars.Length]));
                StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position, offsetPosLt, basePlates[(currentCar - add) % cars.Length]));
            }
            else
            {
                StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position, offsetPos, cars[(currentCar - add) % cars.Length]));
                StartCoroutine(SmoothLerp(lerpTime, CarStartPos.position, offsetPos, basePlates[(currentCar - add) % cars.Length]));
            }
        }

        if (add == 1)
        {
            StartCoroutine(SmoothLerp(lerpTime, offsetPos, CarStartPos.position, basePlates[currentCar]));
            StartCoroutine(SmoothLerp(lerpTime, offsetPos, CarStartPos.position, cars[currentCar]));
        }
        else
        {
            StartCoroutine(SmoothLerp(lerpTime, offsetPosLt, CarStartPos.position, basePlates[currentCar]));
            StartCoroutine(SmoothLerp(lerpTime, offsetPosLt, CarStartPos.position, cars[currentCar]));
        }

        if(player == cars[currentCar])
        {
            selectButton.interactable = false;
            buyButton.interactable = false;
        }
        else
        {
            selectButton.interactable = true;
            if ((carAvailability & 1 << currentCar) == 1 << currentCar)
            {
                buyButton.interactable = false;
            }
            else
            {
                selectButton.interactable = false;
                buyButton.interactable = true;
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

    [System.Serializable]
    public class CarSelect
    {
        public GameObject car;
        public GameObject basePlate;
        public int price;
    }
}
