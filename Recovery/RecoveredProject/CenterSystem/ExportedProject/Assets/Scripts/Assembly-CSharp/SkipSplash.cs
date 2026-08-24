using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class SkipSplash
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void BeforeSplashScreen()
	{
		Task.Run((Action)AsyncSkip);
	}

	private static void AsyncSkip()
	{
		SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
	}
}
