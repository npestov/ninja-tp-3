using System;
using System.Collections;
using System.Collections.Generic;
using IndieMarc.EnemyVision;
using UnityEditor;
using UnityEngine;

public class BakeMeshMe : MonoBehaviour
{
    SkinnedMeshRenderer sm;
    Mesh newMesh;
    public GameObject myWeapon;
    public bool bakeImmediatly = false;

    // Start is called before the first frame update
    void Awake()
    {
        newMesh = new Mesh();

    }

    private void Start()
    {
        if (bakeImmediatly)
            Invoke("ChangeMesh",1);

        sm = transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();
        try
        {
            myWeapon = Helpers.FindComponentInChildWithTag<Transform>(transform.parent.gameObject, "weapon").gameObject;
        }
        catch(Exception e)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMesh()
    {
        //bones.transform.localEulerAngles = transform.parent.eulerAngles - new Vector3(90, 0, 0);
        //transform.localEulerAngles = transform.parent.eulerAngles - new Vector3(90, 0, 0);
        //transform.parent.eulerAngles = Vector3.zero;

        sm.BakeMesh(newMesh);
        //AssetDatabase.CreateAsset(newMesh, "Assets/newPawn.fbx");

        var m = gameObject.AddComponent<MeshFilter>();
        var rend = gameObject.AddComponent<MeshRenderer>();
        rend.receiveShadows = false;
        rend.material = sm.material;
        Destroy(sm.transform.parent.gameObject);
        m.mesh = newMesh;
        Destroy(transform.parent.GetComponent<Animator>());
        Destroy(transform.parent.GetComponent<Enemy>());
        Destroy(transform.parent.GetComponent<ChaserEnemy>());
        Destroy(transform.GetComponent<Collider>());

        var mc = gameObject.AddComponent<MeshCollider>();
        mc.convex = true;

        //remove weapon
        myWeapon.transform.parent = transform.parent;

        GetComponent<TargetScript>().RevertBack();

        Destroy(this);
    }


}

