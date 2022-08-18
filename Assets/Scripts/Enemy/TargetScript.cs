using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    public Material highlightedColor;
    private Material usualColor;
    public Material availableColor;
    private PlayerVision playerVision;
    private EnemyGun enemyGun;
    public bool isDead = false;
    public bool isAvailable;
    private float distanceToBeAvailable;
    private GameObject player;
    private CameraController cameraController;
    private TimeManager timeManager;
    bool queMakeAval = false;

    public bool CreativeOnly;

    //temp
    public bool passedOver;

    //temp
    private bool slowdownShown;


    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        cameraController = FindObjectOfType<CameraController>();
        player = GameObject.FindWithTag("Player");
        distanceToBeAvailable = 50;
        playerVision = FindObjectOfType<PlayerVision>();
        enemyGun = transform.parent.GetComponentInChildren<EnemyGun>();
        skinnedMeshRenderer = transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();
        usualColor = skinnedMeshRenderer.material;

    }

    private void Update()
    {
        if (queMakeAval && GameManager.Instance.State != GameState.Slicing)
        {
            queMakeAval = false;
            MakeAvailable();
        }
        if (!isAvailable && Vector3.Distance(transform.position, player.transform.position) < distanceToBeAvailable)
        {
            if (GameManager.Instance.State == GameState.Slicing)
                queMakeAval = true;
            else
                MakeAvailable();
        }

        if (player.transform.position.z < transform.position.z + 15 && isAvailable && !isDead)
        {
            if (!slowdownShown && GameManager.Instance.State == GameState.Walking && player.transform.position.z > transform.position.z + 7)
            {
                slowdownShown = true;
                timeManager.DoSlowmotion();
            }
            if (player.transform.position.z < transform.position.z + 5 && GameManager.Instance.State != GameState.Slicing)
            {
                timeManager.RemoveSlowMotion();
                //Destroy(this);
                passedOver = true;
                //Destroy(transform.parent.gameObject, 2);
                cameraController.LookAtChange();
            }
        }
    }

    public void AddTarget()
    {
        if (!playerVision.screenTargets.Contains(transform))
            playerVision.screenTargets.Add(transform);
    }

    public void RemoveTarget()
    {
        if (playerVision.screenTargets.Contains(transform))
            playerVision.screenTargets.Remove(transform);
    }

    public void MakeAvailable()
    {
        if (isDead )
            return;
       // if (skinnedMeshRenderer != null && !CreativeOnly)
            //skinnedMeshRenderer.material.SetFloat("_OutlineWidth", 1);
        //if (showArrow)
            //arrowObject.SetActive(true);
    }

    public void HighLight()
    {
        //if (skinnedMeshRenderer != null && !CreativeOnly)
            //skinnedMeshRenderer.material.SetFloat("_OutlineWidth", 5);
    }

    public void RevertBack()
    {
       // if (!CreativeOnly)
         //GetComponent<MeshRenderer>().material.SetFloat("_OutlineWidth", 1);
    }

    public void UnHighlight()
    {
        //if (skinnedMeshRenderer != null && !CreativeOnly)
            //skinnedMeshRenderer.material.SetFloat("_OutlineWidth", 1f);
    }

    public void DeadHighlight()
    {
        isDead = true;
    }

    public void DeleteEnemy()
    {
        /*
        //CreatePlaceholder();
        isAvailable = false;
        isDead = true;
        StartCoroutine(KillMyself());
        timeManager.RemoveSlowMotion();
        */
    }
    private void CreatePlaceholder()
    {
        gameObject.tag = "Untagged";
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.layer = 11;
        cube.GetComponent<MeshRenderer>().enabled = false;
        cube.GetComponent<Collider>().enabled = false;
        cube.transform.position = transform.position;
        cube.tag = "Target";
    }


    IEnumerator KillMyself()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject.transform.parent.gameObject);
    }
}
