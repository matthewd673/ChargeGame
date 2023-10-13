using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Verdant;
using Verdant.Physics;

namespace ChargeGame
{
	public class DemonPumpkin : BoxEntity
	{

		private const int chargeThreshhold = 25;

		private int _sacrifices;
		public int Sacrifices
		{
			get { return _sacrifices; }
			set
			{
				_sacrifices = value;
				if (_sacrifices % chargeThreshhold == 0 && _sacrifices > 0)
				{
					Activate();
				}
			}
		}

		public Player Player { get; set; }

		public DemonPumpkin(Vec2 position, Player player)
			: base(Resources.DemonPumpkin, position, 33, 21, 0f)
		{
			Player = player;

			AngleFriction = 1f;
			ZIndexMode = EntityManager.ZIndexMode.Bottom;
		}

        public override void OnAdd()
        {
            base.OnAdd();
			Manager.AddEntity(new PumpkinLight(Position + new Vec2(0, 8)));
        }

		private void Activate()
		{
			int activationCountdown = 5;
			Timer activationTimer = new(700f, (t) =>
			{
				Manager.Scene.Camera.SetShake(2.5f, 700f);

				Gem gem = new Gem(Position.Copy());
                float velAngle = GameMath.RandomFloat(0, MathHelper.TwoPi);
                gem.Velocity = GameMath.AngleToVec2(velAngle) * 12f;
                Manager.AddEntity(gem);

                activationCountdown--;
				if (activationCountdown > 0)
				{
					t.Restart();
				}
			});
			activationTimer.Start();

			// restore health only if low
			if (Player.HitPoints < 2) // max hitpoints - 1
			{
				Player.HitPoints += 1;
			}
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

			// draw charge bar
			Vec2Int barPos = Manager.Scene.Camera.GetRenderPosition(this);
			barPos.Y -= (int)(Height * 2f);
			barPos.X -= chargeThreshhold + 1;
			for (int i = 0; i <= Sacrifices % chargeThreshhold; i++)
			{
				int index = 3;
				if (i == 0 || i == 1)
				{
					index = i;
				}
				else if (i == Sacrifices % chargeThreshhold - 1 ||
						 i == Sacrifices % chargeThreshhold)
				{
					index = System.Math.Abs(i - chargeThreshhold);
				}
				Resources.ChargeBar.DrawIndex(spriteBatch,
					new Microsoft.Xna.Framework.Rectangle(
						barPos.X + i * Renderer.WorldScale,
						barPos.Y,
						Renderer.WorldScale,
						Resources.ChargeBar.Height * Renderer.WorldScale),
					index
					);
			}
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

