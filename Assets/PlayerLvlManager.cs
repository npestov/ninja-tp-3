using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerLvlManager : MonoBehaviour
{
    [HideInInspector]
    public int playerLvl;
    private TextMeshProUGUI myLvlTxt;
    public GameObject increaseArrow;
    public GameObject decreaseArrow;
    public LevelColors levelColors;
    public SkinnedMeshRenderer mySkin;

    EnemyLvlManager[] allEn;

    private float scalePerLvl = 0.002f;

    // Start is called before the first frame update
    void Start()
    {
        allEn = FindObjectsOfType<EnemyLvlManager>();
        PlayerPrefs.SetInt("RealLvl", 100);
        //PlayerPrefs.SetInt("coins", 100000);
        playerLvl = PlayerPrefs.GetInt("playerLvl", 5);
        if (playerLvl < 5)
        {
            PlayerPrefs.SetInt("playerLvl", 5);
            playerLvl = 5;
        }
        myLvlTxt = GetComponentInChildren<TextMeshProUGUI>();
        ChangeTxt();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeLvl(int increase, bool positive = true, float delay = 0.4f)
    {
        StartCoroutine(IncreaseTextSlowly(increase, positive, delay));
    }

    public void PutLVLTextHigher()
    {
        //myLvlTxt.transform.DOMoveY(myLvlTxt.transform.position.y + 0.6f, 1f);
        myLvlTxt.DOFade(0, 1);
    }

    public void ChangeTxt()
    {
        if (GameManager.Instance.State == GameState.Menu)
            playerLvl = PlayerPrefs.GetInt("playerLvl", 5);
        myLvlTxt.text = playerLvl + " level";
        ChangeSkinColor(playerLvl);
    }

    private void ChangeSkinColor(int i)
    {
        return;
        int index = (int)Mathf.Floor(i / 10f);
        index = Mathf.Clamp(index, 0, levelColors.LvlColors.Length - 1);

        mySkin.material = levelColors.LvlColors[index];
    }

    IEnumerator IncreaseTextSlowly(int increase, bool positive, float delay = 0.4f)
    {
        //myLvlTxt.transform.DOMoveY(myLvlTxt.transform.position.y - 0.3f, 03f);
        myLvlTxt.DOFade(1, 0.1f);
        var oldLvl = playerLvl;

        if (positive)
        {
            increaseArrow.SetActive(true);
            playerLvl += increase;
        }
        else
        {
            decreaseArrow.SetActive(true);
            playerLvl -= increase;
        }

        CheckAllEnemyLevelState();

        yield return new WaitForSeconds(delay);

        for (int i = 0; i < increase; i++)
        {
            if (positive)
                oldLvl++;
            else
                oldLvl--;
            myLvlTxt.text = oldLvl + " level";
            //change skin
            ChangeSkinColor(oldLvl);
            //increase scale
            transform.localScale += new Vector3(scalePerLvl, scalePerLvl, scalePerLvl);
            yield return new WaitForSeconds(0.7f / increase);
        }

        ChangeTxt();

        increaseArrow.SetActive(false);
        decreaseArrow.SetActive(false);
    }

    public void IncreaseLevelInstantly(int increment, bool positive = true)
    {
        if (positive)
        {
            decreaseArrow.SetActive(false);
            increaseArrow.SetActive(true);
            playerLvl += increment;
            transform.localScale += new Vector3(scalePerLvl * increment, scalePerLvl * increment, scalePerLvl * increment);
        }
        else
        {
            increaseArrow.SetActive(false);
            decreaseArrow.SetActive(true);
            playerLvl -= increment;
            if (playerLvl <= 0)
            {
                GameManager.Instance.UpdateGameState(GameState.Lose);
            }
        }
        myLvlTxt.text = playerLvl + " level";
        StopAllCoroutines();
        StartCoroutine(DisableArrows());
        CheckAllEnemyLevelState();
    }

    private IEnumerator DisableArrows()
    {
        yield return new WaitForSeconds(0.6f);
        increaseArrow.SetActive(false);
        decreaseArrow.SetActive(false);
    }

    private void CheckAllEnemyLevelState()
    {
        foreach (EnemyLvlManager en in allEn)
        {
            if (en)
                en.CheckLVLState();
        }
    }


}
