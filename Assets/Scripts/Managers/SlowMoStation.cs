using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoStation : MonoBehaviour
{
    GameObject player;
    PlayerAnim playerAnim;
    bool slowMoDone;
    TimeManager timeManager;
    // Start is called before the first frame update
    void Start()
    {
        playerAnim = FindObjectOfType<PlayerAnim>();
        timeManager = GameObject.FindObjectOfType<TimeManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.z < transform.position.z + 5 && GameManager.Instance.State == GameState.Walking && player.transform.position.z - transform.position.z + 5 > 0 && !playerAnim.IsAttackAnimPlaying())
        {
            Time.timeScale = 0.1f;
            slowMoDone = true;
            Debug.Log("constantly hitting" + (player.transform.position.z - transform.position.z + 5));
        }
        else if (slowMoDone)
        {
            Time.timeScale = 1;
            timeManager.RemoveSlowMotion();
            Destroy(transform.gameObject);
        }

    }
}
