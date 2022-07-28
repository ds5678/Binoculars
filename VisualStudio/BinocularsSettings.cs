using ModSettings;

namespace Binoculars
{
	internal sealed class BinocularsSettings : JsonModSettings
	{
		internal static BinocularsSettings Instance { get; } = new();
		
		[Name("Binocular Zoom")]
		[Slider(1, 20, 191)]
		public float zoom = 10;

		[Name("Start with Binoculars")]
		public bool startWithBinoculars = false;

		internal float GetFovScalar()
		{
			return 1 / zoom;
		}
	}
}
