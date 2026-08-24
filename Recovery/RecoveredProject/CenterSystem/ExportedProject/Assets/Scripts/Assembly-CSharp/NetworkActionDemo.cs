using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NetworkActionDemo : MonoBehaviour
{
	public GameObject[] NetworkObjects;

	public Image demoImg;

	public Text demoText;

	public void Action_ProcessRpcMessage(string RpcMsg)
	{
		if (!RpcMsg.Contains("Cmd"))
		{
			return;
		}
		string text = RpcMsg.Substring(3, RpcMsg.Length - 3);
		if (text.Contains(","))
		{
			string[] array = text.Split(',');
			if (array[1].Contains("(bool)"))
			{
				bool flag = bool.Parse(array[1].Substring(6, array[1].Length - 6));
				GameObject[] networkObjects = NetworkObjects;
				for (int i = 0; i < networkObjects.Length; i++)
				{
					networkObjects[i].SendMessage(array[0], flag, SendMessageOptions.DontRequireReceiver);
				}
			}
			if (array[1].Contains("(int)"))
			{
				int num = int.Parse(array[1].Substring(5, array[1].Length - 5));
				GameObject[] networkObjects = NetworkObjects;
				for (int i = 0; i < networkObjects.Length; i++)
				{
					networkObjects[i].SendMessage(array[0], num, SendMessageOptions.DontRequireReceiver);
				}
			}
			if (array[1].Contains("(float)"))
			{
				float num2 = float.Parse(array[1].Substring(7, array[1].Length - 7));
				GameObject[] networkObjects = NetworkObjects;
				for (int i = 0; i < networkObjects.Length; i++)
				{
					networkObjects[i].SendMessage(array[0], num2, SendMessageOptions.DontRequireReceiver);
				}
			}
			if (array[1].Contains("(string)"))
			{
				string value = array[1].Substring(8, array[1].Length - 8);
				GameObject[] networkObjects = NetworkObjects;
				for (int i = 0; i < networkObjects.Length; i++)
				{
					networkObjects[i].SendMessage(array[0], value, SendMessageOptions.DontRequireReceiver);
				}
			}
			if (array[1].Contains("(Vector3)"))
			{
				string[] array2 = array[1].Substring(9, array[1].Length - 9).Split(':');
				Vector3 vector = new Vector3(float.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]));
				GameObject[] networkObjects = NetworkObjects;
				for (int i = 0; i < networkObjects.Length; i++)
				{
					networkObjects[i].SendMessage(array[0], vector, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else
		{
			GameObject[] networkObjects = NetworkObjects;
			for (int i = 0; i < networkObjects.Length; i++)
			{
				networkObjects[i].SendMessage(text, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void Rpc_SetInt(int _value)
	{
		NetworkActionClient.instance.Action_RpcSend("Action_SetInt,(int)" + _value);
	}

	public void Rpc_SetFloat(float _value)
	{
		NetworkActionClient.instance.Action_RpcSend("Action_SetFloat,(float)" + _value);
	}

	public void Rpc_SetBool(bool _value)
	{
		NetworkActionClient.instance.Action_RpcSend("Action_SetBool,(bool)" + _value);
	}

	public void Rpc_SetString(string _value)
	{
		NetworkActionClient.instance.Action_RpcSend("Action_SetString,(string)" + _value);
	}

	public void Rpc_SetVector3(Vector3 _value)
	{
		NetworkActionClient.instance.Action_RpcSend("Action_SetVector3,(Vector3)" + _value.x + ":" + _value.y + ":" + _value.z);
	}

	public void Rpc_SetVector3Random()
	{
		Rpc_SetVector3(new Vector3(Random.value, Random.value, Random.value));
	}

	public void Cmd_ServerSendByte()
	{
		if (NetworkActionServer.instance != null)
		{
			float value = Random.value;
			NetworkActionServer.instance.Action_AddCmd(Encoding.ASCII.GetBytes("Hello World: " + value));
		}
	}

	public void Action_ProcessByteData(byte[] _data)
	{
		if (_data.Length <= 100)
		{
			demoText.text = "[Decode Bytes] " + Encoding.ASCII.GetString(_data);
		}
	}

	private void Action_SetColorRed()
	{
		demoImg.color = Color.red;
	}

	private void Action_SetColorGreen()
	{
		demoImg.color = Color.green;
	}

	private void Action_SetInt(int _value)
	{
		demoText.text = "[Receive] demo int: " + _value;
	}

	private void Action_SetFloat(float _value)
	{
		demoText.text = "[Receive] demo float: " + _value;
	}

	private void Action_SetBool(bool _value)
	{
		demoText.text = "[Receive] demo bool: " + _value;
	}

	private void Action_SetString(string _value)
	{
		demoText.text = "[Receive] demo string: " + _value;
	}

	private void Action_SetVector3(Vector3 _value)
	{
		demoText.text = "[Receive] demo Vector3: (" + _value.x + ", " + _value.y + ", " + _value.z + ")";
	}
}
