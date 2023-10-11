using Verdant;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ChargeGame
{
	public static class Resources
	{
		public static Sprite Player { get; private set; }
		public static Sprite Enemy { get; private set; }
		public static Sprite Wall { get; private set; }

		public static Animation PlayerIdle { get; private set; }
		public static Sprite Slash { get; private set; }
		public static Sprite Aim { get; private set; }
		public static Sprite AimHead { get; private set; }

		public static Sprite DemonPumpkin { get; private set; }
		public static Sprite PumpkinLightStatic { get; private set; }
		public static Animation PumpkinLightFlicker { get; private set; }

		public static Sprite Cursor { get; private set; }

		public static void LoadResources(ContentManager content)
		{
			Player = content.Load<Texture2D>("player");
			Enemy = content.Load<Texture2D>("enemy");
			Wall = content.Load<Texture2D>("wall");

			PlayerIdle = new(content.Load<Texture2D>("player_idle"),
							 12, 24, looping: true);
			Slash = content.Load<Texture2D>("slash");
			Aim = content.Load<Texture2D>("aim");
			AimHead = content.Load<Texture2D>("aim_head");

			DemonPumpkin = content.Load<Texture2D>("demon_pumpkin");
			PumpkinLightStatic = content.Load<Texture2D>("pumpkin_light_static");
			PumpkinLightFlicker = new(content.Load<Texture2D>("pumpkin_light_flicker"),
									  49, 24, looping: true);

			Cursor = content.Load<Texture2D>("cursor");
		}
	}
}

