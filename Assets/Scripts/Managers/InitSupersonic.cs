using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using Facebook.Unity;
using Tabtale.TTPlugins;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif


public class InitSupersonic : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        
        TTPCore.Setup();
        
        /*
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
        */

        //GameAnalytics.Initialize();
        //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "World_01", "Stage_01", "Level_Progress");
#if UNITY_IOS
        SkAdNetworkBinding.SkAdNetworkRegisterAppForNetworkAttribution();
#endif
    }

    private void Start()
    {
        InitCallback();
    }

    private void InitCallback()
    {
        /*
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
            TestEvent();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
        */

        int sceneToLoad = PlayerPrefs.GetInt("lvl", 1) % SceneManager.sceneCountInBuildSettings;
        if (sceneToLoad == 0)
            PlayerPrefs.SetInt("lvl", PlayerPrefs.GetInt("lvl") + 1);
        if (PlayerPrefs.GetFloat("sharpness") < 1)
            PlayerPrefs.SetFloat("sharpness", 1);

        Debug.Log("LEVEL: " + PlayerPrefs.GetInt("lvl") + "REAL LVL: " + PlayerPrefs.GetInt("RealLvl"));
        SceneManager.LoadScene(sceneToLoad);

        if (PlayerPrefs.GetInt("RealLvl") == 0)
            PlayerPrefs.SetInt("RealLvl", 1);
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void TestEvent()
    {
        var softPurchaseParameters = new Dictionary<string, object>();
        softPurchaseParameters["mygame_purchased_item"] = "bag";
        /*
        FB.LogAppEvent(
          Facebook.Unity.AppEventName.SpentCredits,
          (float)100,
          softPurchaseParameters
        );
        */

    }
}
