using Microsoft.Xna.Framework.Graphics;
using Verdant;
using Verdant.Physics;

namespace ChargeGame
{
	public class Boundary : WallEntity
	{

		public Vec2 Start { get; private set; }
		public Vec2 End { get; private set; }

		public Boundary(Vec2 start, Vec2 end)
			: base(start, end)
		{
			BodyColor = new Microsoft.Xna.Framework.Color(
				GameMath.Random.Next(100, 256),
				GameMath.Random.Next(100, 256),
				GameMath.Random.Next(100, 256));

			Start = start;
			End = end;
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
			//base.DrawBody(spriteBatch);
        }
    }
}

