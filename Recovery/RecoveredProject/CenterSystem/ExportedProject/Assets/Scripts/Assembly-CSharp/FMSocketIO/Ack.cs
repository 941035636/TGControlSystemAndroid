using System;

namespace FMSocketIO
{
	public class Ack
	{
		public int packetId;

		public DateTime time;

		private Action<string> action;

		public Ack(int packetId, Action<string> action)
		{
			this.packetId = packetId;
			time = DateTime.Now;
			this.action = action;
		}

		public void Invoke(string ev)
		{
			action(ev);
		}

		public override string ToString()
		{
			return $"[Ack: packetId={packetId}, time={time}, action={action}]";
		}
	}
}
