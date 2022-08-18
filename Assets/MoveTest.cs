using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveFwrd());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator MoveFwrd()
    {
        yield return new WaitForSeconds(1);
        transform.DOMove(transform.position - new Vector3(transform.position.x, transform.position.y, 5), 2);
    }
}
