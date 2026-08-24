using UnityEngine;

[SerializeField]
public class RelayModel
{
	private string relaykinds;

	private int channelNum;

	private string relayip;

	private int relayPort;

	private int AllChannelNum;

	private string relayname;

	private Order order;

	public int GetAllChannelNum
	{
		get
		{
			return AllChannelNum;
		}
		set
		{
			AllChannelNum = value;
		}
	}

	public string Getrelaykinds
	{
		get
		{
			return relaykinds;
		}
		set
		{
			relaykinds = value;
		}
	}

	public int GetChannelNum
	{
		get
		{
			return channelNum;
		}
		set
		{
			channelNum = value;
		}
	}

	public string GetrelayIp
	{
		get
		{
			return relayip;
		}
		set
		{
			relayip = value;
		}
	}

	public int GetRelayPort
	{
		get
		{
			return relayPort;
		}
		set
		{
			relayPort = value;
		}
	}

	public string GetrelayName
	{
		get
		{
			return relayname;
		}
		set
		{
			relayname = value;
		}
	}

	public Order GetOrder
	{
		get
		{
			return order;
		}
		set
		{
			order = value;
		}
	}
}
