using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    private GameObject container;
    [SerializeField]
    private TextMeshProUGUI txtLevel;

    [SerializeField]
    private Text txtCoins;
    [SerializeField]
    private Button btnEnterStore;

    private GameObject storeContainer;

    //level
    PlayerLvlManager playerLvlManager;
    public Text lvlUpgradePriceTxt;
    public TextMeshProUGUI currentLvl;
    private int lvlUpgradeCost = 100;

    //damage power
    public Text powerUpgradePriceTxt;
    public TextMeshProUGUI currentPower;
    private int powerUpgradeCost = 100;

    // Start is called before the first frame update
    void Awake()
    {
        container = gameObject.transform.GetChild(0).gameObject;
        playerLvlManager = FindObjectOfType<PlayerLvlManager>();
    }
    private void Start()
    {
        txtLevel.text = "Stage: " + PlayerPrefs.GetInt("lvl");
        ChangeCoins();
        btnEnterStore.onClick.AddListener(EnableStore);
        UpdateLvlPrice();
        UpdatePowerPrice();
    }

    void EnableStore()
    {
        GameObject.Find("Store UI").transform.GetChild(0).gameObject.SetActive(true);
        FindObjectOfType<MainShop>().ChangeCoins();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveMenu()
    {
        container.SetActive(false);
    }

    public void ChangeCoins()
    {
        txtCoins.text = PlayerPrefs.GetInt("coins").ToString();
    }

    public void BuyLvlUpgrade()
    {
        if (PlayerPrefs.GetInt("coins") >= lvlUpgradeCost)
        {
            PlayerPrefs.SetInt("playerLvl", (PlayerPrefs.GetInt("playerLvl", 5) + 1));
            playerLvlManager.ChangeTxt();
            UpdateLvlPrice();

            PlayerPrefs.SetInt("coins",(PlayerPrefs.GetInt("coins", 5) - lvlUpgradeCost));
            ChangeCoins();
        }
        
    }
    public void BuyPowerUpgrade()
    {
        if (PlayerPrefs.GetInt("coins") >= powerUpgradeCost)
        {
            PlayerPrefs.SetInt("power", (PlayerPrefs.GetInt("power", 1) + 1));
            UpdatePowerPrice();

            PlayerPrefs.SetInt("coins", (PlayerPrefs.GetInt("coins", 5) - powerUpgradeCost));
            ChangeCoins();
        }

    }

    private void UpdateLvlPrice()
    {
        lvlUpgradeCost = 100 + ((PlayerPrefs.GetInt("playerLvl", 5) - 5) * 10);
        lvlUpgradeCost = Mathf.Clamp(lvlUpgradeCost, 100, 600);
        lvlUpgradePriceTxt.text = lvlUpgradeCost.ToString();
        currentLvl.text = "level " + PlayerPrefs.GetInt("playerLvl", 5).ToString();

    }
    private void UpdatePowerPrice()
    {
        powerUpgradeCost = 500 + ((PlayerPrefs.GetInt("power", 1) - 1) * 200);
        powerUpgradePriceTxt.text = powerUpgradeCost.ToString();
        currentPower.text = "power " + PlayerPrefs.GetInt("power", 1).ToString();
    }
}
