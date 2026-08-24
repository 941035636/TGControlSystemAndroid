public class AccountModel
{
	private string name;

	private string account;

	private string pwd;

	private double old;

	private string phone;

	private string TestTime;

	private int sex;

	private int birthYear;

	private int birthMonth;

	private int birthDay;

	private double Height;

	private double Weight;

	private int type;

	public string testtime
	{
		get
		{
			return TestTime;
		}
		set
		{
			TestTime = value;
		}
	}

	public double height
	{
		get
		{
			return Height;
		}
		set
		{
			Height = value;
		}
	}

	public double weight
	{
		get
		{
			return Weight;
		}
		set
		{
			Weight = value;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public string Account
	{
		get
		{
			return account;
		}
		set
		{
			account = value;
		}
	}

	public string Pwd
	{
		get
		{
			return pwd;
		}
		set
		{
			pwd = value;
		}
	}

	public double Old
	{
		get
		{
			return old;
		}
		set
		{
			old = value;
		}
	}

	public string Phone
	{
		get
		{
			return phone;
		}
		set
		{
			phone = value;
		}
	}

	public int Sex
	{
		get
		{
			return sex;
		}
		set
		{
			sex = value;
		}
	}

	public int BirthYear
	{
		get
		{
			return birthYear;
		}
		set
		{
			birthYear = value;
		}
	}

	public int BirthDay
	{
		get
		{
			return birthDay;
		}
		set
		{
			birthDay = value;
		}
	}

	public int BirthMonth
	{
		get
		{
			return birthMonth;
		}
		set
		{
			birthMonth = value;
		}
	}

	public int Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	public AccountModel()
	{
		account = "";
		pwd = "";
		name = "";
		old = 0.0;
		phone = "";
		TestTime = "";
		Sex = 0;
		BirthYear = 0;
		BirthMonth = 0;
		BirthDay = 0;
		Type = 0;
		Height = 0.0;
		Weight = 0.0;
	}
}
