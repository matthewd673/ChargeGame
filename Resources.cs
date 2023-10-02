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

		public static void LoadResources(ContentManager content)
		{
			Player = content.Load<Texture2D>("player");
			Enemy = content.Load<Texture2D>("enemy");
			Wall = content.Load<Texture2D>("wall");
		}
	}
}

