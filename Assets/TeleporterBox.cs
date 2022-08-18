using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterBox : MonoBehaviour
{
    public GameObject enemyToKill;
    // Start is called before the first frame update
    void Start()
    {
        if (enemyToKill != null && enemyToKill.GetComponents<TargetScript>().Length == 0)
        {
            enemyToKill = enemyToKill.GetComponentInChildren<TargetScript>().gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
