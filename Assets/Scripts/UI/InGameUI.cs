using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    private GameObject container;
    [SerializeField]
    private GameObject killScreen;
    [SerializeField]
    private GameObject other;
    [SerializeField]
    private TextMeshProUGUI coins;

    public Button killButton;
    public Button cancelKillButton;
    public DynamicJoystick mainJoystick;
    private AttackMoveController attackMoveController;

    public TextMeshProUGUI damageTxt;

    public GameObject roullete;
    public Transform endingCursorParent;
    private bool endingCursorMoving = false;
    float cursoRotLimit = 49.2f;
    public TextMeshProUGUI multiplierTxt;


    // Start is called before the first frame update
    void Start()
    {
        container = gameObject.transform.GetChild(0).gameObject;
        attackMoveController = FindObjectOfType<AttackMoveController>();
        roullete.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (endingCursorMoving)
        {
            var speed = 1;
            float rZ = Mathf.SmoothStep(-cursoRotLimit, cursoRotLimit, Mathf.PingPong(Time.time * speed, 1));
            endingCursorParent.rotation = Quaternion.Euler(0, 0, rZ);

            var multValue = 0;

            if (Mathf.Abs(rZ) > 3.8f && Mathf.Abs(rZ) <= 11.79)
                multValue = 6;
            else if (Mathf.Abs(rZ) > 11.79 && Mathf.Abs(rZ) <= 19.9f)
                multValue = 5;
            else if (Mathf.Abs(rZ) > 19.9f && Mathf.Abs(rZ) <= 28)
                multValue = 4;
            else if (Mathf.Abs(rZ) > 28 && Mathf.Abs(rZ) <= 35.9f)
                multValue = 3;
            else if (Mathf.Abs(rZ) > 35.9f && Mathf.Abs(rZ) <= 43.5f)
                multValue = 2;
            else if (Mathf.Abs(rZ) > 43.5f)
                multValue = 1;
            else
                multValue = 7;

            multiplierTxt.text = "X" + multValue;
            
            if (Input.GetMouseButtonDown(0))
            {
                endingCursorMoving = false;
                StartCoroutine(ThrowDelay());
                FindObjectOfType<EndingBonus>().roulleteMultiplier = multValue;
            }
        }
    }
    private IEnumerator ThrowDelay()
    {
        yield return new WaitForSeconds(0.5f);
        roullete.SetActive(false);
        GameManager.Instance.CursorClickedCallback();
    }

    public void EnableInGameUI()
    {
        other.SetActive(true);
    }
    public void DisableInGameUI()
    {
        other.SetActive(false);
    }

    public void DisableJoystick()
    {
        mainJoystick.gameObject.SetActive(false);
    }


    public void ShowMultiplier()
    {
        //StartCoroutine(Mult());
    }

    public void SpawnDamageTest(int damageNum, Vector2 pos)
    {
        var dmg = Instantiate(damageTxt, pos, transform.rotation);
        dmg.transform.parent = transform;
        dmg.gameObject.SetActive(true);
        dmg.text = "-" + damageNum;
        float timer = 1f;
        dmg.DOFade(0, timer);
        Destroy(dmg.gameObject, timer);
    }

    public void RestartLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MoveEndingCursor()
    {
        roullete.SetActive(true);
        endingCursorMoving = true;
    }
}
