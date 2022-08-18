using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private ShopTab shopTab;

    public string Name;
    public int Price;
    public ButtonState State;
    public ItemSection ItemSection;
    public int MyIndex;
    public bool IsPurchased;

    public Button MyButton;

    //buttons
    Transform ButtonVariantsParent;
    GameObject ObjBuy;
    GameObject ObjEquip;
    GameObject ObjEquiped;
    GameObject ObjVideoUnlock;
    GameObject ObjDisabled;
    GameObject[] AllButtons;
    //frames
    Transform FrameParent;
    GameObject ObjPurchasableFrame;
    GameObject ObjOwnedFrame;
    GameObject ObjEquipedFrame;
    GameObject[] AllFrames;
    //icons
    Transform ItemsIconParent;
    GameObject ObjEnabledIcon;
    GameObject ObjMysteryIcon;
    GameObject[] AllIcons;

    //image
    public Sprite myImg;

    public int lvlToUnlock;


    private void Start()
    {
        SetUp();
        if (lvlToUnlock == -1)
            SetPriceDisplay(Price);
        else
            ConfigureLvlUnlock();
        MyButton.onClick.AddListener(() => ButtonPressed());
    }


    void SetPriceDisplay(int price)
    {
        ObjBuy.transform.GetChild(0).GetComponent<Text>().text = price.ToString();
    }
    void ConfigureLvlUnlock()
    {
        if (PlayerPrefs.GetInt("RealLvl") >= lvlToUnlock && State != ButtonState.EQUIPED)
        {
            ChangeButtonState(ButtonState.EQUIPABLE);
        }
        else if (State != ButtonState.EQUIPED)
        {
            ChangeButtonState(ButtonState.DISABLED);
            ObjDisabled.transform.GetChild(0).GetComponent<Text>().text = "lvl " + lvlToUnlock;
        }
    }

    public void ChangeButtonState(ButtonState state)
    {
        if (ObjDisabled == null)
            SetUp();

        switch (state)
        {
            case ButtonState.DISABLED:
                ConfigureEnables(ObjDisabled, AllButtons);
                break;
            case ButtonState.VIDEO_UNLOCK:
                ConfigureEnables(ObjVideoUnlock, AllButtons);
                break;
            case ButtonState.PURCHASABLE:
                ConfigureEnables(ObjBuy, AllButtons);
                ConfigureEnables(ObjPurchasableFrame, AllFrames);
                ConfigureEnables(ObjMysteryIcon, AllIcons);
                break;
            case ButtonState.EQUIPABLE:
                ConfigureEnables(ObjEquip, AllButtons);
                ConfigureEnables(ObjOwnedFrame, AllFrames);
                ConfigureEnables(ObjEnabledIcon, AllIcons);
                IsPurchased = true;
                break;
            case ButtonState.EQUIPED:
                Debug.Log("i am equiped now" + transform.name);
                ConfigureEnables(ObjEquiped, AllButtons);
                ConfigureEnables(ObjEquipedFrame, AllFrames);
                ConfigureEnables(ObjEnabledIcon, AllIcons);
                IsPurchased = true;
                break;
        }
        State = state;
    }

    public void ButtonPressed()
    {
        switch (State)
        {
            case ButtonState.DISABLED:
                break;
            case ButtonState.VIDEO_UNLOCK:
                break;
            case ButtonState.PURCHASABLE:
                PurchaseMyself();
                break;
            case ButtonState.EQUIPABLE:
                shopTab.ChangeEquipped(MyIndex);
                break;
            case ButtonState.EQUIPED:
                break;
        }
    }

    void ConfigureEnables(GameObject enableObj, GameObject[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].gameObject.SetActive(false);
        }

        enableObj.gameObject.SetActive(true);

    }

    void PurchaseMyself()
    {
        if (PlayerPrefs.GetInt("coins") >= Price)
        {
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - Price);
            UnlockMyself();
            FindObjectOfType<MainShop>().ChangeCoins();
        }
    }
    void UnlockMyself()
    {
        shopTab.ChangeEquipped(MyIndex);
        IsPurchased = true;
    }

    void SetUp()
    {
        shopTab = transform.parent.GetComponentInParent<ShopTab>();
        MyButton = GetComponentInChildren<Button>();
        ButtonVariantsParent = transform.GetChild(3);
        ObjBuy = ButtonVariantsParent.GetChild(0).gameObject;
        ObjVideoUnlock = ButtonVariantsParent.GetChild(1).gameObject;
        ObjDisabled = ButtonVariantsParent.GetChild(2).gameObject;
        ObjEquip = ButtonVariantsParent.GetChild(3).gameObject;
        ObjEquiped = ButtonVariantsParent.GetChild(4).gameObject;
        AllButtons = new GameObject[5] { ObjBuy, ObjVideoUnlock, ObjDisabled, ObjEquip, ObjEquiped };

        FrameParent = transform.GetChild(0);
        ObjOwnedFrame = FrameParent.GetChild(0).gameObject;
        ObjEquipedFrame = FrameParent.GetChild(1).gameObject;
        ObjPurchasableFrame = FrameParent.GetChild(2).gameObject;
        AllFrames = new GameObject[3] { ObjOwnedFrame, ObjEquipedFrame, ObjPurchasableFrame };

        ItemsIconParent = transform.GetChild(1);
        ObjEnabledIcon = ItemsIconParent.GetChild(0).gameObject;
        ObjMysteryIcon = ItemsIconParent.GetChild(1).gameObject;
        AllIcons = new GameObject[2] { ObjEnabledIcon, ObjMysteryIcon };
        
        myImg = transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite;

        if (lvlToUnlock != 0)
        {

        }
    }
}
public enum ItemSection
{
    SKINS,
    SWORDS
}

public enum ButtonState
{
    DISABLED,
    VIDEO_UNLOCK,
    PURCHASABLE,
    EQUIPABLE,
    EQUIPED
}
