using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class FallDownRb : MonoBehaviour
{
    bool fallen;
    int counter;
    int targetCount = 1;
    GameObject weapon;
    AttackMoveController amc;

    bool weaponDropped;

    public bool dropImmediatly;

    // Start is called before the first frame update
    void Start()
    {
        if (!gameObject.CompareTag("bonus") && !transform.GetChild(0).CompareTag("pawn"))
            weapon = Helpers.FindComponentInChildWithTag<Transform>(transform.gameObject, "weapon").gameObject;
        amc = FindObjectOfType<AttackMoveController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropRb(float time)
    {
        if (counter == 0 && !gameObject.CompareTag("bonus") && !gameObject.CompareTag("pawn"))
        {
            try
            {
                var rb = weapon.AddComponent<Rigidbody>();
            }
            catch {

            }
        }

        counter++;
        if (counter >= targetCount || dropImmediatly)
        {
            var timer = 0.2f;

            if (!dropImmediatly && !fallen)
            {
                StartCoroutine(DropAllRbDelay(timer));
                
            }
            else if (!fallen)
            {
                StartCoroutine(DropAllRbDelay(0.01f));
            }
        }
        /*
        if (startedTimer)
            return;
        */

        //startedTimer = true;
        //StartCoroutine(DropAllRbDelay(time));
    }

    public void DropImmediatly(float timer = 0.2f, bool easySliced = false)
    {
        if (fallen)
            return;

        StartCoroutine(DropAllRbDelay(timer, easySliced));
        //StartCoroutine(DropAllRbDelay(0.01f));
        Destroy(transform.GetComponentInChildren<Canvas>().gameObject);
    }

    private IEnumerator DropAllRbDelay(float time, bool easySliced = false)
    {
        fallen = true;
        if (!dropImmediatly)
            FindObjectOfType<PlayerLvlManager>().ChangeLvl(GetComponent<EnemyLvlManager>().lvlBoost, true);

        yield return new WaitForSeconds(time);

        if (transform.CompareTag("bonus"))
        {
            MMVibrationManager.Haptic(HapticTypes.Selection);
            FindObjectOfType<SoundEffects>().Pop();
        }
        else
            MMVibrationManager.StopAllHaptics(true);

        bool leftOrRightOnly = gameObject.name.Contains("Pawn") || gameObject.name.Contains("pawn");
        foreach (Rigidbody rb in gameObject.GetComponentsInChildren<Rigidbody>())
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.drag = 0;
            rb.mass *= 100;
            rb.useGravity = true;

            float sideForce = 1500f;


            if (leftOrRightOnly || easySliced)
            {
                //rb.AddForce(Vector3.left * 1000, ForceMode.Impulse);
                //int zeroOrOne = Random.Range(0, 2);
                if (rb.transform.name.Contains("neg")){

                    rb.AddForce(Vector3.left * sideForce, ForceMode.Impulse);
                }

                else if (rb.transform.name.Contains("pos"))
                {
                    rb.AddForce(-Vector3.left * sideForce, ForceMode.Impulse);
                }

            }
            else
            {
                float xForce = 0;
                int zeroOrOne = Random.Range(0, 2);
                if (zeroOrOne == 0)
                    xForce = -1f;
                else
                    xForce = 1f;

                rb.AddForce(new Vector3(xForce, -1, -0.5f) * 1000, ForceMode.Impulse);
            }
            
            //rb.velocity = Vector3.zero;
        }
        if (!leftOrRightOnly && !easySliced)
        {
            amc.FinishedSlice();
        }
        yield return new WaitForSeconds(1f);
        if (leftOrRightOnly)
        {
            Destroy(gameObject);
        }

        else if (gameObject.transform.name.Equals("BOSS"))
        {
            GameManager.Instance.UpdateGameState(GameState.Victory);
        }
        else
        {
            Destroy(gameObject,1.5f);
        }
    }

    public void DropWeapon()
    {
        if (weaponDropped || transform.CompareTag("bonus") || transform.GetChild(0).CompareTag("pawn"))
            return;

        weaponDropped = true;
        
        weapon = Helpers.FindComponentInChildWithTag<Transform>(transform.gameObject, "weapon").gameObject;

        if (weapon != null)
        {
            weapon.AddComponent<Rigidbody>();
            Destroy(weapon, 1);
            weapon = null;
        }
    }
}
