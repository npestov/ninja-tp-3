using UnityEngine;
using Cinemachine;


public class TimeManager : MonoBehaviour
{
	public static TimeManager Instance;
	public float slowdownFactor;
	public float slowdownLength = 2f;

    private void Start()
    {
		Instance = this;
		slowdownFactor = 0.5f;
	}
    public void DoSlowmotion()
	{
		//Time.timeScale = slowdownFactor;
		//Time.fixedDeltaTime = Time.timeScale * .02f;
	}
	public void DoBonusSlowMotion()
    {

		//Time.timeScale = 0.1f;
		//Time.fixedDeltaTime = Time.timeScale * .02f;
	}

	public void RemoveSlowMotion()
    {
		//Time.timeScale = 1;
	}

    private void Update()
    {
    }

}