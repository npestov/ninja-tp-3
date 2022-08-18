using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    TrailRenderer tr;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        DisableTrail();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EnableTrail();
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 3;
            transform.position = pos;
            Debug.Log("Mouse Pos: " + pos + "Actual pos"+ transform.position);
        }
        if (Input.GetMouseButtonUp(0))
        {
            DisableTrail();
        }

    }

    private void DisableTrail()
    {
        /*
        tr.enabled = false;
        sr.enabled = false;
        */
    }
    private void EnableTrail()
    {
        /*
        tr.enabled = true;
        sr.enabled = true;
        */
    }
}
