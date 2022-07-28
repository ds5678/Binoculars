using MelonLoader;

namespace Binoculars;

internal sealed class BinocularsMod : MelonMod
{
	public override void OnApplicationStart()
	{
		BinocularsSettings.Instance.AddToModSettings("Binoculars");
	}
}
