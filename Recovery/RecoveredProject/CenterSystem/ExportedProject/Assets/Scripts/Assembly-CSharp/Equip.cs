public class Equip
{
	private string Name;

	private string Controlway;

	private string ip;

	private int Port;

	private int com;

	public string GetControlWay
	{
		get
		{
			return Controlway;
		}
		set
		{
			Controlway = value;
		}
	}

	public string GetName
	{
		get
		{
			return Name;
		}
		set
		{
			Name = value;
		}
	}

	public string GetIP
	{
		get
		{
			return ip;
		}
		set
		{
			ip = value;
		}
	}

	public int GetPort
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

	public int GetCom
	{
		get
		{
			return com;
		}
		set
		{
			com = value;
		}
	}
}
