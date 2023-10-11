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

		public Enemy(Vec2 position)
			: base(Resources.Enemy, position, 7, 21, 100f)
		{
            AngleFriction = 1f; // prevent rotation
            Friction = 0.7f;
			ZIndexMode = EntityManager.ZIndexMode.Bottom;
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

