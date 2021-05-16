using ModSettings;

namespace Binoculars
{
	internal class BinocularsSettings : JsonModSettings
	{
		[Name("Binocular Zoom")]
		[Slider(1, 20, 191)]
		public float zoom = 10;

		[Name("Start with Binoculars")]
		public bool startWithBinoculars = false;
	}
	internal static class Settings
	{
		internal static BinocularsSettings options;
		internal static void OnLoad()
		{
			options = new BinocularsSettings();
			options.AddToModSettings("Binoculars");
		}
		internal static float GetFovScalar()
		{
			return 1 / options.zoom;
		}
	}
}
