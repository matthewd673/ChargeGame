using Verdant;
using Verdant.Physics;

namespace ChargeGame
{
	public class Wall : BoxEntity
	{
		public Wall(Vec2 position) : base(Resources.Wall, position, 16, 16, 0)
		{

		}
	}
}

