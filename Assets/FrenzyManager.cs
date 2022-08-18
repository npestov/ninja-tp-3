using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrenzyManager : MonoBehaviour
{
    private Image filledImage;
    private float sliderValue;
    private bool dontSlide;
    bool tpedInCycle = false;

    PlayerMovement pm;

    // Start is called before the first frame update
    void Awake()
    {
        filledImage = transform.GetChild(1).GetComponent<Image>();
        pm = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        /*
        if (superTimer > 0)
        {
            superTimer -= Time.deltaTime;
            sliderValue = 1;
        }
        else if (superTimer < 0 && superTimer > -1f)
        {
            return;
        }
        */
        if (Input.GetKeyDown(KeyCode.W))
        {
            sliderValue = 1f;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (dontSlide)
        {
            dontSlide = false;
            return;
        }
        if (sliderValue != 1)
        {
            sliderValue -= 0.045f * Time.deltaTime;
        }
        UpdateSliderValue();
    }

    public void IncreaseSlider(float increment)
    {
        sliderValue += increment;
        dontSlide = true;
        UpdateSliderValue();
        if (sliderValue > 0.82f)
        {
            pm.superSayne.SetActive(true);
            if (sliderValue > 0.99f)
            {
                if (!tpedInCycle)
                {
                    ActivateSayne();
                }
            }
        }
    }

    public void ResetSayne()
    {
        pm.superSayne.SetActive(false);
        sliderValue = 0f;
        tpedInCycle = false;
        UpdateSliderValue();
        Debug.Log("rest sayne " + sliderValue);
    }

    private void ActivateSayne()
    {
        tpedInCycle = true;
        //float time = 3.5f;
        pm.SuperSayne(2);
        //superTimer = time;
    }

    private void UpdateSliderValue()
    {
        sliderValue = Mathf.Clamp(sliderValue, 0f, 1f);
        filledImage.fillAmount = sliderValue;
    }
}
