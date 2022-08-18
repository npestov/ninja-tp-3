using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    float resetTime = 0.5f;
    float timer;
    public AudioSource popSource;
    public AudioSource throwSwordSource;
    float originalPitch;

    // Start is called before the first frame update
    void Awake()
    {
        originalPitch = popSource.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            ResetPop();
        }
    }

    public void Pop()
    {
        timer = resetTime;
        popSource.Play();
        popSource.pitch += 0.10f;
    }
    public void ThrowSwordPlay()
    {
        throwSwordSource.Play();
    }

    private void ResetPop()
    {
        popSource.pitch = originalPitch;
    }
}
