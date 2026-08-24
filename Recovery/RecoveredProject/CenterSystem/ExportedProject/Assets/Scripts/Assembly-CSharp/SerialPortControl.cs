using System;
using System.Globalization;
using System.IO.Ports;
using System.Text;
using System.Threading;
using UnityEngine;

public class SerialPortControl
{
	public static string portName;

	public static int baudRate;

	public static Parity parity;

	public static int dataBit;

	public static StopBits stopBit;

	private static SerialPort sp = new SerialPort();

	private byte[] buffer = new byte[5];

	private int bytes;

	public static string PortName(string portname)
	{
		portName = portname;
		return portName;
	}

	public static int BauRate(int baud)
	{
		baudRate = baud;
		return baudRate;
	}

	public static Parity Paritys(Parity paritys)
	{
		parity = paritys;
		return parity;
	}

	public static int DataBits(int dataBits)
	{
		dataBit = dataBits;
		return dataBit;
	}

	public static StopBits StopBitss(StopBits stopBits)
	{
		stopBit = stopBits;
		return stopBit;
	}

	public static string BytesTohexString(byte[] bytes)
	{
		if (bytes == null || bytes.Length < 1)
		{
			return string.Empty;
		}
		int num = bytes.Length;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("0x");
		for (int i = 0; i < num; i++)
		{
			string text = Convert.ToString(bytes[i], 16).ToUpper();
			stringBuilder.Append((text.Length == 1) ? ("0" + text) : text);
		}
		return stringBuilder.ToString();
	}

	public static byte[] HexStringToBytes(string hexStr)
	{
		if (string.IsNullOrEmpty(hexStr))
		{
			return new byte[0];
		}
		if (hexStr.StartsWith("0x"))
		{
			hexStr = hexStr.Remove(0, 2);
		}
		int num = hexStr.Length / 2;
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			byte b = byte.Parse(hexStr.Substring(2 * i, 2), NumberStyles.HexNumber);
			array[i] = b;
		}
		return array;
	}

	public static byte[] Convert16(string strText)
	{
		strText = strText.Replace(" ", "");
		byte[] array = new byte[strText.Length / 2];
		for (int i = 0; i < strText.Length / 2; i++)
		{
			array[i] = Convert.ToByte(Convert.ToInt32(strText.Substring(i * 2, 2), 16));
		}
		return array;
	}

	public static void OpenPort()
	{
		sp = new SerialPort(portName, baudRate, parity, dataBit, stopBit);
		sp.ReadTimeout = 1000;
		try
		{
			sp.Open();
			Debug.Log("打开端口成功");
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}

	public static void ClosePort()
	{
		try
		{
			sp.Close();
			sp.Dispose();
			Debug.Log("关闭端口");
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}

	public void DataReceiveFun()
	{
		while (true)
		{
			if (sp != null && sp.IsOpen)
			{
				try
				{
					bytes = sp.Read(buffer, 0, buffer.Length);
					sp.DiscardOutBuffer();
					if (bytes == 0)
					{
						continue;
					}
					for (int i = 0; i < buffer.Length; i++)
					{
						Debug.Log(buffer[i].ToString("x8"));
					}
					goto IL_007d;
				}
				catch (Exception message)
				{
					Debug.Log(message);
					goto IL_007d;
				}
			}
			goto IL_007d;
			IL_007d:
			Thread.Sleep(500);
		}
	}

	public static void SendData(byte[] dataStr)
	{
		sp.Write(dataStr, 0, dataStr.Length);
		Debug.LogWarning("发送成功");
	}
}
