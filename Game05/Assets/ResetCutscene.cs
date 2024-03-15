using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCutscene : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	static void OnBeforeSplashScreen()
	{
		PlayerPrefs.SetInt("FirstOpen", 1);
	}
}
