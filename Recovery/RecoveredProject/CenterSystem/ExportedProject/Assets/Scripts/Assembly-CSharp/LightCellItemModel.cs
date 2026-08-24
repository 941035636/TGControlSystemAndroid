public class LightCellItemModel
{
	private string relayName;

	private string CellItemName;

	private string ParentName;

	private string TingName;

	private string DiscribName;

	private string IPadress;

	private string Port;

	private uint ChannelNum;

	private bool IsOpen;

	private string Coder;

	public string Get_ParentName
	{
		get
		{
			return ParentName;
		}
		set
		{
			ParentName = value;
		}
	}

	public string Get_CellItemName
	{
		get
		{
			return CellItemName;
		}
		set
		{
			CellItemName = value;
		}
	}

	public string Get_TingName
	{
		get
		{
			return TingName;
		}
		set
		{
			TingName = value;
		}
	}

	public string Get_DiscribName
	{
		get
		{
			return DiscribName;
		}
		set
		{
			DiscribName = value;
		}
	}

	public string Get_IPadress
	{
		get
		{
			return IPadress;
		}
		set
		{
			IPadress = value;
		}
	}

	public string Get_Port
	{
		get
		{
			return Port;
		}
		set
		{
			Port = value;
		}
	}

	public string Get_RelayName
	{
		get
		{
			return relayName;
		}
		set
		{
			relayName = value;
		}
	}

	public uint Get_Channel
	{
		get
		{
			return ChannelNum;
		}
		set
		{
			ChannelNum = value;
		}
	}

	public bool Get_IsOpen
	{
		get
		{
			return IsOpen;
		}
		set
		{
			IsOpen = value;
		}
	}

	public string Get_Coder
	{
		get
		{
			return Coder;
		}
		set
		{
			Coder = value;
		}
	}
}
