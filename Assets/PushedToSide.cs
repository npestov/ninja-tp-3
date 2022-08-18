using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushedToSide : MonoBehaviour
{
    public GameObject slicedPawn;
    public bool flyLeft = false;
    public bool flyRight = false;
    private bool animated = false;

    private float rotLimit = 33;

    // Start is called before the first frame update
    void Start()
    {
        if (flyLeft)
        {
            var rb = GetComponent<Rigidbody>();
            rb.AddForce(-Vector3.left * 10, ForceMode.Impulse);
            Destroy(transform.parent.gameObject, 1);
        }
        else if (flyRight)
        {
            var rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.left * 10, ForceMode.Impulse);
            Destroy(transform.parent.gameObject, 1);
        }
        else
        {
            animated = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!animated)
            return;
        var speed = 0.6f;
        float rY = Mathf.SmoothStep(-rotLimit, rotLimit, Mathf.PingPong(Time.time * speed, 1));
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rY, 0);
    }

    public void PmCallsSlice()
    {
        var g = Instantiate(slicedPawn, transform.parent.position, transform.parent.rotation);
        g.transform.GetChild(0).localScale = transform.localScale;
        g.transform.GetChild(1).localScale = transform.localScale;
        Destroy(transform.parent.gameObject);
        //StartCoroutine(Slice(null));
    }
}
