using UnityEngine;

public class GyroController : MonoBehaviour
{
	public static GyroController instance;

	private bool gyroEnabled;

	private Gyroscope gyro;

	private GameObject cameraContainer;

	private Quaternion rot;

	public GameObject offsetContainer;

	private Quaternion offsetRot;

	public GameObject CamChildrenGrp;

	public Vector3 localRotAng;

	private Vector2 MD_oldPos;

	private Vector2 MD_currentPos;

	private Vector2 MD_deltaPos;

	private float MD_speed = 3000f;

	private bool ignoreFirstTouch;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	private void Start()
	{
		cameraContainer = new GameObject("Camera Container");
		cameraContainer.transform.position = base.transform.position;
		base.transform.SetParent(cameraContainer.transform);
		gyroEnabled = EnableGyro();
		offsetContainer = new GameObject("Offset Container");
		offsetContainer.transform.position = base.transform.position;
		cameraContainer.transform.SetParent(offsetContainer.transform);
	}

	private bool EnableGyro()
	{
		if (SystemInfo.supportsGyroscope)
		{
			gyro = Input.gyro;
			gyro.enabled = true;
			cameraContainer.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
			rot = new Quaternion(0f, 0f, 1f, 0f);
			gyro.updateInterval = 0.01f;
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (gyroEnabled)
		{
			base.transform.localRotation = gyro.attitude * rot;
		}
		else
		{
			MouseDragRot();
		}
		localRotAng = base.transform.localRotation.eulerAngles;
		TouchDragRot();
	}

	public void AimingOffset(float _angle)
	{
		offsetContainer.transform.Rotate(new Vector3(0f, _angle, 0f), Space.World);
	}

	private void MouseDragRot()
	{
		if (Input.GetMouseButtonDown(1))
		{
			MD_currentPos.x = Input.mousePosition.x / (float)Screen.width;
			MD_currentPos.y = Input.mousePosition.y / (float)Screen.height;
		}
		else if (Input.GetMouseButton(1))
		{
			MD_oldPos = MD_currentPos;
			MD_currentPos.x = Input.mousePosition.x / (float)Screen.width;
			MD_currentPos.y = Input.mousePosition.y / (float)Screen.height;
			MD_deltaPos = MD_currentPos - MD_oldPos;
			offsetContainer.transform.Rotate(new Vector3(0f, (0f - MD_deltaPos.x) * Time.deltaTime * MD_speed, 0f), Space.World);
			offsetContainer.transform.Rotate(new Vector3(MD_deltaPos.y * Time.deltaTime * MD_speed, 0f, 0f), Space.Self);
		}
	}

	private void TouchDragRot()
	{
		if (Input.touchCount > 2)
		{
			if (Input.touches[0].phase == TouchPhase.Began)
			{
				MD_currentPos.x = Input.touches[0].position.x / (float)Screen.width;
				MD_currentPos.y = Input.touches[0].position.y / (float)Screen.height;
			}
			else
			{
				MD_oldPos = MD_currentPos;
				MD_currentPos.x = Input.touches[0].position.x / (float)Screen.width;
				MD_currentPos.y = Input.touches[0].position.y / (float)Screen.height;
				MD_deltaPos = MD_currentPos - MD_oldPos;
			}
			if (ignoreFirstTouch)
			{
				ignoreFirstTouch = false;
			}
			else
			{
				offsetContainer.transform.Rotate(new Vector3(0f, (0f - MD_deltaPos.x) * Time.deltaTime * MD_speed, 0f), Space.World);
			}
		}
		if (Input.touchCount == 0)
		{
			ignoreFirstTouch = true;
		}
	}
}
