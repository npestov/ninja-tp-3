using System.Collections;
using UnityEngine;
using DG.Tweening;
using IndieMarc.EnemyVision;
using UnityEngine.SceneManagement;
using BzKovSoft.CharacterSlicer;
using UnityEngine.AI;
using MoreMountains.NiceVibrations;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicerSamples;

public class AttackMoveController : MonoBehaviour
{
    public bool isAttackQueued;
    //temp
    bool isQuickDash;
    public int timesTp = 0;
    private Vector3 tpPos;
    bool bonusStarted;

    public GameObject enemyToKill;
    private PlayerAnim playerAnim;
    private CameraController cameraController;
    private EndingBonus endingBonus;
    private TimeManager timeManager;
    private PlayerLvlManager playerLvlManager;
    private PlayerMovement pm;
    private SoundEffects soundEffects;
    [Space]
    public float warpDuration = .5f;

    [Space]
    //SWORD
    public Transform sword;
    private Transform swordHand;
    private Vector3 swordOrigRot;
    private Vector3 swordOrigPos;
    private MeshRenderer swordMesh;
    private Vector3 swordShootOffset;
    private float Y_OFFSET = 1.0f;

    //HOOKING
    [Space]

    public bool isRunnerSelected;
    public GameObject hookOBJ;
    GameObject currentHook;
    public Transform hookParent;
    //Bonus
    public GameObject bonusSlicer;
    Rigidbody rb;
    public GameObject manualSlicer;

    TargetScript[] allEn;

    //temp
    public bool processOfWarping = false;
    public Material hologramMaterial;
    FrenzyManager fm;

    GameObject playerRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        soundEffects = GetComponent<SoundEffects>();
        playerLvlManager = FindObjectOfType<PlayerLvlManager>();
        pm = FindObjectOfType<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<PlayerAnim>();
        cameraController = FindObjectOfType<CameraController>();
        endingBonus = FindObjectOfType<EndingBonus>();
        timeManager = FindObjectOfType<TimeManager>();
        playerRenderer = transform.GetChild(1).gameObject;
        allEn = FindObjectsOfType<TargetScript>();
        fm = FindObjectOfType<FrenzyManager>();
    }
    void Start()
    {
        //CRE adding sharpness
        Debug.Log("RELA LVL: " + PlayerPrefs.GetInt("RealLvl"));
        FindSword();
        swordShootOffset = new Vector3(0, Y_OFFSET, 0);
    }


    //WARP START
    public void WarpKill()
    {
        if (enemyToKill == null|| enemyToKill.transform.position.z > transform.position.z || (enemyToKill.GetComponentsInChildren<TargetScript>().Length != 1 && enemyToKill.GetComponents<TargetScript>().Length != 1))
        {
            GameManager.Instance.UpdateGameState(GameState.Walking);
            return;
        }
        if (playerAnim.IsAnimationPlaying("warp"))
            return;

        if (!bonusStarted)
            cameraController.SwitchToSlicing();
        enemyToKill.GetComponentInChildren<TargetScript>().passedOver = true;
        //TEMP FIX
        transform.DOLookAt(enemyToKill.transform.position, 1);

        //TEMP END

        GameManager.Instance.UpdateGameState(GameState.Killing);
        swordMesh.enabled = true;

        playerAnim.ResetAnims();

        isQuickDash = false;
        playerAnim.WarpAnim();
        //temp bug fix
        StartCoroutine(BackupWarp());
        processOfWarping = true;

        Invoke("KillAllEnemiesInRadius", 1f);

        Debug.Log("warpKill has ended");


    }

    public void Warp()
    {
        if (GameManager.Instance.State == GameState.Lose || GameManager.Instance.isBonus)
            return;

        /*
        if (!(enemyToKill.transform.position.z < transform.position.z))
        {
            FindClosestEn();
        }
        */

        //destory pure trigger
        try
        {
            Destroy(enemyToKill.transform.parent.GetChild(4).gameObject);
        }
        catch
        {

        }
        if (!GameManager.Instance.isBonus)
        {
            KillEveryPawnInRadius();
        }
        Debug.Log("inside warp func");
        MMVibrationManager.Haptic(HapticTypes.Selection);
        soundEffects.ThrowSwordPlay();
        
        //cameraController.SwitchToSlicing();

        playerAnim.ResetAnims();
        GameManager.Instance.dontRagdoll = true;
        //rotate towards starget
        if (!isQuickDash)
            transform.DOLookAt(new Vector3(enemyToKill.transform.position.x, transform.position.y, enemyToKill.transform.position.z), 0.2f);
        
        Vector3 swordTpPos = enemyToKill.transform.position + swordShootOffset;
        // + new Vector3(Random.Range(-0.02f, 0.02f), 1.5f);

        float distanceToEnemy = Vector3.Distance(transform.position, enemyToKill.transform.position);

        warpDuration = Mathf.Sqrt(distanceToEnemy) / 25;

        if (!bonusStarted)
        {
            sword.parent = null;
            sword.DOMove(swordTpPos, warpDuration / 0.77f);
            sword.DOLookAt(swordTpPos, .2f, AxisConstraint.None);
        }

        //tryign new tp
        Vector3 difference = enemyToKill.transform.position - transform.position;
        Vector3 directionOnly = difference.normalized;

        float distToEn = Vector3.Distance(transform.position, enemyToKill.transform.position);
        float zOffset = Mathf.Clamp(distToEn, 4f, 5);

        //tpPos = enemyToKill.transform.position + new Vector3(0, Y_OFFSET, zOffset);
        //CL only
        tpPos = enemyToKill.transform.position + new Vector3(0, Y_OFFSET, zOffset);

        var posn = enemyToKill.transform.position - (directionOnly * zOffset);

        //temp
        rb.velocity = new Vector3(0, 0, 0);

        if (!isQuickDash)
            ShowBody(false);

        //transform.DOMove(tpPos - new Vector3(0, Y_OFFSET, 0), warpDuration).SetEase(Ease.InExpo).OnComplete(() => DoneWarp());
        transform.DOMove(new Vector3(posn.x,enemyToKill.transform.position.y, posn.z), warpDuration * 2f).SetEase(Ease.InExpo).OnComplete(() => DoneWarp());

        enemyToKill.layer = 13;
        Destroy(enemyToKill.GetComponentInParent<EnemyVision>());
        Destroy(enemyToKill.GetComponentInParent<Enemy>());
        Destroy(enemyToKill.GetComponentInChildren<VisionCone>());
        Destroy(enemyToKill.GetComponentInParent<ChaserEnemy>());
        Destroy(enemyToKill.GetComponentInParent<NavMeshAgent>());
        //GameManager.Instance.UpdateGameState(GameState.Walking);

        try
        {
            enemyToKill.GetComponent<BakeMeshMe>().ChangeMesh();
        }
        catch
        {
            return;
        }
        BonusSlicer(false);
        Debug.Log("at end of warp");
    }


    void DoneWarp()
    {

        processOfWarping = false;
        if (!isQuickDash)
        {
            //CL DELETE
            //playerAnim.StrikeToHalf();
        }
        if (true)
        {
            //if level is low enough in enemy
            GameManager.Instance.UpdateGameState(GameState.Slicing);
            rb.isKinematic = true;
            playerRenderer.SetActive(false);
            sword.gameObject.SetActive(false);
        }
        else
        {
           // GameManager.Instance.UpdateGameState(GameState.Lose);
            //playerLvlManager.ChangeLvl(5, false);
            //if level is too high
            //FinishedSlice();

        }

        //temp
        transform.position = tpPos - new Vector3(0, Y_OFFSET, 0);

        GameManager.Instance.dontRagdoll = false;
        ShowBody(true);
        //rotate straight
        if (!GameManager.Instance.isBonus && !bonusStarted)
        {
            StartCoroutine(FixSword());
            SwordBackInHand();
        }
        else
        {
            ShowBody(false);
        }

        DOTween.Kill(transform);
        transform.DORotate(new Vector3(0, -180, 0), 0.45f);

        cameraController.ShakeCam();
        //careful with these, they stop the script form running
        enemyToKill.GetComponentInChildren<EnemyLvlManager>().ScaleLVLDown();
        //enemyToKill.GetComponent<Animator>().SetInteger("state", 5);
        if (!isQuickDash)
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

    }

    void SwordBackInHand()
    {
        if (swordHand == null)
            swordHand = GameObject.FindGameObjectWithTag("rightHand").transform;

        sword.parent = swordHand;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;
    }

    //WARP END

    public void ThrowForBonus()
    {
        //killsiwtch
        if (bonusStarted)
            return;
        bonusStarted = true;

        
        playerAnim.QuickSlash();
        Destroy(GameObject.Find("Slicer"));
        bonusSlicer.SetActive(true);
        StartCoroutine(ThrowDelay());
    }

    public bool FindClosestEn()
    {
        TargetScript closestEn;

        closestEn = null;
            
        foreach (TargetScript en in allEn)
        {
            try
            {
                if ((closestEn == null && en.transform.position.z + 25 < transform.position.z) || ( en.transform.position.z + 25 < transform.position.z && Vector3.Distance(transform.position, en.transform.position) < Vector3.Distance(transform.position, closestEn.transform.position)))
                {
                    if (!en.transform.parent.name.Equals("BOSS"))
                        closestEn = en;
                }
            }
            catch
            {
                continue;
            }
        }

        if (closestEn != null)
        {
            enemyToKill = closestEn.transform.gameObject;
            return true;
        }

        return false;

    }


    void ShowBody(bool state)
    {
        SkinnedMeshRenderer[] skinMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinMeshList)
        {
            smr.enabled = state;
        }
        playerLvlManager.increaseArrow.transform.parent.gameObject.SetActive(state);
    }

    public void FindSword()
    {
        if (sword == null)
        {
            sword = GameObject.FindGameObjectWithTag("sword").transform;
        }
        swordOrigRot = sword.localEulerAngles;
        swordOrigPos = sword.localPosition;
        swordMesh = sword.GetComponentInChildren<MeshRenderer>();
        swordMesh.enabled = true;
        //this is what im testing
        //TODO if you want ot test without slicer
        BonusSlicer(true, true);
    }

    private void BonusSlicer(bool state, bool reset = false)
    {
        if (bonusSlicer == null || reset)
            bonusSlicer = GameObject.Find("BonusSlicer");
        bonusSlicer.SetActive(state);
    }

    public void FinishedSlice()
    {
        if (fm == null)
            fm = FindObjectOfType<FrenzyManager>();
        fm.ResetSayne();

        rb.isKinematic = false;
        if (!bonusStarted)
            cameraController.SwitchToWalk();
        Time.timeScale = 1;
        GameManager.Instance.UpdateGameState(GameState.Walking);
        playerRenderer.SetActive(true);
        playerRenderer.transform.GetComponent<SkinnedMeshRenderer>().enabled = true;
        if (sword)
            sword.gameObject.SetActive(true);
        pm.delayTimer = 0.5f;
        BonusSlicer(true);

        foreach (TargetScript ts in FindObjectsOfType<TargetScript>())
        {
            if (ts.isAvailable && !ts.isDead)
            {
                ts.MakeAvailable();
            }
        }

    }

    IEnumerator FixSword()
    {
        yield return new WaitForSeconds(.2f);
        //cameraController.LookAtChange();
        if (swordHand == null)
            swordHand = GameObject.FindGameObjectWithTag("rightHand").transform;
        sword.parent = swordHand;
        if (!SceneManager.GetActiveScene().name.Contains("CL") && false)
        {
            sword.localPosition = swordOrigPos;
            sword.localEulerAngles = swordOrigRot;
        }
        else
        {
            sword.localPosition = new Vector3(0, 0, 0);
            sword.localEulerAngles = new Vector3(0,0,0);
        }
        yield return new WaitForSeconds(0.7f);
        //FinishedSlice();
        yield return new WaitForSeconds(2);
        while (!isQuickDash &&( GameManager.Instance.State == GameState.Killing || playerAnim.IsAttackAnimPlaying()))
        {
            yield return new WaitForSeconds(0.1f);
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    IEnumerator ThrowDelay()
    {
        transform.DOMoveX(GameObject.Find("BonusTarget").transform.position.x + 0.7f, 0.7f);
        //Transform myBonusTarget = GameObject.Find("BonusTarget").transform;
        Transform myBonusTarget = FindObjectOfType<EndingBonus>().WhichEnemyToThrowTo();
        Vector3 posToThrowTo = myBonusTarget.position + new Vector3(0,0,5);

        bool isBoss = false;
        if (myBonusTarget.GetComponents<Boss>().Length == 1)
        {
            isBoss = true;
            posToThrowTo += new Vector3(0, 0, 2);
        }

        yield return new WaitForSeconds(0.3f);

        sword.GetComponent<CoinCOllection>().bonusThrow = true;
        soundEffects.ThrowSwordPlay();
        sword.parent = null;
        float throwTime = Mathf.Log10(Vector3.Distance(transform.position, posToThrowTo) * 5 / endingBonus.roulleteMultiplier);
        throwTime = Mathf.Clamp(throwTime, 3, 10);
        //sword.DOMove(myBonusTarget.position + new Vector3(0,1,0), Mathf.Log(throwTime)).SetEase(Ease.Linear).OnComplete(() => BonusComplete());
        cameraController.ZoomOutPOVCam();
        sword.DOMoveZ(posToThrowTo.z, throwTime).SetEase(Ease.Linear).OnComplete(() => BonusComplete(isBoss));
        sword.DOLookAt(posToThrowTo + new Vector3(0, 1, 0), .2f, AxisConstraint.None);
        //bonusSlicer.transform.DORotate(new Vector3(0,360,0), 1f).SetLoops(-1, LoopType.Incremental);

        if (endingBonus.roulleteMultiplier == 7)
        {
            var part = Instantiate(pm.superSayne, sword.transform.position, new Quaternion(0, 0, 0, 0));
            part.transform.parent = sword;
            part.transform.localPosition = new Vector3(0, 0, 0);
            part.SetActive(true);
            part.transform.localScale = new Vector3(2, 2, 2);
        }
    }

    void BonusComplete(bool isBoss)
    {
        if (isBoss)
        {
            PlayerPrefs.SetInt("boss", PlayerPrefs.GetInt("boss", 0) + 1);
            FindObjectOfType<CoinCOllection>().coinsEarned += 20;
            GameManager.Instance.isBonus = false;
            Warp();
        }
        else
        {
            Destroy(sword.gameObject);
            GameManager.Instance.UpdateGameState(GameState.Victory);
        }
    }

    //temp
    IEnumerator BackupWarp()
    {
        yield return new WaitForSeconds(0.7f);
        if (!playerAnim.IsAnimationPlaying("warp"))
        {
            Warp();
        }
    }

    private void KillAllEnemiesInRadius()
    {
        float radius = Vector3.Distance(enemyToKill.transform.position, transform.position);
        List<GameObject> affectedEnemies = new List<GameObject>();
        TargetScript[] allEns = FindObjectsOfType<TargetScript>();

        //enemeis
        foreach (TargetScript ts in allEns)
        {
            if (Vector3.Distance(ts.gameObject.transform.position, transform.position) < radius)
            {
                affectedEnemies.Add(ts.gameObject);
            }
        }

        foreach (GameObject g in affectedEnemies)
        {
            g.transform.parent.GetComponentInChildren<BakeMeshMe>().ChangeMesh();
            g.transform.parent.GetComponentInChildren<KnifeSliceableAsync>().PmCallsSlice();
            g.transform.parent.GetComponent<FallDownRb>().DropImmediatly(0.001f, true);
        }
    }

    private void KillEveryPawnInRadius()
    {
        float radius = Vector3.Distance(enemyToKill.transform.position, transform.position);
        GameObject[] allPawns = GameObject.FindGameObjectsWithTag("pawnParent");
        GameObject[] allObstacles = GameObject.FindGameObjectsWithTag("bigHurt");
        List<GameObject> affectedPawns = new List<GameObject>();
        List<GameObject> affectedObstacles = new List<GameObject>();

        //pawns
        foreach (GameObject g in allPawns)
        {
            if (Vector3.Distance(g.transform.position, transform.position) < radius)
            {
                affectedPawns.Add(g);
            }
        }

        foreach (GameObject g in affectedPawns)
        {
            g.transform.GetComponentInChildren<PushedToSide>().PmCallsSlice();
            playerLvlManager.ChangeLvl(1);
        }

        //obstacles
        foreach (GameObject g in allObstacles)
        {
            if (Vector3.Distance(g.transform.position, transform.position) < radius)
            {
                affectedObstacles.Add(g);
            }
        }

        foreach (GameObject g in affectedObstacles)
        {
            Destroy(g);
        }
    }

}
