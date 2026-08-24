using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TcpController : MonoBehaviour
{
	public Toggle oneTog;

	public Toggle TwoTog;

	public Toggle ThreeTog;

	public Toggle FourTog;

	public Toggle FiveTog;

	public Toggle SixTog;

	public Toggle SevenTog;

	public Toggle EightTog;

	public Button orderBtn;

	public Button AllOpenBtn;

	public Button allCloseBtn;

	public Button BackBtn;

	private static TcpController _singleton;

	private const int BUFFER_SIZE = 128;

	public string host = string.Empty;

	public int port;

	private byte[] buffer;

	private Socket socket;

	private string str;

	public static TcpController Singleton
	{
		get
		{
			if (_singleton == null)
			{
				_singleton = UnityEngine.Object.FindObjectOfType<TcpController>();
			}
			return _singleton;
		}
	}

	private void OnEnable()
	{
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		Connect();
	}

	private void Start()
	{
		AllOpenBtn.onClick.AddListener(delegate
		{
			Send("FE 0F 00 00 00 08 01 FF F1 D1");
			TogTrue();
		});
		orderBtn.onClick.AddListener(delegate
		{
			StartCoroutine(OrderOpen());
		});
		allCloseBtn.onClick.AddListener(delegate
		{
			Send("FE 0F 00 00 00 08 01 00 B1 91");
			TogFalse();
		});
		oneTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Send("FE 05 00 00 FF 00 98 35");
				oneTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 0f, 0f, 255f);
			}
			else
			{
				Send("FE 05 00 00 00 00 D9 C5");
				oneTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			}
		});
		TwoTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Send("FE 05 00 01 FF 00 C9 F5");
				TwoTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 0f, 0f, 255f);
			}
			else
			{
				Send("FE 05 00 01 00 00 88 05");
				TwoTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			}
		});
		ThreeTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Send("FE 05 00 02 FF 00 39 F5");
				ThreeTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 0f, 0f, 255f);
			}
			else
			{
				Send("FE 05 00 02 00 00 78 05");
				ThreeTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			}
		});
		FourTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Send("FE 05 00 03 FF 00 68 35");
				FourTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 0f, 0f, 255f);
			}
			else
			{
				Send("FE 05 00 03 00 00 29 C5");
				FourTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			}
		});
		FiveTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Send("FE 05 00 04 FF 00 D9 F4");
				FiveTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 0f, 0f, 255f);
			}
			else
			{
				Send("FE 05 00 04 00 00 98 04");
				FiveTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			}
		});
		SixTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Send("FE 05 00 05 FF 00 88 34");
				SixTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 0f, 0f, 255f);
			}
			else
			{
				Send("FE 05 00 05 00 00 C9 C4");
				SixTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			}
		});
		SevenTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Send("FE 05 00 06 FF 00 78 34");
				SevenTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 0f, 0f, 255f);
			}
			else
			{
				Send("FE 05 00 06 00 00 39 C4");
				SevenTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			}
		});
		EightTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Send("FE 05 00 07 FF 00 29 F4");
				EightTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 0f, 0f, 255f);
			}
			else
			{
				Send("FE 05 00 07 00 00 68 04");
				EightTog.transform.Find("Background").GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			}
		});
		BackBtn.onClick.AddListener(delegate
		{
			base.transform.gameObject.SetActive(value: false);
		});
	}

	private IEnumerator OrderOpen()
	{
		Send("FE 05 00 00 FF 00 98 35");
		oneTog.isOn = true;
		yield return new WaitForSeconds(0.4f);
		Send("FE 05 00 01 FF 00 C9 F5");
		TwoTog.isOn = true;
		yield return new WaitForSeconds(0.4f);
		Send("FE 05 00 02 FF 00 39 F5");
		ThreeTog.isOn = true;
		yield return new WaitForSeconds(0.4f);
		Send("FE 05 00 03 FF 00 68 35");
		FourTog.isOn = true;
		yield return new WaitForSeconds(0.4f);
		Send("FE 05 00 04 FF 00 D9 F4");
		FiveTog.isOn = true;
		yield return new WaitForSeconds(0.4f);
		Send("FE 05 00 05 FF 00 88 34");
		SixTog.isOn = true;
		yield return new WaitForSeconds(0.4f);
		Send("FE 05 00 06 FF 00 78 34");
		SevenTog.isOn = true;
		yield return new WaitForSeconds(0.4f);
		Send("FE 05 00 07 FF 00 29 F4");
		EightTog.isOn = true;
		yield return new WaitForSeconds(0.4f);
	}

	private IEnumerator AllClose()
	{
		Send("FE 05 00 00 00 00 D9 C5");
		yield return new WaitForSeconds(0.1f);
		Send("FE 05 00 01 00 00 88 05");
		yield return new WaitForSeconds(0.1f);
		Send("FE 05 00 02 00 00 78 05");
		yield return new WaitForSeconds(0.1f);
		Send("FE 05 00 03 00 00 29 C5");
		yield return new WaitForSeconds(0.1f);
		Send("FE 05 00 04 00 00 98 04");
		yield return new WaitForSeconds(0.1f);
		Send("FE 05 00 05 00 00 C9 C4");
		yield return new WaitForSeconds(0.1f);
		Send("FE 05 00 06 00 00 39 C4");
		yield return new WaitForSeconds(0.1f);
		Send("FE 05 00 07 00 00 68 04");
		TogFalse();
	}

	private void TogFalse()
	{
		oneTog.isOn = false;
		TwoTog.isOn = false;
		ThreeTog.isOn = false;
		FourTog.isOn = false;
		FiveTog.isOn = false;
		SixTog.isOn = false;
		SevenTog.isOn = false;
		EightTog.isOn = false;
	}

	private void TogTrue()
	{
		oneTog.isOn = true;
		TwoTog.isOn = true;
		ThreeTog.isOn = true;
		FourTog.isOn = true;
		FiveTog.isOn = true;
		SixTog.isOn = true;
		SevenTog.isOn = true;
		EightTog.isOn = true;
	}

	private void Connect()
	{
		try
		{
			socket.Connect(host, port);
		}
		catch (Exception ex)
		{
			MonoBehaviour.print(ex.Message);
		}
		if (socket.Connected)
		{
			MonoBehaviour.print("Connected");
			Receive();
		}
		else
		{
			MonoBehaviour.print("Connect fail");
		}
	}

	private void Receive()
	{
		if (!socket.Connected)
		{
			return;
		}
		buffer = new byte[128];
		try
		{
			socket.BeginReceive(buffer, 0, 128, SocketFlags.None, Receive_Callback, socket);
		}
		catch (Exception ex)
		{
			MonoBehaviour.print(ex.Message);
		}
	}

	private void Receive_Callback(IAsyncResult ar)
	{
		if (socket.Connected && socket.EndReceive(ar) > 0)
		{
			Debug.Log(ByteArrayToHexStringNoBlank(buffer));
			Receive();
		}
	}

	public void Send(string message)
	{
		if (socket.Connected)
		{
			byte[] array = strToToHexByte(message);
			socket.Send(array);
		}
	}

	public static string ByteArrayToHexStringNoBlank(byte[] data)
	{
		StringBuilder stringBuilder = new StringBuilder(data.Length * 3);
		foreach (byte b in data)
		{
			stringBuilder.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
			if (stringBuilder.Length == 18)
			{
				break;
			}
		}
		return stringBuilder.ToString().ToUpper();
	}

	private static byte[] strToToHexByte(string hexString)
	{
		hexString = hexString.Replace(" ", "");
		if (hexString.Length % 2 != 0)
		{
			hexString += " ";
		}
		byte[] array = new byte[hexString.Length / 2];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
		}
		return array;
	}

	private void OnDisable()
	{
		if (socket.Connected)
		{
			Debug.Log("断开连接");
			socket.Shutdown(SocketShutdown.Both);
			socket.Close();
		}
	}
}
