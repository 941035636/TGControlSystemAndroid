namespace FMSocketIO
{
	public class SocketIOEvent
	{
		public string name { get; set; }

		public string data { get; set; }

		public SocketIOEvent(string name)
		{
			this.name = name;
		}

		public SocketIOEvent(string name, string data)
		{
			this.name = name;
			this.data = data;
		}
	}
}
