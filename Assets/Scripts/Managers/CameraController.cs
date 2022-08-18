using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera walkingCamera;
    [SerializeField]
    private CinemachineVirtualCamera aimingCamera;
    [SerializeField]
    private CinemachineVirtualCamera menuCamera;
    [SerializeField]
    private CinemachineVirtualCamera swordCam;
    [SerializeField]
    private CinemachineVirtualCamera swordFollowWarp;
    private CinemachineVirtualCamera slicingcam;

    private CinemachineImpulseSource impulse;
    private CinemachineImpulseSource impulseWalk;
    private GameObject player;
    public GameObject floorCentral;
    private PlayerLvlManager playerLvlManager;



    // Start is called before the first frame update
    void Start()
    {
        slicingcam = GameObject.FindGameObjectWithTag("slicingCam").GetComponent<CinemachineVirtualCamera>();
        player = GameObject.FindGameObjectWithTag("Player");
        impulseWalk = walkingCamera.GetComponent<CinemachineImpulseSource>();
        impulse = aimingCamera.GetComponent<CinemachineImpulseSource>();
        playerLvlManager = FindObjectOfType<PlayerLvlManager>();
        walkingCamera.m_LookAt = floorCentral.transform;
        walkingCamera.m_Follow = floorCentral.transform;
        //walkingCamera.m_LookAt = player.transform;
        //walkingCamera.m_Follow = player.transform;
        aimingCamera.m_Follow = floorCentral.transform;
        FindSword();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SwitchToWalk()
    {
        walkingCamera.Priority = 1;
        aimingCamera.Priority = 0;
        slicingcam.Priority = 0;
        slicingcam.gameObject.SetActive(false);
        Debug.Log("swithced to ewlaking");
    }
    public void SwitchToAim()
    {
        
        walkingCamera.Priority = 0;
        aimingCamera.Priority = 1;
        slicingcam.Priority = 0;
        slicingcam.gameObject.SetActive(false);
        aimingCamera.gameObject.SetActive(false);
        aimingCamera.gameObject.SetActive(true);
        
    }

    public void SwitchToSlicing()
    {
        walkingCamera.Priority = 0;
        aimingCamera.Priority = 0;
        slicingcam.Priority = 4;
        slicingcam.gameObject.SetActive(true);
    }

    public void ShakeCam()
    {
        if (walkingCamera.Priority == 1)
        {
            impulseWalk.GenerateImpulse(Vector3.right);
        }
        else
            impulse.GenerateImpulse(Vector3.right);
    }
    public void SwitchToSwordCam()
    {
        swordCam.Priority = 2;
    }

    public void SwitchToFollowWarpCam()
    {
        swordFollowWarp.Priority = 3;
    }

    public void DisableMenuCam()
    {
        menuCamera.Priority = -1;
    }

    public void LookAtChange()
    {
        /*
        if (GameManager.Instance.State == GameState.Killing)
            return;
        SwitchToWalk();
        TargetScript[] allTargets = FindObjectsOfType<TargetScript>();
        Transform closestTarget = null;
        for (int i = 0; i < allTargets.Length; i++)
        {
            if (allTargets[i].GetComponentInParent<EnemyLvlManager>().LVL > playerLvlManager.playerLvl)
                continue;

            if (closestTarget == null && allTargets[i].isAvailable && !allTargets[i].passedOver)
            {
                closestTarget = allTargets[i].transform;
                continue;
            }
            if (!allTargets[i].passedOver && allTargets[i].isAvailable && Vector3.Distance(allTargets[i].transform.position, player.transform.position) < Vector3.Distance(closestTarget.transform.position, player.transform.position))
            {
                closestTarget = allTargets[i].transform;
            }
        }
        */
    }

    public void FindSword()
    {
        StartCoroutine(DelayFindSword());
    }

    public void ZoomOutPOVCam()
    {
        Debug.Log("Zoom out the fooking cam is called");
        slicingcam.transform.localPosition += new Vector3(0, 12, -12.9f);
    }

    IEnumerator DelayFindSword()
    {
        yield return new WaitForSeconds(0.3f);
        swordCam.m_LookAt = GameObject.Find("swordHoldPoint").transform;
        swordCam.m_Follow = GameObject.Find("swordHoldPoint").transform;
    }
}
