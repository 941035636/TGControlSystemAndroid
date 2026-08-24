using System;
using System.Text;
using UnityEngine;

public class MsgBase
{
	public string protoName = "";

	public static byte[] Encode(MsgBase msgBase)
	{
		string s = JsonUtility.ToJson(msgBase);
		return Encoding.UTF8.GetBytes(s);
	}

	public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
	{
		return (MsgBase)JsonUtility.FromJson(Encoding.UTF8.GetString(bytes, offset, count), Type.GetType(protoName));
	}

	public static byte[] EncodeName(MsgBase msgBase)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(msgBase.protoName);
		short num = (short)bytes.Length;
		byte[] array = new byte[2 + num];
		array[0] = (byte)(num % 256);
		array[1] = (byte)(num / 256);
		Array.Copy(bytes, 0, array, 2, num);
		return array;
	}

	public static string DecodeName(byte[] bytes, int offset, out int count)
	{
		count = 0;
		if (offset + 2 > bytes.Length)
		{
			return "";
		}
		short num = (short)((bytes[offset + 1] << 8) | bytes[offset]);
		if (offset + 2 + num > bytes.Length)
		{
			return "";
		}
		count = 2 + num;
		return Encoding.UTF8.GetString(bytes, offset + 2, num);
	}
}
