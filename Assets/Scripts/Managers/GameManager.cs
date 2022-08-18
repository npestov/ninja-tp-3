using System;
using System.Collections;
using System.Collections.Generic;
using Tabtale.TTPlugins;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instances
    private CameraController cameraController;
    private PlayerMovement playerMovement;
    private AttackMoveController attackMoveController;
    private TimeManager timeManager;
    private ThirdPersonInput thirdPersonInput;
    private InGameUI inGameUI;
    private PlayerAnim playerAnim;
    private WinScreenUI winScreenUI;
    private LossScreenUI lossScreenUI;
    private MenuUI menuUI;
    private ClickDetection clickDetection;
    private PlayerLvlManager playerLvlManager;

    public bool levelStarted;
    public bool isBonus;
    public bool dontRagdoll;
    private bool isStarting = false;
    public bool enemyHasWon; 

    //GM
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;

    //tut
    GameObject tut;

    private void Awake()
    {
        
        TTPCore.Setup();
        var parameters = new Dictionary<string, object>();
        parameters.Add("mission number", "level: " + PlayerPrefs.GetInt("RealLvl").ToString());
        TTPGameProgression.FirebaseEvents.MissionStarted(PlayerPrefs.GetInt("RealLvl"), parameters);


        
        Instance = this;
        playerLvlManager = FindObjectOfType<PlayerLvlManager>();
        clickDetection = FindObjectOfType<ClickDetection>();
        menuUI = FindObjectOfType<MenuUI>();
        lossScreenUI = FindObjectOfType<LossScreenUI>();
        winScreenUI = FindObjectOfType<WinScreenUI>();
        playerAnim = FindObjectOfType<PlayerAnim>();
        cameraController = FindObjectOfType<CameraController>();
        attackMoveController = FindObjectOfType<AttackMoveController>();
        timeManager = FindObjectOfType<TimeManager>();
        thirdPersonInput = FindObjectOfType<ThirdPersonInput>();
        inGameUI = FindObjectOfType<InGameUI>();
        playerMovement = FindObjectOfType<PlayerMovement>();

        tut = GameObject.Find("SliceTut");
    }

    private void Start()
    {
        UpdateGameState(GameState.Menu);
    }


    private void Update()
    {
        CheckForStartingTouch();
    }

    public void UpdateGameState(GameState newState)
    {
        Debug.Log("MY NEW STATE IS: " + newState);

        switch (newState)
        {
            case GameState.Walking:
                if (tut != null)
                    tut.transform.GetChild(0).gameObject.SetActive(false);
                timeManager.RemoveSlowMotion();
                break;
            case GameState.Aiming:
                break;
            case GameState.Slicing:
                cameraController.SwitchToSlicing();
                if (tut != null)
                    tut.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case GameState.Killing:
                if (State == GameState.Killing) return;

                if (attackMoveController.enemyToKill.transform.parent.GetComponents<EnemyLvlManager>().Length == 1 && attackMoveController.enemyToKill.transform.parent.GetComponent<EnemyLvlManager>().isKillable)
                    playerLvlManager.PutLVLTextHigher();
                timeManager.RemoveSlowMotion();
                //cameraController.SwitchToWalk();
                break;
            case GameState.QuickKilling:
                break;
            case GameState.Victory:
                if (State == GameState.Victory)
                    break;
                
                var parameters = new Dictionary<string, object>();
                parameters.Add("mission number", "level: " + PlayerPrefs.GetInt("RealLvl").ToString());
                TTPGameProgression.FirebaseEvents.MissionComplete(parameters);

                //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, PlayerPrefs.GetInt("RealLvl").ToString());
                inGameUI.DisableInGameUI();
                inGameUI.ShowMultiplier();
                //cameraController.SwitchToWalk();
                playerAnim.Idle();
                GameObject.Find("BOSS").GetComponent<FallDownRb>().DropRb(0.1f);
                StartCoroutine(winScreenUI.DisplayWinScreen());
                break;
            case GameState.Lose:
                if (State == GameState.Lose)
                    break;
                //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, PlayerPrefs.GetInt("RealLvl").ToString());
                Debug.Log("LOSS");
                var parameters2 = new Dictionary<string, object>();
                parameters2.Add("mission number", "level: " + PlayerPrefs.GetInt("RealLvl").ToString());
                TTPGameProgression.FirebaseEvents.MissionFailed(parameters2);
                
                if (enemyHasWon)
                {
                    StartCoroutine(lossScreenUI.DislayFinishFail());
                }
                else
                {
                    playerAnim.Die();
                    StartCoroutine(lossScreenUI.DisplayLossScreen());
                }
                playerAnim.Idle();
                timeManager.RemoveSlowMotion();
                inGameUI.DisableInGameUI();
                break;
        }

        State = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public bool AreEnemiesDead()
    {
        if (GameObject.FindGameObjectsWithTag("Target").Length == 0)
        {
            GameManager.Instance.UpdateGameState(GameState.Victory);
            return true;
        }
        return false;
    }

    private void CheckForStartingTouch()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            if (State == GameState.Menu && !isStarting)
            {
                if (!clickDetection.IsClickOverUI() && GameObject.FindObjectsOfType<MainShop>().Length == 0)
                {
                    //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, PlayerPrefs.GetInt("RealLvl").ToString());
                    cameraController.DisableMenuCam();
                    menuUI.RemoveMenu();
                    isStarting = true;
                    playerAnim.Run();
                    levelStarted = true;
                    State = GameState.Walking;
                    playerMovement.runningState = RunningState.RUNNING;
                    timeManager.RemoveSlowMotion();
                    attackMoveController.FindSword();

                    inGameUI.EnableInGameUI();
                    if (FindObjectsOfType<Tutorial>().Length == 1)
                        FindObjectOfType<Tutorial>().Aim(true);
                }
            }
        }
        
    }

    public void FinishedTheTrack()
    {
        isBonus = true;
        inGameUI.MoveEndingCursor();
    }

    public void CursorClickedCallback()
    {
        attackMoveController.ThrowForBonus();
        cameraController.SwitchToSwordCam();
        timeManager.DoBonusSlowMotion();
    }
}

public enum GameState
{
    Menu,
    Walking,
    Aiming,
    Killing,
    Slicing,
    QuickKilling,
    Victory,
    Lose
}
