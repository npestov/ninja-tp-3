using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCOllection : MonoBehaviour
{
    public int coinsEarned;
    private AttackMoveController attackMoveController;
    public float bonusMultiplier = 1;
    public bool bonusThrow;

    private Vector3 starMousePos;
    private Vector3 currentMousePos;

    private float bonusLowerClamp;
    bool firstUpdateHit;

    // Start is called before the first frame update
    void Awake()
    {
        attackMoveController = FindObjectOfType<AttackMoveController>();
        bonusLowerClamp = FindObjectOfType<EndingBonus>().transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bonusThrow)
        {
            if (!firstUpdateHit)
            {
                starMousePos = Input.mousePosition;
                firstUpdateHit = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                starMousePos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                currentMousePos = Input.mousePosition;
                Vector3 adjustment = Vector2.zero;
                Vector3 newRot = Vector2.zero;

                //new rot
                adjustment.y += currentMousePos.y - starMousePos.y;
                newRot.z = currentMousePos.y - starMousePos.y;

                //new pos
                Vector3 newPos = transform.position + (adjustment * Time.deltaTime * 0.5f);
                newPos.y = Mathf.Clamp(newPos.y, bonusLowerClamp, bonusLowerClamp + 12);
                Debug.Log(newPos);
                //actual change pos
                transform.position = newPos;

                //actually rotate
                transform.GetChild(0).eulerAngles -= newRot * Time.deltaTime;
                starMousePos = currentMousePos;

                //minus rot
                //transform.GetChild(0).eulerAngles -= 1;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("coin"))
        {
            coinsEarned += 3;
            Destroy(other.gameObject);
        }


        if (other.gameObject.CompareTag("bonus"))
        {

            //old bonus target
            /*
            attackMoveController.enemyToKill = null;
            attackMoveController.KillSwordTween();
            if (float.Parse(other.gameObject.name) > bonusMultiplier)
            {
                bonusMultiplier = float.Parse(other.gameObject.name);
            }
            */
        }

    }
}
