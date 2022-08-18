using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

[ExecuteInEditMode]
public class EnemyLvlManager : MonoBehaviour
{
    PlayerLvlManager playerLvlManager;
    public int LVL;
    public int lvlBoost;
    private TextMeshProUGUI lvlTxt;

    public SpriteRenderer bg;

    private Color32 red;
    private Color32 green;
    [HideInInspector]
    public bool isKillable;

    public bool endingBonus;
    public bool isPawn;

    public GameObject arrowDown;

    public bool randomSkinGenerator;

    public LevelColors levelColors;

    private int originalLVL;

    public GameObject notKillableParticle;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnStateChanged;
        if (!endingBonus)
        {
            RandomSkin();
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnStateChanged;
    }

    private void GameManagerOnStateChanged(GameState state)
    {
        if (state == GameState.Walking && bg != null)
            CheckLVLState();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerLvlManager = FindObjectOfType<PlayerLvlManager>();
        if (!endingBonus || isPawn)
            ScaleToPlayer();

        originalLVL = LVL;

        green = new Color32(0, 214, 0, 255);
        red = new Color32(255,38,63,255);
        lvlTxt = transform.GetComponentInChildren<TextMeshProUGUI>();
        AlterLVLtxt();
        CheckLVLState();


    }

    public void ScaleLVLDown()
    {
        lvlTxt.transform.parent.DOScale(lvlTxt.transform.parent.localScale - new Vector3(0.5f, 0.5f, 0.5f), 0.3f);
    }

    public void HideLVL()
    {
        lvlTxt.transform.parent.gameObject.SetActive(false);
    }

    //returns weather enemy is dead
    public bool ReduceLVL(int reduction)
    {
        //reduction = originalLVL / 10;
        //reduction = Mathf.Clamp(1, )
        if (arrowDown != null)
            arrowDown.SetActive(true);
        LVL -= reduction;
        AlterLVLtxt();
        //inGameUI.SpawnDamageTest(reduction, new Vector2(Random.Range(0f, Screen.width), Random.Range(0f, Screen.height)));
        if (LVL <= 0)
            return true;
        else
            return false;
    }

    public void AlterLVLtxt()
    {
        if (LVL < 0)
            LVL = 0;
        if (lvlTxt != null)
            lvlTxt.text = "LVL " + LVL;
    }

    public bool CheckLVLState()
    {
        if (endingBonus || bg == null)
            return false;

        if (playerLvlManager.playerLvl >= LVL || 5 >= LVL)
        {
            bg.color = green;
            isKillable = true;
            if (notKillableParticle)
                notKillableParticle.SetActive(false);
            return true;
        }
        else
        {
            isKillable = false;
            bg.color = red;
            if (notKillableParticle)
                notKillableParticle.SetActive(true);
            return false;
        }
    }
    //if you want a specific skin then pass a parameter
    public void RandomSkin(int specificSkin = -1)
    {

        var parent = transform.Find("Skin");
        int numofChildren = parent.childCount - 1;

        int skinNumber = specificSkin;
        if (skinNumber == -1)
            skinNumber = UnityEngine.Random.Range(0, numofChildren);

        for (int i =0; i < numofChildren; i++)
        {
            parent.GetChild(i).gameObject.SetActive(false);
        }
        if (!randomSkinGenerator)
            skinNumber = 0;
        parent.GetChild(skinNumber).gameObject.SetActive(true);
    }

    private void SetSkinColor()
    {
        return;
        int index = (int)Mathf.Floor(LVL / 10f);
        index = Mathf.Clamp(index, 0, levelColors.LvlColors.Length - 1);
        var mySkin = GetComponentInChildren<SkinnedMeshRenderer>();
        if (mySkin != null)
        {
            mySkin.material = levelColors.LvlColors[index];
        }
        else
            GetComponentInChildren<MeshRenderer>().material = levelColors.LvlColors[index];

    }

    private void ScaleToPlayer()
    {
        if (GetComponentsInChildren<TargetScript>().Length == 1 && GetComponentInChildren<TargetScript>().CreativeOnly)
            return;
        int maxLimit = playerLvlManager.playerLvl - 6;
        maxLimit = Mathf.Clamp(maxLimit, 0, 100);
        int lowerLimit = playerLvlManager.playerLvl - 6 - 5;
        lowerLimit = Mathf.Clamp(lowerLimit, 0, playerLvlManager.playerLvl - 7);
        LVL += UnityEngine.Random.Range(lowerLimit, maxLimit);

        if (isPawn)
        {
            LVL = PlayerPrefs.GetInt("playerLvl", 5);
            Debug.Log("PLAYERS LVL: " + playerLvlManager.playerLvl);
        }

        SetSkinColor();
    }
}

