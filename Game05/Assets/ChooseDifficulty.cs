using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseDifficulty : MonoBehaviour
{
	public Button easyButton;
	public Button hardButton;
	public void ChooseEasy()
	{
		PlayerPrefs.SetInt("Difficulty", 1);
		easyButton.interactable = false;
		hardButton.interactable = false;
		if (PlayerPrefs.GetInt("FirstOpen") == 1)
			SceneChanger.Instance.FadeToNextScene();
		else
			SceneChanger.Instance.FadeToScene(2);
	}
	public void ChooseHard()
	{
		PlayerPrefs.SetInt("Difficulty", 0);
		easyButton.interactable = false;
		hardButton.interactable = false;
		if (PlayerPrefs.GetInt("FirstOpen") == 1)
			SceneChanger.Instance.FadeToNextScene();
		else
			SceneChanger.Instance.FadeToScene(2);
	}
}
