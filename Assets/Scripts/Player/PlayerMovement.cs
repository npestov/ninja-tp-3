using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using BzKovSoft.ObjectSlicerSamples;
using MoreMountains.NiceVibrations;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    private float originalPlayerSpeed;
    private PlayerAnim anim;
    public bool stopRunning;

    PlayerLvlManager playerLvlManager;
    FrenzyManager frenzyManager;

    Vector3 touchPosition;

    private float timer;
    private float jumpDelay = 0.5f;
    float originalMass;
    bool jumping = false;

    //touching wall
    bool touchingLeft = false;
    bool touchingRight = false;

    float sideFactor = 500;

    AttackMoveController ac;
    HitDetection hd;

    //Side to side
    [SerializeField]
    float sideSpeed;
    [SerializeField]
    float jumpForce;
    [SerializeField]
    float jumpOffWallForce;
    [SerializeField]
    float jumpOffWallSideForce;
    [SerializeField]
    float jumpForwardFoce;
    [SerializeField]
    float runningUpWallSpeed;
    [HideInInspector]
    public float delayTimer;

    [Space]

    [SerializeField]
    float jumpBoosterForce;
    Vector3 lastClickPos;
    float turnIndex = 0.5f;
    Rigidbody rb;

    public RunningState runningState = RunningState.NONE;

    GameObject myFloor;
    float offsetClamp = 3.2f;

    public GameObject playersFloorCenter;
    public GameObject superSayne;
    SoundEffects soundEffects;

    public float pawnSliderIncrease = 0.025f;

    // Start is called before the first frame update
    void Awake()
    {
        anim = FindObjectOfType<PlayerAnim>();
        rb = GetComponent<Rigidbody>();
        ac = FindObjectOfType<AttackMoveController>();
        playerLvlManager = FindObjectOfType<PlayerLvlManager>();
        frenzyManager = FindObjectOfType<FrenzyManager>();
        hd = FindObjectOfType<HitDetection>();
        originalPlayerSpeed = playerSpeed;
        playersFloorCenter = GameObject.Find("FloorCentral");
        soundEffects = GetComponent<SoundEffects>();
    }

    private void Start()
    {
        originalMass = rb.mass;
        //PlayerPrefs.SetInt("coins", 10000);
        //PlayerPrefs.SetInt("RealLvl", 40);
        //Debug.Log("MY PATH: "+Application.persistentDataPath);
        //transform.position = new Vector3(myFloor.transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
            lastClickPos = Input.mousePosition;
        if ((GameManager.Instance.State == GameState.Walking || GameManager.Instance.State == GameState.Killing) && !stopRunning)
            Move();
    }



    private void Update()
    {
        anim.Turn(turnIndex);


        if (runningState == RunningState.UP_WALL_RUNNING)
            transform.position += new Vector3(0, runningUpWallSpeed * Time.deltaTime, 0);
    

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            //reset speed
            playerSpeed = originalPlayerSpeed;
            //superSayne.SetActive(false);
        }

        delayTimer -= Time.deltaTime;
    }

    private void UpdateRunnerState(RunningState state)
    {
        if (GameManager.Instance.State == GameState.Killing || anim.IsAttackAnimPlaying())
            return;
        anim.ResetAnims();
        //uncomment if using running up wall
        //rb.constraints = RigidbodyConstraints.FreezeRotation;
        switch (state)
        {
            case RunningState.RUNNING:
                if (runningState == RunningState.RUNNING)
                    return;
                rb.useGravity = true;
                if (!stopRunning)
                    anim.Run();
                break;
            case RunningState.LEFT_WALL_RUNNING:
                anim.WallRun(true);
                break;
            case RunningState.RIGHT_WALL_RUNNING:
                anim.WallRun(false);
                break;
            case RunningState.UP_WALL_RUNNING:
                if (runningState == RunningState.UP_WALL_RUNNING) return;
                rb.velocity = new Vector3(0,0,0);
                rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                anim.UpWallRun();
                rb.useGravity = false;
                break;
            case RunningState.JUMPING:
                if (runningState == RunningState.JUMPING) return;
                //rem
                //StartCoroutine(IncreaseGravity());
                if (runningState == RunningState.RIGHT_WALL_RUNNING)
                {
                    Jump(jumpOffWallForce);
                    //rb.AddForce(-transform.right * jumpOffWallSideForce, ForceMode.Impulse);
                    anim.SideJump();
                }
                else if (runningState == RunningState.LEFT_WALL_RUNNING)
                {
                    Jump(jumpOffWallForce);
                    //rb.AddForce(transform.right * jumpOffWallSideForce, ForceMode.Impulse);
                    anim.SideJump();
                }
                else
                {
                    anim.RegularJump();
                }

                rb.useGravity = true;
                break;
            case RunningState.SLIDE:

                break;
            case RunningState.VAULT:
                anim.Vault();
                break;
            case RunningState.LAND:
                anim.Land();
                break;
        }
        runningState = state;
    }

    private void LateUpdate()
    {
        if (myFloor != null)
        {
            var clampedOffset = myFloor.transform.position.x - transform.position.x;
            clampedOffset = Mathf.Clamp(clampedOffset, -2f, 2f);
            playersFloorCenter.transform.position = new Vector3(myFloor.transform.position.x - clampedOffset, transform.position.y, transform.position.z);
        }
        else
            playersFloorCenter.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

    }
    //COLLISION START

    private void OnCollisionEnter(Collision collision)
    {
        /*
        if (collision.gameObject.CompareTag("floor") && GameManager.Instance.State != GameState.Menu && runningState != RunningState.VAULT)
        {
            UpdateRunnerState(RunningState.LAND);
        }

        if (GameManager.Instance.State != GameState.Walking)
            return;
        */

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.UpdateGameState(GameState.Lose);
        }

    }
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            if (jumping && !stopRunning)
            {
                jumping = false;
                anim.Run();
            }
            myFloor = collision.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            jumping = true;
            myFloor = null;
            if (GameManager.Instance.State == GameState.Walking &&  runningState != RunningState.UP_WALL_RUNNING && runningState != RunningState.VAULT)
            {
                Jump(jumpForce);
                UpdateRunnerState(RunningState.JUMPING);
            }
        }
        if (collision.gameObject.CompareTag("runupwall"))
        {
            collision.gameObject.tag = "Untagged";
            UpdateRunnerState(RunningState.LAND);
            rb.useGravity = true;
        }

    }
    //COLLISIONS END
    //TRIGGERS
    
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("rightwall"))
        {
            touchingRight = false;
        }
        if (collision.gameObject.CompareTag("leftwall"))
        {
            touchingLeft = false;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //collectibles
        if (collision.gameObject.GetComponents<TeleporterBox>().Length == 1)
        {
            if (GameManager.Instance.State == GameState.Killing || anim.IsAnimationPlaying("warp"))
                return;
            ac.enemyToKill = collision.gameObject.GetComponent<TeleporterBox>().enemyToKill;
            hd.KillIfPossible(true);
            Destroy(collision.gameObject.GetComponent<TeleporterBox>());
            Jump(jumpForce);
        }
        if (collision.gameObject.CompareTag("speedBoost"))
        {
            playerSpeed = 14;
            timer = 3;
        }

        if (collision.gameObject.CompareTag("boost"))
        {
            MMVibrationManager.Haptic(HapticTypes.Selection);
            Destroy(collision.gameObject);
            playerLvlManager.ChangeLvl(1, true, 0.05f);
        }
        if (collision.gameObject.CompareTag("hurt"))
        {
            Destroy(collision.gameObject);
            playerLvlManager.ChangeLvl(1, false, 0.05f);
        }
        if (collision.gameObject.CompareTag("bigHurt"))
        {
            Destroy(collision.gameObject);
            if (GameManager.Instance.State == GameState.Killing || anim.IsAnimationPlaying("warp") || ac.processOfWarping)
                return;
            GetHurt();
        }
        if (collision.gameObject.CompareTag("slide"))
        {
            if (GameManager.Instance.State == GameState.Killing || anim.IsAnimationPlaying("warp") || ac.processOfWarping)
                return;
            anim.Slide();
            timer = 1.5f;
            playerSpeed = 14;
        }
        if (collision.gameObject.CompareTag("jumpbooster"))
        {
            if (GameManager.Instance.State == GameState.Killing || anim.IsAnimationPlaying("warp") || ac.processOfWarping)
                return;
            Jump(28);
            //timer = 1.5f;
            //playerSpeed = 14;
        }

        if (collision.gameObject.CompareTag("pawn"))
        {

            if (collision.gameObject.name.Contains("trigger"))
            {
                MMVibrationManager.Haptic(HapticTypes.Selection);
                Destroy(collision.gameObject);
                soundEffects.Pop();
            }
            if (frenzyManager == null)
            {
                frenzyManager = FindObjectOfType<FrenzyManager>();
            }

            frenzyManager.IncreaseSlider(pawnSliderIncrease);

            collision.transform.parent.GetComponentInChildren<PushedToSide>().PmCallsSlice();
            playerLvlManager.IncreaseLevelInstantly(1);

            if (GameManager.Instance.State == GameState.Killing || anim.IsAnimationPlaying("warp") || ac.processOfWarping)
                return;

            anim.ResetAnims();
            anim.StrikeToHalf();
            //collision.transform.parent.GetComponentInChildren<KnifeSliceableAsync>().pm = this;
        }

        if (collision.gameObject.CompareTag("enemyTrigger") )
        {
            try
            {
                if (collision.transform.parent.GetComponentInChildren<TargetScript>().gameObject == ac.enemyToKill)
                    return;
            }
            catch
            {
                return;
            }

            MMVibrationManager.Haptic(HapticTypes.Selection);

            //too high level, hurt me
            if (!collision.transform.GetComponentInParent<EnemyLvlManager>().CheckLVLState())
            {
                GetHurt();
            }
            //high eneought level, kill en
            else
            {
                soundEffects.Pop();
                if (frenzyManager == null)
                {
                    frenzyManager = FindObjectOfType<FrenzyManager>();
                }
                frenzyManager.IncreaseSlider(0.1f);
                collision.transform.parent.GetComponentInChildren<BakeMeshMe>().ChangeMesh();
                collision.transform.parent.GetComponentInChildren<KnifeSliceableAsync>().PmCallsSlice();
                collision.transform.parent.GetComponent<FallDownRb>().DropImmediatly(0.001f, true);
                if (GameManager.Instance.State != GameState.Killing && !anim.IsAnimationPlaying("warp") && !ac.processOfWarping)
                {
                    anim.ResetAnims();
                    anim.StrikeToHalf();
                }
                playerLvlManager.IncreaseLevelInstantly(5);
            }
            Destroy(collision.gameObject);
        }

            //BONUS TRIGGERS
        if (collision.gameObject.CompareTag("bonusWarning"))
        {
            timer = 100;
            playerSpeed = 1;
            anim.DecreaseRunSpeed();
            GameManager.Instance.FinishedTheTrack();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("finish"))
        {
            anim.ResetAnims();
            anim.Idle();
            Destroy(collision.gameObject);
            rb.velocity = new Vector3(0, 0, 0);
            stopRunning = true;
        }

    }
    //TRIGGERS END

    private void GetHurt()
    {
        if (GameManager.Instance.State == GameState.Killing || anim.IsAnimationPlaying("warp") || ac.processOfWarping)
            return;

        if (frenzyManager == null)
        {
            frenzyManager = FindObjectOfType<FrenzyManager>();
        }

        playerLvlManager.IncreaseLevelInstantly(10, false);
        //frenzyManager.IncreaseSlider(-1f);
        anim.ResetAnims();
        timer = 0.6f;
        playerSpeed = 5;
        transform.DOMove(transform.position + new Vector3(0, 0, 3), 0.2f);
    }

    private void Jump(float force)
    {
        Debug.Log("jumped");
        if (timer < 0)
        {
            anim.ResetAnims();
            rb.AddForce(transform.up * force, ForceMode.Impulse);
            rb.AddForce(transform.forward * jumpForwardFoce, ForceMode.Impulse);
            timer = jumpDelay;
            playerSpeed = 18;
            jumping = true;
        }
    }

    public void SuperSayne(float time = 2)
    {
        playerSpeed = 15;
        timer = time;
        if (ac.FindClosestEn())
        {
            Debug.Log("Find closest enemy returns true");
            hd.KillIfPossible();

        }

        Debug.Log("SUPER SAYNE CALLED in pm");
    }

    public void IncreaseSpeed()
    {
        playerSpeed += 0.002f;
    }

    void Move()
    {
        //Move less if killing
        Vector3 deltaPosition = new Vector3(0,0,0);
        if (runningState != RunningState.UP_WALL_RUNNING)
        {
            if (GameManager.Instance.State == GameState.Killing)
                deltaPosition = new Vector3(0, 0, -1) * playerSpeed / 1.2f;
            else
                deltaPosition = new Vector3(0, 0, -1) * playerSpeed;
        }

        //SIDE TO SIDE START, comment this
        
        touchPosition = Input.mousePosition;
        //left
        if (Input.GetMouseButton(0) && (GameManager.Instance.State != GameState.Killing || anim.IsAnimationPlaying("warp")) && delayTimer < 0)
        {
            var preDelta = deltaPosition;

            deltaPosition += transform.right * sideSpeed * ((lastClickPos.x - touchPosition.x) / Screen.width);
            if ((deltaPosition.x > preDelta.x && touchingLeft) || (deltaPosition.x < preDelta.x && touchingRight))
            {
                deltaPosition = Vector3.zero;
            }
            lastClickPos = Input.mousePosition;

            //if (turnIndex > 0.35f && turnIndex < 0.65f)
            deltaPosition.x = Mathf.Clamp(deltaPosition.x, -80f, 80f);
            turnIndex -= 150 * Time.deltaTime * deltaPosition.x / Screen.width;
            //if (turnIndex < 0.65f)
            //turnIndex -= 100 * Time.deltaTime * deltaPosition.x / Screen.width;

        }

        if (turnIndex > 0.5)
        {
            turnIndex -= 1f * Time.deltaTime;
        }
        if (turnIndex < 0.5)
        {
            turnIndex += 1f * Time.deltaTime;
        }

        //Debug.Log("Delt: " + deltaPosition);
        deltaPosition.x = Mathf.Clamp(deltaPosition.x, -50f, 50f);
        var newTransfrom = transform.position + deltaPosition * Time.deltaTime;
        if (myFloor != null)
            newTransfrom.x = Mathf.Clamp(newTransfrom.x, myFloor.transform.position.x - offsetClamp, myFloor.transform.position.x + offsetClamp);
        transform.position = newTransfrom;
    }

    private IEnumerator IncreaseGravity()
    {
        yield return new WaitForSeconds(0.2f);
        while (runningState == RunningState.JUMPING)
        {
            yield return new WaitForSeconds(0.1f);
            rb.mass += 0.5f;
        }
        rb.mass = originalMass;
    }

    public void ChangeToRun()
    {
        Debug.Log("update to run");
        UpdateRunnerState(RunningState.RUNNING);
    }
}

public enum RunningState
{
    NONE,
    RUNNING,
    LEFT_WALL_RUNNING,
    RIGHT_WALL_RUNNING,
    UP_WALL_RUNNING,
    JUMPING,
    VAULT,
    SLIDE,
    LAND
}
