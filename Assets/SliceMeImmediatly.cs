using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BzKovSoft.ObjectSlicer;
using BzKovSoft.ObjectSlicerSamples;

public class SliceMeImmediatly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponents<BzKnife>().Length == 1)
        {
            IBzSliceable sliceable = GetComponent<IBzSliceable>();
            IBzSliceableAsync sliceableA = GetComponent<IBzSliceableAsync>();

            Plane plane = new Plane(Vector3.up, 1);

            if (sliceable != null)
                sliceable.Slice(plane);

            if (sliceableA != null)
                sliceableA.Slice(plane, Random.Range(0,9999), null);

            Debug.Log("Hit the knfie");
        }
        Debug.Log("Hit outside knfie");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponents<BzKnife>().Length == 1)
        {
            IBzSliceable sliceable = GetComponent<IBzSliceable>();
            IBzSliceableAsync sliceableA = GetComponent<IBzSliceableAsync>();

            Plane plane = new Plane(Vector3.up, 2);

            if (sliceable != null)
                sliceable.Slice(plane);

            if (sliceableA != null)
                sliceableA.Slice(plane, Random.Range(0, 9999), null);

            Debug.Log("Hit the knfie");
        }
        Debug.Log("Hit outside knfie");

    }
}
