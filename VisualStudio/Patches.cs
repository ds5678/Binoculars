extern alias Hinterland;
using HarmonyLib;
using Hinterland;

namespace Binoculars
{
	internal static class Patches
	{
		[HarmonyPatch(typeof(StartGear), "AddAllToInventory")]
		internal class StartGear_AddAllToInventory
		{
			private static void Postfix()
			{
				if (Settings.options.startWithBinoculars)
				{
					GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_Binoculars");
				}
			}
		}
	}
}
