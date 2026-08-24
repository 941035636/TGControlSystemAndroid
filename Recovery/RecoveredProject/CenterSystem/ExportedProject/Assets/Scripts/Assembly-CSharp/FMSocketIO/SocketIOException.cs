using System;

namespace FMSocketIO
{
	public class SocketIOException : Exception
	{
		public SocketIOException()
		{
		}

		public SocketIOException(string message)
			: base(message)
		{
		}

		public SocketIOException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
