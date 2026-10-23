using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

internal sealed class TestTcpController : ReliableTcpControlBase
{
	public void StartForTest()
	{
		Initialise(Eventdefine.None, Eventdefine.None, Eventdefine.None, "TEST");
	}

	public void StopForTest()
	{
		Shutdown();
	}

	public void SetSuspendedForTest(bool suspended)
	{
		SetApplicationSuspended(suspended);
	}

	public Coroutine QueueCoroutine => LastStartedCoroutine;
}

internal static class Program
{
	private static async Task Main()
	{
		var tests = new List<(string Name, Func<Task> Run)>
		{
			("connect-and-immediate-queue", TestImmediateQueue),
			("open-close-order", TestOpenCloseOrder),
			("disconnect-and-reconnect", TestReconnect),
			("application-suspend-and-resume", TestApplicationSuspendAndResume),
			("same-host-port-switch", TestPortSwitch),
			("rapid-50-command-queue", TestRapidQueue),
			("wol-magic-packet", TestWakeOnLanPacket)
		};

		int passed = 0;
		foreach ((string name, Func<Task> run) in tests)
		{
			try
			{
				await run();
				passed++;
				Console.WriteLine("PASS " + name);
			}
			catch (Exception ex)
			{
				Console.WriteLine("FAIL " + name + " :: " + ex);
			}
		}

		Console.WriteLine($"RESULT {passed}/{tests.Count} passed");
		if (passed != tests.Count)
		{
			Environment.ExitCode = 1;
		}
	}

	private static async Task TestImmediateQueue()
	{
		using var listener = StartListener(out int port);
		using var controller = new ControllerScope();
		Task<TcpClient> accepted = listener.AcceptTcpClientAsync();
		controller.Value.Connect("127.0.0.1", port);
		controller.Value.SendSingleCode("AA 01", "127.0.0.1");
		using TcpClient peer = await PumpForResult(controller.Value, accepted);
		byte[] received = await ReadWithPump(controller.Value, peer, 2);
		AssertBytes(new byte[] { 0xAA, 0x01 }, received, "immediate queued command");
	}

	private static async Task TestOpenCloseOrder()
	{
		using var listener = StartListener(out int port);
		using var controller = new ControllerScope();
		Task<TcpClient> accepted = listener.AcceptTcpClientAsync();
		controller.Value.Connect("127.0.0.1", port);
		controller.Value.Send("01 02", "03 04", "127.0.0.1");
		using TcpClient peer = await PumpForResult(controller.Value, accepted);
		byte[] received = await ReadWithPump(controller.Value, peer, 4);
		AssertBytes(new byte[] { 0x01, 0x02, 0x03, 0x04 }, received, "open/close ordering");
	}

	private static async Task TestReconnect()
	{
		using var listener = StartListener(out int port);
		using var controller = new ControllerScope();
		Task<TcpClient> firstAccept = listener.AcceptTcpClientAsync();
		controller.Value.Connect("127.0.0.1", port);
		controller.Value.SendSingleCode("10 01", "127.0.0.1");
		using (TcpClient firstPeer = await PumpForResult(controller.Value, firstAccept))
		{
			AssertBytes(new byte[] { 0x10, 0x01 }, await ReadWithPump(controller.Value, firstPeer, 2), "first connection");
			firstPeer.Client.LingerState = new LingerOption(true, 0);
		}

		await PumpSteps(controller.Value, 50);
		Task<TcpClient> secondAccept = listener.AcceptTcpClientAsync();
		controller.Value.SendSingleCode("20 02", "127.0.0.1");
		using TcpClient secondPeer = await PumpForResult(controller.Value, secondAccept);
		AssertBytes(new byte[] { 0x20, 0x02 }, await ReadWithPump(controller.Value, secondPeer, 2), "reconnected command");
	}

	private static async Task TestApplicationSuspendAndResume()
	{
		using var listener = StartListener(out int port);
		using var controller = new ControllerScope();
		Task<TcpClient> firstAccept = listener.AcceptTcpClientAsync();
		controller.Value.Connect("127.0.0.1", port);
		controller.Value.SendSingleCode("41", "127.0.0.1");
		using (TcpClient firstPeer = await PumpForResult(controller.Value, firstAccept))
		{
			AssertBytes(new byte[] { 0x41 }, await ReadWithPump(controller.Value, firstPeer, 1), "command before suspend");
			controller.Value.SetSuspendedForTest(true);
		}

		Task<TcpClient> resumedAccept = listener.AcceptTcpClientAsync();
		controller.Value.SendSingleCode("42", "127.0.0.1");
		await PumpSteps(controller.Value, 200);
		Assert(!resumedAccept.IsCompleted, "controller must not reconnect while application is suspended");

		controller.Value.SetSuspendedForTest(false);
		using TcpClient resumedPeer = await PumpForResult(controller.Value, resumedAccept);
		AssertBytes(new byte[] { 0x42 }, await ReadWithPump(controller.Value, resumedPeer, 1), "queued command after resume");
	}

	private static async Task TestPortSwitch()
	{
		using var firstListener = StartListener(out int firstPort);
		using var secondListener = StartListener(out int secondPort);
		using var controller = new ControllerScope();

		Task<TcpClient> firstAccept = firstListener.AcceptTcpClientAsync();
		controller.Value.Connect("127.0.0.1", firstPort);
		controller.Value.SendSingleCode("31", "127.0.0.1");
		using (TcpClient firstPeer = await PumpForResult(controller.Value, firstAccept))
		{
			AssertBytes(new byte[] { 0x31 }, await ReadWithPump(controller.Value, firstPeer, 1), "first port");
		}

		Task<TcpClient> secondAccept = secondListener.AcceptTcpClientAsync();
		controller.Value.Connect("127.0.0.1", secondPort);
		controller.Value.SendSingleCode("32", "127.0.0.1");
		using TcpClient secondPeer = await PumpForResult(controller.Value, secondAccept);
		AssertBytes(new byte[] { 0x32 }, await ReadWithPump(controller.Value, secondPeer, 1), "second port");
	}

	private static async Task TestRapidQueue()
	{
		using var listener = StartListener(out int port);
		using var controller = new ControllerScope();
		Task<TcpClient> accepted = listener.AcceptTcpClientAsync();
		controller.Value.Connect("127.0.0.1", port);

		var expected = new List<byte>();
		for (int i = 0; i < 50; i++)
		{
			byte first = (byte)i;
			byte second = (byte)(255 - i);
			expected.Add(first);
			expected.Add(second);
			controller.Value.SendSingleCode(first.ToString("X2") + second.ToString("X2"), "127.0.0.1");
		}

		using TcpClient peer = await PumpForResult(controller.Value, accepted);
		byte[] received = await ReadWithPump(controller.Value, peer, expected.Count, 8000);
		AssertBytes(expected.ToArray(), received, "rapid queue");
	}

	private static Task TestWakeOnLanPacket()
	{
		Type type = typeof(WakeUpComputer);
		BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
		var regex = (System.Text.RegularExpressions.Regex)type.GetField("MacCheckRegex", flags).GetValue(null);
		Assert(regex.IsMatch("00-11-22-33-44-55"), "hyphen MAC should be valid");
		Assert(regex.IsMatch("00:11:22:33:44:55"), "colon MAC should be valid");
		Assert(regex.IsMatch("001122334455"), "compact MAC should be valid");
		Assert(!regex.IsMatch("00:11:22:33:44"), "short MAC should be invalid");

		var parse = type.GetMethod("ParseMac", flags);
		var build = type.GetMethod("BuildMagicPacket", flags);
		var calculateBroadcast = type.GetMethod("CalculateBroadcast", flags);
		byte[] mac = (byte[])parse.Invoke(null, new object[] { "00:11:22:33:44:55" });
		byte[] packet = (byte[])build.Invoke(null, new object[] { mac });
		Assert(packet.Length == 102, "magic packet length must be 102");
		Assert(packet.Take(6).All(value => value == 0xFF), "magic packet prefix must be FF x6");
		for (int repeat = 0; repeat < 16; repeat++)
		{
			AssertBytes(mac, packet.Skip(6 + repeat * 6).Take(6).ToArray(), "magic packet MAC repeat " + repeat);
		}

		IPAddress broadcast = (IPAddress)calculateBroadcast.Invoke(null, new object[]
		{
			IPAddress.Parse("192.168.86.123"),
			IPAddress.Parse("255.255.255.0")
		});
		Assert(broadcast.ToString() == "192.168.86.255", "directed broadcast calculation");
		return Task.CompletedTask;
	}

	private static TcpListener StartListener(out int port)
	{
		var listener = new TcpListener(IPAddress.Loopback, 0);
		listener.Start();
		port = ((IPEndPoint)listener.LocalEndpoint).Port;
		return listener;
	}

	private static async Task<T> PumpForResult<T>(TestTcpController controller, Task<T> task, int maxSteps = 2000)
	{
		for (int i = 0; i < maxSteps && !task.IsCompleted; i++)
		{
			PumpOnce(controller);
			await Task.Delay(2);
		}
		if (!task.IsCompleted)
		{
			throw new TimeoutException("operation did not complete while pumping transport");
		}
		return await task;
	}

	private static async Task<byte[]> ReadWithPump(TestTcpController controller, TcpClient peer, int expectedLength, int maxSteps = 2000)
	{
		using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(15));
		Task<byte[]> read = ReadExact(peer.GetStream(), expectedLength, cancellation.Token);
		return await PumpForResult(controller, read, maxSteps);
	}

	private static async Task<byte[]> ReadExact(NetworkStream stream, int length, CancellationToken cancellationToken)
	{
		byte[] result = new byte[length];
		int offset = 0;
		while (offset < result.Length)
		{
			int count = await stream.ReadAsync(result.AsMemory(offset, result.Length - offset), cancellationToken);
			if (count == 0)
			{
				throw new InvalidOperationException("peer closed before expected data was received");
			}
			offset += count;
		}
		return result;
	}

	private static async Task PumpSteps(TestTcpController controller, int steps)
	{
		for (int i = 0; i < steps; i++)
		{
			PumpOnce(controller);
			await Task.Delay(2);
		}
	}

	private static void PumpOnce(TestTcpController controller)
	{
		Time.realtimeSinceStartup += 0.05f;
		if (!controller.QueueCoroutine.Routine.MoveNext())
		{
			throw new InvalidOperationException("transport queue coroutine ended unexpectedly");
		}
	}

	private static void AssertBytes(byte[] expected, byte[] actual, string label)
	{
		if (!expected.SequenceEqual(actual))
		{
			throw new InvalidOperationException(label + " expected=" + Convert.ToHexString(expected) + " actual=" + Convert.ToHexString(actual));
		}
	}

	private static void Assert(bool condition, string message)
	{
		if (!condition)
		{
			throw new InvalidOperationException(message);
		}
	}

	private sealed class ControllerScope : IDisposable
	{
		public ControllerScope()
		{
			Value = new TestTcpController();
			Value.StartForTest();
		}

		public TestTcpController Value { get; }

		public void Dispose()
		{
			Value.StopForTest();
		}
	}
}
