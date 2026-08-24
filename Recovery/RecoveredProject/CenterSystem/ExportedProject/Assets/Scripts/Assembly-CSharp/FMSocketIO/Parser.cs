namespace FMSocketIO
{
	public class Parser
	{
		public SocketIOEvent Parse(string json)
		{
			string[] array = json.Split(new char[1] { ',' }, 2);
			string name = array[0].Substring(2, array[0].Length - 3);
			if (array.Length == 1)
			{
				return new SocketIOEvent(name);
			}
			return new SocketIOEvent(name, array[1].TrimEnd(']'));
		}

		public string ParseData(string json)
		{
			return json.Substring(1, json.Length - 2);
		}
	}
}
