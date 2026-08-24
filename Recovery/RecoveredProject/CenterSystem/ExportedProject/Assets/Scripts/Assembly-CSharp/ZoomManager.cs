using UnityEngine;

public class ZoomManager : MonoBehaviour
{
	public Camera cam;

	public float miniFov = 10f;

	public float maxFov = 60f;

	private float fov;

	private Touch[] fingers;

	private float NDistStart;

	private float NDistNow;

	private float NDistDelta;

	private float fovStart;

	private bool startZoom;

	private void Start()
	{
		if (cam == null)
		{
			cam = Camera.main;
		}
		fov = cam.fieldOfView;
	}

	private void Update()
	{
		GestureZoom();
		fov = Mathf.Clamp(fov, miniFov, maxFov);
		cam.fieldOfView = fov;
	}

	private void GestureZoom()
	{
		if (Input.touchCount >= 2)
		{
			fingers = Input.touches;
			if (fingers[1].phase != TouchPhase.Began && fingers[1].phase == TouchPhase.Moved)
			{
				if (!startZoom)
				{
					startZoom = true;
					fovStart = fov;
					Vector2 a = new Vector2(fingers[0].position.x / (float)Screen.width, fingers[0].position.y / (float)Screen.height);
					Vector2 b = new Vector2(fingers[1].position.x / (float)Screen.width, fingers[1].position.y / (float)Screen.height);
					NDistStart = Vector2.Distance(a, b);
				}
				else
				{
					Vector2 a2 = new Vector2(fingers[0].position.x / (float)Screen.width, fingers[0].position.y / (float)Screen.height);
					Vector2 b2 = new Vector2(fingers[1].position.x / (float)Screen.width, fingers[1].position.y / (float)Screen.height);
					NDistNow = Vector2.Distance(a2, b2);
					NDistDelta = NDistNow - NDistStart;
					fov = fovStart - NDistDelta * 30f;
				}
			}
		}
		else
		{
			startZoom = false;
		}
	}

	public void ZoomIn()
	{
		fov -= Time.deltaTime * 10f;
	}

	public void ZoomOut()
	{
		fov += Time.deltaTime * 10f;
	}
}
