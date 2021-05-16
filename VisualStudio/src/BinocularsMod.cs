using MelonLoader;
using UnityEngine;

namespace Binoculars
{
	public static class BuildInfo
	{
		public const string Name = "Binoculars"; // Name of the Mod.  (MUST BE SET)
		public const string Description = "A mod to let you see farther."; // Description for the Mod.  (Set as null if none)
		public const string Author = "WulfMarius, ds5678"; // Author of the Mod.  (MUST BE SET)
		public const string Company = null; // Company that made the Mod.  (Set as null if none)
		public const string Version = "4.3.0"; // Version of the Mod.  (MUST BE SET)
		public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
	}
	internal class BinocularsMod : MelonMod
	{
		public override void OnApplicationStart()
		{
			Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
			Settings.OnLoad();
		}

		internal static void Log(string message, params object[] parameters) => MelonLogger.Log(message, parameters);
		internal static void LogWarning(string message, params object[] parameters) => MelonLogger.LogWarning(message, parameters);
		internal static void LogError(string message, params object[] parameters) => MelonLogger.LogError(message, parameters);
	}
}
