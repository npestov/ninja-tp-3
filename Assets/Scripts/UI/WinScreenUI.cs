using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WinScreenUI : MonoBehaviour
{
    [SerializeField]
    private Button btnNext;
    [SerializeField]
    private Button btnEquipWeapon;
    private GameObject container;
    [SerializeField]
    private TextMeshProUGUI addedCoins;
    [SerializeField]
    private CoinCOllection coinCOllection;
    bool displayedOnce;
    EndingBonus endingBonus;

    //weapon image
    public Image weaponImage;
    public Image weaponsBackground;
    public Sprite[] weaponSprites;

    private int LVL_FOR_WEAPON;

    // Start is called before the first frame update
    void Awake()
    {
        btnEquipWeapon.onClick.AddListener(WeaponEquiped);
        btnNext.onClick.AddListener(NextClicked);
        container = gameObject.transform.GetChild(0).gameObject;
        coinCOllection = FindObjectOfType<CoinCOllection>();
        endingBonus = FindObjectOfType<EndingBonus>();
        LVL_FOR_WEAPON = 4;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        SetUpWeapon();
    }

    private void NextClicked()
    {
        PlayerPrefs.SetInt("RealLvl", PlayerPrefs.GetInt("RealLvl") + 1);
        PlayerPrefs.SetFloat("sharpness", PlayerPrefs.GetFloat("sharpness") + 1);
        //this is so that the levels loop
        int levelIndex = (PlayerPrefs.GetInt("lvl") + 1);
        if (levelIndex == 1)
        {
            PlayerPrefs.SetInt("lvl", 2);
            levelIndex = 2;
        }
        if (SceneManager.GetActiveScene().buildIndex + 1== SceneManager.sceneCountInBuildSettings)
        {
            PlayerPrefs.SetInt("lvl", 9);
            levelIndex = 9;
        }
        SceneManager.LoadSceneAsync(levelIndex % SceneManager.sceneCountInBuildSettings);
        PlayerPrefs.SetInt("lvl", levelIndex);
        Destroy(btnNext.gameObject);
    }

    private void WeaponEquiped()
    {
        int weaponIndex = Mathf.FloorToInt(PlayerPrefs.GetFloat("sharpness")) / LVL_FOR_WEAPON;
        weaponIndex = Mathf.Clamp(weaponIndex, 0, 8);
        PlayerPrefs.SetInt("equippedSword", weaponIndex +1);
        btnEquipWeapon.enabled = false;
        btnEquipWeapon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "EQUIPPED";
    }

    private void SetUpWeapon()
    {
        int weaponIndex = Mathf.FloorToInt(PlayerPrefs.GetFloat("sharpness")) / LVL_FOR_WEAPON;
        if (weaponIndex < weaponSprites.Length)
        {
            weaponImage.sprite = weaponSprites[weaponIndex];
            weaponsBackground.sprite = weaponSprites[weaponIndex];
        }
        else
        {
            //if all weapons unlocked
            weaponsBackground.enabled = false;
            weaponImage.enabled = false;
            return;
        }
        
        switch (Mathf.FloorToInt(PlayerPrefs.GetFloat("sharpness")) % LVL_FOR_WEAPON + 1)
        {
            case 1:
                weaponImage.fillAmount = 0.25f;
                Debug.Log("CASE 1");
                break;
            case 2:
                weaponImage.fillAmount = 0.5f;
                Debug.Log("CASE 2");
                break;
            case 3:
                weaponImage.fillAmount = 0.75f;
                Debug.Log("CASE 3");
                break;
            case 0:
                weaponImage.fillAmount = 1;
                btnEquipWeapon.gameObject.SetActive(true);
                Debug.Log("CASE 0");
                break;
            default:
                weaponImage.fillAmount = 1;
                btnEquipWeapon.gameObject.SetActive(true);
                Debug.Log("CASE default");
                break;


        }
    }

    public IEnumerator DisplayWinScreen()
    {
        if (!displayedOnce)
        {
            displayedOnce = true;
            coinCOllection = FindObjectOfType<CoinCOllection>();
            yield return new WaitForSeconds(1);

            if (GameObject.FindObjectsOfType<InGameUI>().Length != 0)
                FindObjectOfType<InGameUI>().gameObject.SetActive(false);

            float multiplier = endingBonus.finalMultiplier;

            int coinsEarned = coinCOllection.coinsEarned;
            int lvlBoost = PlayerPrefs.GetInt("RealLvl");
            lvlBoost = (int)Mathf.Clamp(lvlBoost, 1, 15);
            coinsEarned += 20 + lvlBoost;
            if (multiplier < 1)
                multiplier = 1;
            addedCoins.text = coinsEarned.ToString() + " X " + multiplier.ToString("F1") + " = " + (int)(coinsEarned * multiplier);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + (int)((coinsEarned * multiplier)));
            container.SetActive(true);
        }

    }
}
