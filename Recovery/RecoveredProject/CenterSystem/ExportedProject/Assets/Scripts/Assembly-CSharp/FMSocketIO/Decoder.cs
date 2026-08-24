using System;
using System.Text;
using WebSocketSharp;

namespace FMSocketIO
{
	public class Decoder
	{
		public Packet Decode(MessageEventArgs e)
		{
			try
			{
				string data = e.Data;
				Packet packet = new Packet();
				int num = 0;
				if ((packet.enginePacketType = (EnginePacketType)int.Parse(data.Substring(num, 1))) == EnginePacketType.MESSAGE)
				{
					int socketPacketType = int.Parse(data.Substring(++num, 1));
					packet.socketPacketType = (SocketPacketType)socketPacketType;
				}
				if (data.Length <= 2)
				{
					return packet;
				}
				if (data[num + 1] == ',')
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (num < data.Length - 1 && data[++num] != ',')
					{
						stringBuilder.Append(data[num]);
					}
					packet.nsp = stringBuilder.ToString();
				}
				else
				{
					packet.nsp = "/";
				}
				char c = data[num + 1];
				if (c != ' ' && char.IsNumber(c))
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					while (num < data.Length - 1)
					{
						char c2 = data[++num];
						if (char.IsNumber(c2))
						{
							stringBuilder2.Append(c2);
							continue;
						}
						num--;
						break;
					}
					packet.id = int.Parse(stringBuilder2.ToString());
				}
				if (++num < data.Length - 1)
				{
					packet.json = data.Substring(num);
				}
				return packet;
			}
			catch (Exception innerException)
			{
				throw new SocketIOException("Packet decoding failed: " + e.Data, innerException);
			}
		}
	}
}
