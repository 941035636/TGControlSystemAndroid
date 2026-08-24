using UnityEngine;
using UnityEngine.UI;

public class FPSManager : MonoBehaviour
{
	public Text fpsText;

	private float deltaTime;

	public int targetFps = 30;

	[Tooltip("ignore targetFPS when vSyncCount > 0")]
	public int vSyncCount;

	private float UpdateTextTimer;

	private float UpdateTextThreshold = 0.2f;

	private void Update()
	{
		if (QualitySettings.vSyncCount != vSyncCount)
		{
			QualitySettings.vSyncCount = vSyncCount;
		}
		if (Application.targetFrameRate != targetFps)
		{
			Application.targetFrameRate = targetFps;
		}
		if (!(fpsText == null))
		{
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			UpdateTextTimer += Time.deltaTime;
			if (UpdateTextTimer > UpdateTextThreshold)
			{
				UpdateTextTimer = 0f;
				float f = 1f / deltaTime;
				fpsText.text = "FPS: " + Mathf.Ceil(f);
			}
		}
	}

	public void Action_SetFPS(int _fps)
	{
		targetFps = _fps;
	}
}
