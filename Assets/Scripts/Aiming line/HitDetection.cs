using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public bool targetSelected;
    private AttackMoveController attackMoveController;
    private PlayerAnim anim;
    private DrawTrajectory drawTrajectory;

    private Tutorial tutorial;

    private void Awake()
    {
        tutorial = FindObjectOfType<Tutorial>();
        attackMoveController = FindObjectOfType<AttackMoveController>();
        anim = attackMoveController.gameObject.GetComponent<PlayerAnim>();
        drawTrajectory = FindObjectOfType<DrawTrajectory>();
    }

    public void CheckIfHitEnemy(RaycastHit hit)
    {
        if (hit.transform.gameObject.GetComponentInChildren<TargetScript>() != null && !hit.transform.gameObject.CompareTag("bonus"))
        {
            foreach (TargetScript enemy in FindObjectsOfType<TargetScript>())
            {
                if (enemy.gameObject.layer != 7 && enemy.gameObject.layer != 12)
                    continue;
                enemy.UnHighlight();
            }
            //its an enemy
            if (hit.transform.tag == "Target" && hit.transform.gameObject.layer == 7)
            {
                attackMoveController.isRunnerSelected = false;
            }
            //itds a runner
            if (hit.transform.tag == "Runner")
            {
                attackMoveController.isRunnerSelected = true;
            }
            if (hit.transform.gameObject.GetComponentsInChildren<TargetScript>().Length != 0)
            {
                hit.transform.gameObject.GetComponentInChildren<TargetScript>().HighLight();
                if (tutorial != null)
                    tutorial.Release(true);
            }
            targetSelected = true;
            attackMoveController.enemyToKill = hit.transform.gameObject;
            KillIfPossible();
        }
        else if (attackMoveController.enemyToKill && attackMoveController.isRunnerSelected)
        {
            //uncomment to remove runner from targetting once u dont aim at him
            //attackMoveController.enemyToKill.GetComponentInChildren<TargetScript>().UnHighlight();
            //attackMoveController.enemyToKill = null;
            Debug.Log("gimpy fleix");
        }
        else
        {
            Debug.Log("gimpy fleix");
        }
    }

    public void KillIfPossible(bool gauranteed = false)
    {
        //target selected removed from seocnd below
        if (gauranteed || (attackMoveController.enemyToKill != null))
        {
            //attackMoveController.enemyToKill.transform.Find("rootChrSlr").GetChild(0).gameObject.SetActive(true);
            if (!attackMoveController.isRunnerSelected)
            {
                GameManager.Instance.UpdateGameState(GameState.Killing);
                anim.ResetAnims();
                anim.StopAll();
                attackMoveController.WarpKill();
                targetSelected = false;
                attackMoveController.timesTp++;
                drawTrajectory.HideLine();
                Debug.Log("Kill if possivble succesuflly calls warpKill");
            }
        }
        else
        {
            GameManager.Instance.UpdateGameState(GameState.Walking);
        }
    }
}
