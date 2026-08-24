using System;
using System.Text;

namespace FMSocketIO
{
	public class Encoder
	{
		public string Encode(Packet packet)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append((int)packet.enginePacketType);
				if (!packet.enginePacketType.Equals(EnginePacketType.MESSAGE))
				{
					return stringBuilder.ToString();
				}
				stringBuilder.Append((int)packet.socketPacketType);
				if (packet.socketPacketType == SocketPacketType.BINARY_EVENT || packet.socketPacketType == SocketPacketType.BINARY_ACK)
				{
					stringBuilder.Append(packet.attachments);
					stringBuilder.Append('-');
				}
				if (!string.IsNullOrEmpty(packet.nsp) && !packet.nsp.Equals("/"))
				{
					stringBuilder.Append(packet.nsp);
					stringBuilder.Append(',');
				}
				if (packet.id > -1)
				{
					stringBuilder.Append(packet.id);
				}
				if (packet.json != null && !packet.json.ToString().Equals("null"))
				{
					stringBuilder.Append(packet.json.ToString());
				}
				return stringBuilder.ToString();
			}
			catch (Exception innerException)
			{
				throw new SocketIOException("Packet encoding failed: " + packet, innerException);
			}
		}
	}
}
