using System;

namespace FMSocketIO
{
	[Serializable]
	public class SocketOpenData
	{
		public string sid;

		public string[] upgrades;

		public int pingInterval;

		public int pingTimeout;
	}
}
