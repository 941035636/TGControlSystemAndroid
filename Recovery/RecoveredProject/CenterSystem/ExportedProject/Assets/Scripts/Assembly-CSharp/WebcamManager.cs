using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WebcamManager : MonoBehaviour
{
	private WebCamDevice[] devices;

	private WebCamTexture[] webCams;

	private Texture[] textures;

	public int TargetCamID;

	public bool useFrontCam = true;

	private bool canRestart = true;

	private WebCamTexture webCamTexture;

	public Material[] materials;

	public GameObject[] targetMeshObjects;

	public GameObject BackgroundQuad;

	private int timeoutFrameCount = 300;

	public Vector2 requestResolution = new Vector2(1280f, 720f);

	public Vector2 textureResolution;

	private bool isInitWaiting;

	private float TextureRatio = 1f;

	private float ScreenRatio = 1f;

	private Quaternion baseRotation;

	public void Action_useFrontCam(bool _value)
	{
		useFrontCam = _value;
		if (canRestart)
		{
			StartCoroutine(RestartWebcam());
		}
	}

	private IEnumerator RestartWebcam()
	{
		canRestart = false;
		Action_StopWebcam();
		yield return initAndWaitForWebCamTexture();
		canRestart = true;
		yield return null;
	}

	private void Start()
	{
		StartCoroutine(initAndWaitForWebCamTexture());
		if (BackgroundQuad != null)
		{
			baseRotation = BackgroundQuad.transform.localRotation;
		}
	}

	private void Update()
	{
		if (BackgroundQuad != null && webCamTexture != null)
		{
			CalculateBackgroundQuad();
		}
	}

	public void OnApplicationQuit()
	{
		Action_StopWebcam();
	}

	private void CalculateBackgroundQuad()
	{
		Camera main = Camera.main;
		ScreenRatio = (float)Screen.width / (float)Screen.height;
		BackgroundQuad.transform.SetParent(main.transform);
		BackgroundQuad.transform.localPosition = new Vector3(0f, 0f, main.farClipPlane / 2f);
		float num = webCamTexture.videoRotationAngle;
		BackgroundQuad.transform.localRotation = baseRotation * Quaternion.AngleAxis(webCamTexture.videoRotationAngle, Vector3.forward);
		float num2 = main.farClipPlane / 2f;
		float num3 = 2f * num2 * Mathf.Tan(main.fieldOfView * 0.5f * ((float)Math.PI / 180f));
		BackgroundQuad.transform.localPosition = new Vector3(0f, 0f, num2);
		Vector3 localScale = new Vector3(1f, num3, 1f);
		if (num == 0f || num == 180f)
		{
			TextureRatio = (float)webCamTexture.width / (float)webCamTexture.height;
			if (ScreenRatio > TextureRatio)
			{
				float num4 = ScreenRatio / TextureRatio;
				float x = TextureRatio * num3 * num4;
				float y = num3 * (float)((!webCamTexture.videoVerticallyMirrored) ? 1 : (-1)) * num4;
				localScale = new Vector3(x, y, 1f);
			}
			else
			{
				float x2 = TextureRatio * num3;
				localScale = new Vector3(x2, num3 * (float)((!webCamTexture.videoVerticallyMirrored) ? 1 : (-1)), 1f);
			}
		}
		else
		{
			TextureRatio = (float)webCamTexture.height / (float)webCamTexture.width;
			if (ScreenRatio > TextureRatio)
			{
				float num5 = ScreenRatio / TextureRatio;
				float num6 = num3 * -1f * num5;
				float y2 = num6 * (float)(webCamTexture.videoVerticallyMirrored ? 1 : (-1)) * num5;
				localScale = new Vector3(num6, y2, 1f);
			}
			else
			{
				float num7 = TextureRatio * num3;
				localScale = new Vector3(num3 * -1f, num7 * (float)(webCamTexture.videoVerticallyMirrored ? 1 : (-1)), 1f);
			}
		}
		BackgroundQuad.transform.localScale = localScale;
	}

	public void Action_StopWebcam()
	{
		if (webCams != null)
		{
			for (int i = 0; i < webCams.Length; i++)
			{
				if (webCams[i].isPlaying)
				{
					webCams[i].Stop();
				}
				UnityEngine.Object.Destroy(webCams[i]);
				UnityEngine.Object.Destroy(textures[i]);
			}
		}
		webCams = null;
		devices = null;
		textures = null;
		UnityEngine.Object.Destroy(webCamTexture);
		webCamTexture = null;
	}

	public void Action_StartWebcam()
	{
		if (base.enabled && base.gameObject.activeSelf && !isInitWaiting)
		{
			StartCoroutine(initAndWaitForWebCamTexture());
		}
	}

	private void OnEnable()
	{
		if (!(Time.realtimeSinceStartup < 3f))
		{
			Action_StartWebcam();
		}
	}

	private void OnDisable()
	{
		Action_StopWebcam();
	}

	private IEnumerator initAndWaitForWebCamTexture()
	{
		isInitWaiting = true;
		devices = WebCamTexture.devices;
		webCams = new WebCamTexture[devices.Length];
		textures = new Texture[devices.Length];
		for (int i = 0; i < devices.Length; i++)
		{
			webCams[i] = new WebCamTexture();
			webCams[i] = new WebCamTexture(devices[i].name, Mathf.RoundToInt(requestResolution.x), Mathf.RoundToInt(requestResolution.y), 30);
			textures[i] = webCams[i];
			textures[i].wrapMode = TextureWrapMode.Repeat;
			if (useFrontCam)
			{
				if (devices[i].isFrontFacing)
				{
					TargetCamID = i;
				}
			}
			else if (!devices[i].isFrontFacing)
			{
				TargetCamID = i;
			}
		}
		if (TargetCamID > devices.Length - 1)
		{
			TargetCamID = devices.Length - 1;
		}
		webCams[TargetCamID].requestedFPS = 30f;
		webCams[TargetCamID].Play();
		for (int j = 0; j < materials.Length; j++)
		{
			materials[j].mainTexture = textures[TargetCamID];
		}
		if (BackgroundQuad != null)
		{
			BackgroundQuad.GetComponent<Renderer>().material.mainTexture = textures[TargetCamID];
		}
		GameObject[] array = targetMeshObjects;
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<Renderer>() != null)
			{
				gameObject.GetComponent<Renderer>().material.mainTexture = textures[TargetCamID];
			}
			if (gameObject.GetComponent<RawImage>() != null)
			{
				gameObject.GetComponent<RawImage>().texture = textures[TargetCamID];
			}
		}
		if (textures.Length != 0)
		{
			webCamTexture = webCams[TargetCamID];
			int initFrameCount = 0;
			while (webCamTexture.width <= 16 && initFrameCount <= timeoutFrameCount)
			{
				initFrameCount++;
				yield return new WaitForEndOfFrame();
			}
			textureResolution = new Vector2(webCamTexture.width, webCamTexture.height);
			isInitWaiting = false;
		}
		yield return null;
	}
}
