using Microsoft.Xna.Framework.Graphics;
using Verdant;
namespace ChargeGame
{
	public class Wall : Entity
	{
		public const int WallWidth = 10;
		public const int WallHeight = 10;

		public enum WallStyle
		{
			Bottom	= 0b_0000_0001,
			Top		= 0b_0000_0010,
			Right	= 0b_0000_0100,
			Left	= 0b_0000_1000,
		}

		private WallStyle style;

		public Wall(Vec2 position, WallStyle style)
			: base(Resources.WallSheet, position, WallWidth, WallHeight)
		{
			this.style = style;
			ZIndexMode = EntityManager.ZIndexMode.Bottom;
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
			Microsoft.Xna.Framework.Rectangle bounds = Manager.Scene.Camera.GetRenderBounds(this);

			// try to draw every type of wall
			for (int i = 0; i < 4; i++)
			{
				if (((int)style & 0x01 << i) > 0)
				{
					Sprite.DrawIndex(spriteBatch, new Microsoft.Xna.Framework.Rectangle(
						bounds.X - bounds.Width / 2,
						bounds.Y - bounds.Height / 2,
						bounds.Width,
						bounds.Height), i);
				}
			}
        }
    }
}

