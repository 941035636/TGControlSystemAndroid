using System.Collections;
using UnityEngine;

public class TargetProjectionMatrix : MonoBehaviour
{
	public Camera referenceCam;

	public Camera targetCam;

	public bool useCustomFOV;

	public float fov = 60f;

	[Header("limit the maximum fov")]
	public bool maxFovAsReference;

	public bool allowUpdate = true;

	public bool ForceDisableUpdate;

	public void Action_UpdateFOV(float _fov)
	{
		fov = _fov;
	}

	public void Action_SetAllowUpdate(bool _value)
	{
		allowUpdate = _value;
	}

	public void Action_SetForceDisableUpdate(bool _value)
	{
		ForceDisableUpdate = _value;
	}

	private IEnumerator UpdateProjectionMatrixLoop()
	{
		while (true)
		{
			if (!allowUpdate || ForceDisableUpdate)
			{
				yield return new WaitForEndOfFrame();
				continue;
			}
			Matrix4x4 projectionMatrix = referenceCam.projectionMatrix;
			if (!useCustomFOV)
			{
				fov = referenceCam.fieldOfView;
			}
			if (maxFovAsReference && fov > referenceCam.fieldOfView)
			{
				fov = referenceCam.fieldOfView;
			}
			float aspect = referenceCam.aspect;
			float num = 1f / Mathf.Tan(fov / 114.59156f);
			float value = num / aspect;
			projectionMatrix[0, 0] = value;
			projectionMatrix[1, 1] = num;
			targetCam.fieldOfView = fov;
			targetCam.projectionMatrix = projectionMatrix;
			yield return new WaitForSeconds(0.005f);
		}
	}

	private void Start()
	{
		StartCoroutine(UpdateProjectionMatrixLoop());
	}
}
