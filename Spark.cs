using Verdant;

namespace ChargeGame
{
	public class Spark : Entity
	{
		public Spark(Vec2 position)
			: base(Resources.Spark, position)
		{
			ZIndexMode = EntityManager.ZIndexMode.Manual;
			ZIndex = int.MaxValue;

			// delete self when animation is done
			((Animation)Sprite).OnComplete = (sender) =>
			{
				ForRemoval = true;
			};
		}
	}
}

