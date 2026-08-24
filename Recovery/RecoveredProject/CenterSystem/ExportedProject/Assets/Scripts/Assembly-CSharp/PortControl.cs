using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using UnityEngine;

// Serial-port controller recovered from the original Mono assembly.
public class PortControl : MonoBehaviour
{
	public string portName = "COM3";

	public int baudRate = 9600;

	public Parity parity;

	public int dataBits = 8;

	public StopBits stopBits = StopBits.One;

	private SerialPort sp;

	private Thread dataReceiveThread;

	private volatile bool receiveRunning;

	private string message = "";

	public List<byte> listReceive = new List<byte>();

	private char[] strchar = new char[100];

	private string str;

	private void Start()
	{
	}

	public void OpenPortEvn()
	{
		OpenPort();
		if (sp != null && sp.IsOpen && (dataReceiveThread == null || !dataReceiveThread.IsAlive))
		{
			receiveRunning = true;
			dataReceiveThread = new Thread(DataReceiveFunction)
			{
				IsBackground = true,
				Name = "SerialPort-Receive"
			};
			dataReceiveThread.Start();
		}
	}

	public void OpenPort()
	{
		sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
		sp.ReadTimeout = 400;
		try
		{
			sp.Open();
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}

	private void OnApplicationQuit()
	{
		ClosePort();
	}

	public void ClosePort()
	{
		receiveRunning = false;
		try
		{
			if (sp != null && sp.IsOpen)
			{
				sp.Close();
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
		if (dataReceiveThread != null && dataReceiveThread.IsAlive && Thread.CurrentThread != dataReceiveThread)
		{
			dataReceiveThread.Join(500);
		}
		dataReceiveThread = null;
		if (sp != null)
		{
			sp.Dispose();
			sp = null;
		}
	}

	private void PrintData()
	{
		for (int i = 0; i < listReceive.Count; i++)
		{
			strchar[i] = (char)listReceive[i];
			str = new string(strchar);
		}
		Debug.Log(str);
	}

	private void DataReceiveFunction()
	{
		byte[] array = new byte[1024];
		while (receiveRunning)
		{
			if (sp != null && sp.IsOpen)
			{
				try
				{
					if (sp.Read(array, 0, array.Length) == 0)
					{
						continue;
					}
					Debug.Log(Encoding.Default.GetString(array));
					goto IL_005d;
				}
				catch (TimeoutException)
				{
				}
				catch (Exception ex)
				{
					if (receiveRunning)
					{
						Debug.LogWarning("串口接收中断: " + ex.Message);
					}
					goto IL_005d;
				}
			}
			goto IL_005d;
			IL_005d:
			Thread.Sleep(10);
		}
	}

	public void WriteData(string dataStr)
	{
		if (sp != null && sp.IsOpen)
		{
			sp.Write(dataStr);
		}
	}

	private void Update()
	{
		string dataStr = "AA BB 01 12345 01AB 0@ab 发送";
		if (Input.GetKeyDown(KeyCode.W))
		{
			WriteData(dataStr);
		}
	}
}
