using System;
using System.Collections;
using UnityEngine;

public class GameViewEncoder : MonoBehaviour
{
	public GameViewCaptureMode CaptureMode;

	public GameViewResize Resize = GameViewResize.Quarter;

	public Camera MainCam;

	public Camera RenderCam;

	public Vector2 Resolution = new Vector2(512f, 512f);

	public bool MatchScreenAspect = true;

	public bool FastMode;

	public bool GZipMode;

	[Range(10f, 100f)]
	public int Quality = 40;

	[Range(1f, 60f)]
	public float StreamFPS = 20f;

	private float interval = 0.05f;

	private bool NeedUpdateTexture;

	private bool EncodingTexture;

	public Texture2D CapturedTexture;

	private RenderTexture rt;

	private Texture2D Screenshot;

	public UnityEventByteArray OnDataByteReadyEvent;

	public int label = 1001;

	private int dataID;

	private int maxID = 1024;

	private int chunkSize = 8096;

	private float next;

	private bool stop;

	private byte[] dataByte;

	public int dataLength;

	private void Start()
	{
		Application.runInBackground = true;
		StartCoroutine(SenderCOR());
	}

	private void Update()
	{
		if (CaptureMode == GameViewCaptureMode.MainCam && MainCam == null)
		{
			MainCam = GetComponent<Camera>();
		}
		if (CaptureMode != GameViewCaptureMode.RenderCam)
		{
			Resolution = new Vector2(Screen.width, Screen.height);
			Resolution /= Mathf.Pow(2f, (float)Resize);
			if (RenderCam != null && RenderCam.targetTexture != null)
			{
				RenderCam.targetTexture = null;
			}
		}
		else if (MatchScreenAspect)
		{
			if (Screen.width > Screen.height)
			{
				Resolution.y = Resolution.x / (float)Screen.width * (float)Screen.height;
			}
			if (Screen.width < Screen.height)
			{
				Resolution.x = Resolution.y / (float)Screen.height * (float)Screen.width;
			}
		}
		Resolution.x = Mathf.RoundToInt(Resolution.x);
		Resolution.y = Mathf.RoundToInt(Resolution.y);
	}

	private void CheckResolution()
	{
		if (rt == null)
		{
			rt = new RenderTexture(Mathf.RoundToInt(Resolution.x), Mathf.RoundToInt(Resolution.y), 16, RenderTextureFormat.ARGB32);
		}
		else if (rt.width != Mathf.RoundToInt(Resolution.x) || rt.height != Mathf.RoundToInt(Resolution.y))
		{
			UnityEngine.Object.Destroy(rt);
			rt = new RenderTexture(Mathf.RoundToInt(Resolution.x), Mathf.RoundToInt(Resolution.y), 16, RenderTextureFormat.ARGB32);
		}
		if (CapturedTexture == null)
		{
			CapturedTexture = new Texture2D(Mathf.RoundToInt(Resolution.x), Mathf.RoundToInt(Resolution.y), TextureFormat.RGB24, mipChain: false);
		}
		else if (CapturedTexture.width != Mathf.RoundToInt(Resolution.x) || CapturedTexture.height != Mathf.RoundToInt(Resolution.y))
		{
			UnityEngine.Object.Destroy(CapturedTexture);
			CapturedTexture = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, mipChain: false);
		}
	}

	private IEnumerator ProcessCapturedTexture()
	{
		CapturedTexture.ReadPixels(new Rect(0f, 0f, rt.width, rt.height), 0, 0);
		CapturedTexture.Apply();
		StartCoroutine(EncodeBytes());
		yield break;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (CaptureMode == GameViewCaptureMode.MainCam && NeedUpdateTexture && !EncodingTexture)
		{
			NeedUpdateTexture = false;
			CheckResolution();
			Graphics.Blit(source, rt);
			StartCoroutine(ProcessCapturedTexture());
		}
		Graphics.Blit(source, destination);
	}

	private IEnumerator RenderTextureRefresh()
	{
		if (!NeedUpdateTexture || EncodingTexture)
		{
			yield break;
		}
		NeedUpdateTexture = false;
		EncodingTexture = true;
		yield return new WaitForEndOfFrame();
		CheckResolution();
		if (CaptureMode == GameViewCaptureMode.RenderCam && RenderCam != null)
		{
			RenderCam.targetTexture = rt;
			RenderCam.Render();
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = rt;
			StartCoroutine(ProcessCapturedTexture());
			RenderTexture.active = active;
		}
		if (CaptureMode != GameViewCaptureMode.FullScreen)
		{
			yield break;
		}
		if (Resize == GameViewResize.Full)
		{
			if (CapturedTexture != null)
			{
				UnityEngine.Object.Destroy(CapturedTexture);
			}
			CapturedTexture = ScreenCapture.CaptureScreenshotAsTexture();
			StartCoroutine(EncodeBytes());
			yield break;
		}
		if (Screenshot != null)
		{
			UnityEngine.Object.Destroy(Screenshot);
		}
		Screenshot = ScreenCapture.CaptureScreenshotAsTexture();
		Graphics.Blit(Screenshot, rt);
		StartCoroutine(ProcessCapturedTexture());
	}

	private IEnumerator SenderCOR()
	{
		while (!stop)
		{
			if (Time.realtimeSinceStartup > next)
			{
				interval = 1f / StreamFPS;
				next = Time.realtimeSinceStartup + interval;
				if (!EncodingTexture)
				{
					NeedUpdateTexture = true;
					if (CaptureMode != GameViewCaptureMode.MainCam)
					{
						StartCoroutine(RenderTextureRefresh());
					}
				}
				yield return null;
			}
			yield return null;
		}
	}

	private IEnumerator EncodeBytes()
	{
		if (CapturedTexture != null)
		{
			yield return null;
			dataByte = (FastMode ? CapturedTexture.FMEncodeToJPG(Quality) : CapturedTexture.EncodeToJPG(Quality));
			if (GZipMode)
			{
				dataByte = dataByte.FMZipBytes();
			}
			dataLength = dataByte.Length;
			int num = dataByte.Length;
			int num2 = 0;
			byte[] bytes = BitConverter.GetBytes(label);
			byte[] bytes2 = BitConverter.GetBytes(dataID);
			byte[] bytes3 = BitConverter.GetBytes(num);
			int num3 = Mathf.FloorToInt(dataByte.Length / chunkSize);
			for (int i = 0; i <= num3; i++)
			{
				int num4 = ((i == num3) ? (num % chunkSize + 17) : (chunkSize + 17));
				byte[] bytes4 = BitConverter.GetBytes(num2);
				byte[] array = new byte[num4];
				Buffer.BlockCopy(bytes, 0, array, 0, 4);
				Buffer.BlockCopy(bytes2, 0, array, 4, 4);
				Buffer.BlockCopy(bytes3, 0, array, 8, 4);
				Buffer.BlockCopy(bytes4, 0, array, 12, 4);
				array[16] = (byte)(GZipMode ? 1u : 0u);
				Buffer.BlockCopy(dataByte, num2, array, 17, array.Length - 17);
				OnDataByteReadyEvent.Invoke(array);
				num2 += chunkSize;
			}
			dataID++;
			if (dataID > maxID)
			{
				dataID = 0;
			}
		}
		EncodingTexture = false;
	}

	private void OnEnable()
	{
		StartAll();
	}

	private void OnDisable()
	{
		StopAll();
	}

	private void OnApplicationQuit()
	{
		StopAll();
	}

	private void OnDestroy()
	{
		StopAll();
	}

	private void StopAll()
	{
		stop = true;
		StopAllCoroutines();
	}

	private void StartAll()
	{
		if (!(Time.realtimeSinceStartup < 3f))
		{
			stop = false;
			StartCoroutine(SenderCOR());
			NeedUpdateTexture = false;
			EncodingTexture = false;
		}
	}
}
