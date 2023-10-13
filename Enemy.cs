using Microsoft.Xna.Framework.Graphics;
using Verdant;
using Verdant.Physics;

namespace ChargeGame
{
	public class Enemy : BoxEntity
	{

		private int _hitPoints = 1;
		public int HitPoints
		{
			get { return _hitPoints; }
			set
			{
				_hitPoints = value;
				if (_hitPoints <= 0)
				{
					Die();
				}
			}
		}

		private Player player;

		public Enemy(Vec2 position, Player player)
			: base(Resources.Enemy, position, 7, 21, 100f)
		{
			this.player = player;

            AngleFriction = 1f; // prevent rotation
            Friction = 0.7f;
			ZIndexMode = EntityManager.ZIndexMode.Bottom;

			Speed = GameMath.RandomFloat(0.6f, 0.9f);
        }

        public override void Update()
        {
            base.Update();

			Acceleration = GameMath.AngleToVec2(GameMath.AngleBetweenPoints(Position, player.Position));
        }

        public void Hit()
		{
			HitPoints -= 1;
		}

		private void Die()
		{
			// TODO: fancy death
			ForRemoval = true;
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
			base.DrawBody(spriteBatch);
        }
    }
}

