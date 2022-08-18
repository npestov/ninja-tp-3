using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{

    public GameObject AimHelp;
    public GameObject ReleaseHelp;
    public GameObject SliceHelp;

    public GameObject animatedFinger;

    private int counter;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnStateChanged;
    }

    private void GameManagerOnStateChanged(GameState state)
    {
        if (state == GameState.Killing)
        {
            StartCoroutine(SliceDalay());
            Aim(false);
            Release(false);
        }
        if (state == GameState.Walking)
        {
            Slice(false);
            if (counter < 3)
            {
                counter++;
                StartCoroutine(AimDelay(true));
            }
        }

    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && SliceHelp.activeSelf)
        {
            animatedFinger.SetActive(false);
        }
    }

    public void Release(bool state)
    {
        if (ReleaseHelp == null)
            return;

        StartCoroutine(ReleaseDelay(state));
    }

    public void Aim(bool state)
    {
        if (AimHelp == null)
            return;

        if (state == false)
            AimHelp.SetActive(false);

        StartCoroutine(AimDelay(state));
    }

    private void Slice(bool state)
    {
        if (SliceHelp == null)
            return;

        SliceHelp.SetActive(state);
        animatedFinger.SetActive(true);

        if (state)
        {

        }
        else
        {

        }
    }

    IEnumerator AimDelay(bool state)
    {
        if (AimHelp)
        {
            yield return new WaitForSeconds(0.5f);
            AimHelp.SetActive(state);

            if (state)
            {

            }
            else
            {

            }
        }
    }

    IEnumerator SliceDalay()
    {
        yield return new WaitForSeconds(1.5f);
        Slice(true);
    }

    IEnumerator ReleaseDelay(bool state)
    {
        if (state)
        {
            Aim(false);
            Slice(false);
        }
        else
        {
            ReleaseHelp.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        ReleaseHelp.SetActive(state);
    }

}
