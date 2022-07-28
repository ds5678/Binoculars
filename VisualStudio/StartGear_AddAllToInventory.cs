extern alias Hinterland;
using HarmonyLib;
using Hinterland;

namespace Binoculars;

[HarmonyPatch(typeof(StartGear), nameof(StartGear.AddAllToInventory))]
internal static class StartGear_AddAllToInventory
{
	private static void Postfix()
	{
		if (BinocularsSettings.Instance.startWithBinoculars)
		{
			GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_Binoculars");
		}
	}
}
