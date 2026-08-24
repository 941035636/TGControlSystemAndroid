using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager_helper : MonoBehaviour
{
	public void Action_LoadSceneName(string _scene)
	{
		SceneManager.LoadScene(_scene);
	}

	public void Action_Quit()
	{
		Application.Quit();
	}
}
