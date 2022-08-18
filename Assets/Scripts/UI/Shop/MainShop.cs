using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainShop : MonoBehaviour
{
    private ShopTab[] shopTabs;
    private Button[] tabButtons;
    [SerializeField]
    private Text txtCoins;

    public void Awake()
    {
        FindTabs();
        tabButtons = GameObject.Find("Tabs").GetComponentsInChildren<Button>();
        EnableShopTab(0);
    }

    public void EnableMenu()
    {
        FindObjectOfType<MenuUI>().transform.GetChild(0).gameObject.SetActive(true);
        FindObjectOfType<MenuUI>().ChangeCoins();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void EnableShopTab(int enabledIndex)
    {
        Debug.Log("tab clicked");
        for (int i = 0; i < shopTabs.Length; i++)
        {
            shopTabs[i].gameObject.SetActive(false);
            tabButtons[i].transform.GetChild(0).gameObject.SetActive(false);
            tabButtons[i].transform.GetChild(1).gameObject.SetActive(true);
        }
        shopTabs[enabledIndex].gameObject.SetActive(true);
        tabButtons[enabledIndex].transform.GetChild(0).gameObject.SetActive(true);

    }

    private void FindTabs()
    {
        shopTabs = FindObjectsOfType<ShopTab>();
        for (int i = 1; i < shopTabs.Length; i++)
        {
            int j = i;
            while (j > 0 && shopTabs[j].tabIndex < shopTabs[j - 1].tabIndex)
            {
                ShopTab temp = shopTabs[j - 1];
                shopTabs[j - 1] = shopTabs[j];
                shopTabs[j] = temp;
                j--;
            }
        }
    }

    public void ChangeCoins()
    {
        txtCoins.text = PlayerPrefs.GetInt("coins").ToString();
    }

}
