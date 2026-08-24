public class GameData
{
	private static GameData instance;

	public bool IsDebugWrite = true;

	public string GameLevelName = "Start";

	public string curUser;

	public static GameData Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameData();
			}
			return instance;
		}
	}
}
