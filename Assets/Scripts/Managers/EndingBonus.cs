using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingBonus : MonoBehaviour
{
    [SerializeField]
    private GameObject[] allBonus;
    private GameObject bonusTarget;
    [HideInInspector]
    public float finalMultiplier;
    [HideInInspector]
    public float roulleteMultiplier;
    [HideInInspector]
    public int targetIndex;
    [SerializeField]
    float levelIncriment;
    int incNum;

    private float[] Incriment = new float[] { 5, 6, 8f, 10, 14, 15,17,19,21,24};

    // Start is called before the first frame update

    private void Awake()
    {
        AssignIncriment();
    }
    void Start()
    {
        //PlayerPrefs.SetFloat("sharpness", 1010000);
        allBonus = GameObject.FindGameObjectsWithTag("bonus");
        bonusTarget = GameObject.Find("BonusTarget");


        SortByDistance();
        AssignLevels();
        AssignMutiplier();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void SortByDistance()
    {
        for (int i = 1; i < allBonus.Length; i++)
        {
            int j = i;
            while (j > 0 && IsFirstCloser(allBonus[j], allBonus[j - 1]))
            {
                GameObject temp = allBonus[j - 1];
                allBonus[j - 1] = allBonus[j];
                allBonus[j] = temp;
                j--;
            }
        }
    }

    bool IsFirstCloser(GameObject firstObj, GameObject secObj)
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        if (Vector3.Distance(firstObj.transform.position, playerPos) < Vector3.Distance(secObj.transform.position, playerPos))
            return true;

        return false;
    }

    void AssignIncriment()
    {
        incNum = PlayerPrefs.GetInt("boss", 0);
        incNum = Mathf.Clamp(incNum, 0, Incriment.Length - 1);

        int bossNum = incNum;
        if (bossNum >= Incriment.Length)
        {
            bossNum = bossNum % Incriment.Length;
        }
        levelIncriment = Incriment[bossNum];

        Transform boss = GetComponentInChildren<Boss>().transform.Find("Skin");

        for (int i = 0; i < boss.childCount; i++)
        {
            boss.GetChild(i).gameObject.SetActive(false);
        }
        boss.GetChild(incNum).gameObject.SetActive(true);

        //the skins of the youngings
        /*
        foreach(GameObject g in allBonus)
        {
            if(g.GetComponents<Boss>().Length == 0)
                g.GetComponent<EnemyLvlManager>().RandomSkin(incNum);
        }
        */
    }

    void AssignLevels()
    {
        for (int i = 0; i < allBonus.Length; i++)
        {
            var lvlMang = allBonus[i].GetComponent<EnemyLvlManager>();
            if (lvlMang == null)
                return;
            lvlMang.LVL = 20 + (int)Mathf.Floor(levelIncriment * i);
            lvlMang.AlterLVLtxt();
        }
    }

    void AssignMutiplier()
    {
        Transform canvas = transform.Find("Bonus Canvas");
        for (int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).GetComponent<TextMeshProUGUI>().text = "X" + ((1.2 + (0.2f * i)) + (incNum * 0.2f * i)).ToString("F1");
        }
    }


    public Transform WhichEnemyToThrowTo()
    {
        int playerLvl = FindObjectOfType<PlayerLvlManager>().playerLvl;
        playerLvl *= ((int)((Mathf.Round(FindObjectOfType<EndingBonus>().roulleteMultiplier) / 10) + 1));
        Transform target = allBonus[0].transform;
        for (int i = 0; i < allBonus.Length-1; i++)
        {
            if (allBonus[i].GetComponent<EnemyLvlManager>().LVL < playerLvl)
                continue;
            else
            {
                if (i == 0)
                    i = 1;

                finalMultiplier = (1.2f + (0.2f * i)) + (incNum * 0.2f * i);

                try
                {
                    target = allBonus[i].transform;
                }
                catch(Exception e)
                {
                    target = allBonus[i-1].transform;
                }

                return target;

            }
        }

        //if this hits you reached the end.
        AttackMoveController attackMoveController = FindObjectOfType<AttackMoveController>();
        attackMoveController.enemyToKill = allBonus[allBonus.Length - 1].GetComponentInChildren<TargetScript>().gameObject;
        //attackMoveController.Warp();
        finalMultiplier = (1.2f + (0.2f * allBonus.Length)) + (incNum * 0.2f * allBonus.Length);
        return allBonus[allBonus.Length-1].transform;
    }


}
