using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopTab : MonoBehaviour
{
    public int tabIndex;

    [System.Serializable]
    public class ShopData
    {
        [HideInInspector]
        public int equipedIndex = 0;
        [HideInInspector]
        public bool[] isPurchased = new bool[30];

        public void SetEquiped(int equipedIndex)
        {
            this.equipedIndex = equipedIndex;
            this.isPurchased[equipedIndex] = true;
            Debug.Log(this.equipedIndex);
        }
        public void SetIsPurchased(bool isPurchased, int index)
        {
            this.isPurchased[index] = isPurchased;
        }


    }
    public ShopItem[] shopItemsList;
    public Image displayEquippedImage;

    public ShopData shopData = new ShopData();

    GameObject g;

    private SwordSpawner swordSpawner;


    // Start is called before the first frame update
    void Start()
    {
        GenerateShopArray();
        LoadShopData();
        SetButtonStates();
        swordSpawner = FindObjectOfType<SwordSpawner>();
        if (transform.name.Equals("sword tab"))
            displayEquippedImage.sprite = shopItemsList[PlayerPrefs.GetInt("equippedSword")].myImg;
        else if (transform.name.Equals("skins tab"))
            displayEquippedImage.sprite = shopItemsList[PlayerPrefs.GetInt("equippedSkin")].myImg;
    }


    void LoadShopData()
    {
        string fileName = "shopdata" + transform.name;
        shopData = BinarySerializer.Load<ShopData>(fileName);
    }

    void SetButtonStates()
    {
        for (int i = 0; i < shopItemsList.Length; i++)
        {
            if (shopData.isPurchased[i] && transform.name != "sword tab")
            {
                shopItemsList[i].ChangeButtonState(ButtonState.EQUIPABLE);
            }

        }
        if (transform.name.Equals("sword tab"))
        {
            shopItemsList[PlayerPrefs.GetInt("equippedSword")].ChangeButtonState(ButtonState.EQUIPED);
        }
        else if (transform.name.Equals("skins tab"))
        {
            shopItemsList[PlayerPrefs.GetInt("equippedSkin")].ChangeButtonState(ButtonState.EQUIPED);
        }

    }

    void GenerateShopArray()
    {
        shopItemsList = FindObjectsOfType<ShopItem>();

        for (int i = 1; i < shopItemsList.Length; i++)
        {
            int j = i;
            while (j > 0 && shopItemsList[j].MyIndex < shopItemsList[j - 1].MyIndex)
            {
                ShopItem temp = shopItemsList[j - 1];
                shopItemsList[j - 1] = shopItemsList[j];
                shopItemsList[j] = temp;
                j--;
            }
        }
    }

    public void ChangeEquipped(int index)
    {
        if (transform.name.Equals("sword tab"))
        {
            PlayerPrefs.SetInt("equippedSword", index);
            swordSpawner.DeleteOldSword();
            swordSpawner.SpawnNewSword(index);
            shopItemsList[PlayerPrefs.GetInt("equippedSword")].ChangeButtonState(ButtonState.EQUIPED);
        }
        if (transform.name.Equals("skins tab"))
        {
            PlayerPrefs.SetInt("equippedSkin", index);
            shopItemsList[PlayerPrefs.GetInt("equippedSkin")].ChangeButtonState(ButtonState.EQUIPED);
        }

        for (int i = 0; i < shopItemsList.Length; i++)
        {
            if (shopItemsList[i].IsPurchased && transform.name.Equals("skins tab") && PlayerPrefs.GetInt("equippedSkin") !=i)
                shopItemsList[i].ChangeButtonState(ButtonState.EQUIPABLE);
            else if (transform.name.Equals("sword tab") && shopItemsList[i].State == ButtonState.EQUIPED && PlayerPrefs.GetInt("equippedSword") != i)
                shopItemsList[i].ChangeButtonState(ButtonState.EQUIPABLE);
        }

        //save data
        shopData.SetEquiped(index);
        BinarySerializer.Save(shopData, "shopdata" + transform.name);
        displayEquippedImage.sprite = shopItemsList[index].myImg;
    }
}

