using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SettingsButton : MonoBehaviour
{

	[Inject]
	private ZenjectSceneLoader sceneLoader;

	public void ButtonClick ()
	{
		sceneLoader.LoadScene ("SettingsScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
	}
}
