using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;

public class WakeUpComputer : MonoBehaviour
{
	private static WakeUpComputer instance;

	private static readonly Regex MacCheckRegex = new Regex("^([0-9a-fA-F]{2})([\\s:/-]?[0-9a-fA-F]{2}){5}$");

	[Tooltip("可选。建议填写平板所在设备网段的定向广播地址，例如 192.168.86.255。留空时自动探测。")]
	public string directedBroadcastAddress = string.Empty;

	[Range(1, 5)]
	public int repeatCount = 3;

	private static readonly int[] WakePorts = { 9, 7, 50000 };

	private void Awake()
	{
		instance = this;
	}

	public static WakeUpComputer GetInstance()
	{
		if (instance == null)
		{
			instance = UnityEngine.Object.FindObjectOfType<WakeUpComputer>();
		}
		return instance;
	}

	public static bool WakeUp(string mac)
	{
		return SendMagicPackets(mac, null, 3);
	}

	public void Button_Click_WakeUp(string mac)
	{
		IPAddress configuredBroadcast = null;
		if (!string.IsNullOrWhiteSpace(directedBroadcastAddress) &&
			!IPAddress.TryParse(directedBroadcastAddress.Trim(), out configuredBroadcast))
		{
			UnityEngine.Debug.LogError("网络唤醒广播地址无效: " + directedBroadcastAddress);
			ControlDiagnostics.PublishStatus("网络唤醒失败：广播地址无效", true);
			return;
		}

		bool sent = SendMagicPackets(mac, configuredBroadcast, Mathf.Clamp(repeatCount, 1, 5));
		if (!sent)
		{
			UnityEngine.Debug.LogError("网络唤醒指令发送失败，MAC=" + mac);
		}
	}

	private static bool SendMagicPackets(string mac, IPAddress configuredBroadcast, int repeats)
	{
		string commandId = ControlDiagnostics.NewCommandId("WOL");
		if (string.IsNullOrWhiteSpace(mac) || !MacCheckRegex.IsMatch(mac.Trim()))
		{
			UnityEngine.Debug.LogError("网络唤醒 MAC 地址格式无效: " + mac);
			ControlDiagnostics.RecordCommand(commandId, "WOL", "MAGIC_PACKET", mac, "INVALID", "MAC format");
			ControlDiagnostics.PublishStatus("网络唤醒失败：MAC 地址无效", true);
			return false;
		}

		byte[] packet = BuildMagicPacket(ParseMac(mac));
		List<IPAddress> destinations = GetBroadcastDestinations(configuredBroadcast);
		bool sentAny = false;
		ControlDiagnostics.RecordCommand(commandId, "WOL", "MAGIC_PACKET", mac, "STARTED", "destinations=" + string.Join(",", destinations) + " repeats=" + repeats);

		foreach (IPAddress destination in destinations)
		{
			foreach (int wakePort in WakePorts)
			{
				try
				{
					using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
					{
						client.EnableBroadcast = true;
						client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
						IPEndPoint endpoint = new IPEndPoint(destination, wakePort);
						for (int i = 0; i < repeats; i++)
						{
							client.Send(packet, packet.Length, endpoint);
							sentAny = true;
						}
						ControlDiagnostics.RecordCommand(commandId, "WOL", "MAGIC_PACKET", destination + ":" + wakePort, "SOCKET_OK", mac + " repeats=" + repeats);
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogWarning("网络唤醒发送失败 " + destination + ":" + wakePort + " " + ex.Message);
					ControlDiagnostics.RecordCommand(commandId, "WOL", "MAGIC_PACKET", destination + ":" + wakePort, "FAILED", mac + " " + ex.Message);
				}
			}
		}

		if (sentAny)
		{
			UnityEngine.Debug.Log("网络唤醒魔术包已发送，MAC=" + mac + " 广播目标=" + string.Join(",", destinations));
		}
		ControlDiagnostics.RecordCommand(commandId, "WOL", "MAGIC_PACKET", mac, sentAny ? "COMPLETED" : "FAILED", "socket send result only");
		ControlDiagnostics.PublishStatus(sentAny
			? "网络唤醒包已发送（请观察设备是否启动）"
			: "网络唤醒失败：广播包未能发送", !sentAny);
		return sentAny;
	}

	private static List<IPAddress> GetBroadcastDestinations(IPAddress configuredBroadcast)
	{
		HashSet<string> unique = new HashSet<string>();
		List<IPAddress> result = new List<IPAddress>();

		AddDestination(IPAddress.Broadcast, unique, result);
		if (configuredBroadcast != null)
		{
			AddDestination(configuredBroadcast, unique, result);
		}

		try
		{
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.OperationalStatus != OperationalStatus.Up ||
					networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
				{
					continue;
				}

				foreach (UnicastIPAddressInformation addressInfo in networkInterface.GetIPProperties().UnicastAddresses)
				{
					if (addressInfo.Address.AddressFamily != AddressFamily.InterNetwork || addressInfo.IPv4Mask == null)
					{
						continue;
					}
					AddDestination(CalculateBroadcast(addressInfo.Address, addressInfo.IPv4Mask), unique, result);
				}
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning("无法自动计算定向广播地址，将使用 255.255.255.255: " + ex.Message);
		}
		return result;
	}

	private static void AddDestination(IPAddress address, HashSet<string> unique, List<IPAddress> result)
	{
		if (address != null && unique.Add(address.ToString()))
		{
			result.Add(address);
		}
	}

	private static IPAddress CalculateBroadcast(IPAddress address, IPAddress mask)
	{
		byte[] addressBytes = address.GetAddressBytes();
		byte[] maskBytes = mask.GetAddressBytes();
		byte[] broadcastBytes = new byte[4];
		for (int i = 0; i < broadcastBytes.Length; i++)
		{
			broadcastBytes[i] = (byte)(addressBytes[i] | ~maskBytes[i]);
		}
		return new IPAddress(broadcastBytes);
	}

	private static byte[] ParseMac(string mac)
	{
		string compact = Regex.Replace(mac, "[\\s:/-]", string.Empty);
		byte[] bytes = new byte[6];
		for (int i = 0; i < bytes.Length; i++)
		{
			bytes[i] = Convert.ToByte(compact.Substring(i * 2, 2), 16);
		}
		return bytes;
	}

	private static byte[] BuildMagicPacket(byte[] mac)
	{
		byte[] packet = new byte[102];
		for (int i = 0; i < 6; i++)
		{
			packet[i] = byte.MaxValue;
		}
		for (int repeat = 1; repeat <= 16; repeat++)
		{
			Buffer.BlockCopy(mac, 0, packet, repeat * 6, mac.Length);
		}
		return packet;
	}

	public void ShutDown(string ip)
	{
		string command = "/c shutdown -s -m \\\\" + ip + " -t 0";
		Process.Start("cmd.exe", command);
		UnityEngine.Debug.Log(command);
	}
}
