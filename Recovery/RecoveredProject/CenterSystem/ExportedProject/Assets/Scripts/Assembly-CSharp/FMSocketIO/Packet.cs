namespace FMSocketIO
{
	public class Packet
	{
		public EnginePacketType enginePacketType;

		public SocketPacketType socketPacketType;

		public int attachments;

		public string nsp;

		public int id;

		public string json;

		public Packet()
			: this(EnginePacketType.UNKNOWN)
		{
		}

		public Packet(EnginePacketType enginePacketType)
			: this(enginePacketType, SocketPacketType.UNKNOWN, -1, "/", -1, "")
		{
		}

		public Packet(EnginePacketType enginePacketType, SocketPacketType socketPacketType, int attachments, string nsp, int id, string json)
		{
			this.enginePacketType = enginePacketType;
			this.socketPacketType = socketPacketType;
			this.attachments = attachments;
			this.nsp = nsp;
			this.id = id;
			this.json = json;
		}
	}
}
