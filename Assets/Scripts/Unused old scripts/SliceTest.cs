using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BzKovSoft.CharacterSlicer;
using BzKovSoft.ObjectSlicer;

public class SliceTest : MonoBehaviour
{
    public GameObject _target;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelaySlice());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator DelaySlice()
    {
        yield return new WaitForSeconds(1f);
        var sliceable = _target.GetComponent<BzSliceableCharacterBase>();
        if (sliceable == null)
        {
            sliceable = _target.GetComponentInChildren<BzSliceableCharacterBase>();
        }
        Debug.Log("My sliceble: " + sliceable);
        sliceable.Slice(new Plane(new Vector3(1,0,0), 90), 0, null);
    }
}
