using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwordSpawner : MonoBehaviour
{
    public GameObject[] allSwords = new GameObject[9];
    GameObject mySword;
    //temp
    Vector3 spawnPoint;
    Quaternion spawnRot;
    Vector3 spawnScale;
    Transform parentHand;

    Transform slicerSwordsParent;

    // Start is called before the first frame update
    void Awake()
    {
        slicerSwordsParent = GameObject.FindGameObjectWithTag("slicingSword").transform;
        spawnPoint = new Vector3(0.357f, -0.005f, 0.576f);
        spawnRot = Quaternion.Euler(-11.91f, 21.04f, 3.999f);
        spawnScale = new Vector3(1, 1, 1);
        SpawnNewSword(PlayerPrefs.GetInt("equippedSword",0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeleteOldSword()
    {
        Destroy(GameObject.FindGameObjectWithTag("sword"));
    }

    public void SpawnNewSword(int swordIndex)
    {
        //if (SceneManager.GetActiveScene().name.Contains("CL"))
            //return;
        /*
        if (swordIndex > 8)
        {
            swordIndex = swordIndex - 8 * (Mathf.FloorToInt(PlayerPrefs.GetFloat("sharpness") / 24));
        }
        swordIndex = Mathf.Clamp(swordIndex, 0, 8);
        */
        parentHand = GameObject.FindGameObjectWithTag("rightHand").transform;
        mySword = Instantiate(allSwords[swordIndex], parentHand.position, parentHand.rotation);
        mySword.transform.parent = parentHand;
        mySword.transform.localPosition = new Vector3(0,0,0);
        mySword.transform.localEulerAngles = new Vector3(0, 0, 0);
        mySword.transform.localScale = spawnScale;
        FindObjectOfType<CameraController>().FindSword();

        AdjustSlicingSword(swordIndex);
    }

    private void AdjustSlicingSword(int index)
    {
        

        for (int i = 0; i < slicerSwordsParent.childCount; i++)
        {
            slicerSwordsParent.GetChild(i).gameObject.SetActive(false);
        }

        slicerSwordsParent.GetChild(index).gameObject.SetActive(true);

    }
}
