using Verdant;
using Verdant.Physics;

namespace ChargeGame
{
	public class Gem : BoxEntity
	{
		public Gem(Vec2 position)
			: base(Resources.Gem, position, 10, 10, 1f)
		{
			AngleFriction = 1f;
			ZIndexMode = EntityManager.ZIndexMode.Bottom;
			Friction = 0.1f;
		}
	}
}

