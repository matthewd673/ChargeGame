using Verdant;
using Verdant.Physics;

namespace ChargeGame
{
	public class DemonPumpkin : BoxEntity
	{
		public DemonPumpkin(Vec2 position)
			: base(Resources.DemonPumpkin, position, 33, 21, 0f)
		{
			AngleFriction = 1f;
			ZIndexMode = EntityManager.ZIndexMode.Bottom;
		}

        public override void OnAdd()
        {
            base.OnAdd();
			Manager.AddEntity(new PumpkinLight(Position + new Vec2(0, 8)));
        }
    }

	class PumpkinLight : Entity
	{
		public PumpkinLight(Vec2 position)
			: base(Resources.PumpkinLightFlicker, position)
		{
			ZIndexMode = EntityManager.ZIndexMode.Manual;
			ZIndex = int.MinValue;
		}
    }
}

