using Microsoft.Xna.Framework;
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

		private Animation enemyWalkRight;
		private Animation enemyWalkLeft;

		public Enemy(Vec2 position, Player player)
			: base(Resources.EnemyWalk, position, 10, 19, 100f)
		{
			this.player = player;

            AngleFriction = 1f; // prevent rotation
            Friction = 0.7f;
			ZIndexMode = EntityManager.ZIndexMode.Bottom;

			enemyWalkRight = Resources.EnemyWalk.Copy();
			enemyWalkLeft = Resources.EnemyWalkLeft.Copy();

			Speed = GameMath.RandomFloat(0.3f, 0.6f);
        }

        public override void Update()
        {
            base.Update();

			Acceleration = GameMath.AngleToVec2(GameMath.AngleBetweenPoints(Position, player.Position));

			if (Acceleration.X >= 0)
			{
				Sprite = enemyWalkRight;
			}
			else
			{
				Sprite = enemyWalkLeft;
			}
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
        }
    }
}

