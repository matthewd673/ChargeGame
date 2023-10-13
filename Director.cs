using System.Collections.Generic;
using Verdant;
namespace ChargeGame
{
	public class Director
	{
		private EntityManager entityManager;
		private List<Vec2> spawnCandidates;
		private Player player;

		private float spawnCooldown = 3_500f;
		private int spawnAmount = 2;
		private Timer spawnTimer;

		public Director(EntityManager entityManager, List<Vec2> spawnCandidates, Player player)
		{
			this.spawnCandidates = spawnCandidates;
			this.entityManager = entityManager;
			this.player = player;

			spawnTimer = new(spawnCooldown, SpawnTimerCallback);
			spawnTimer.Start();
		}

		private void SpawnTimerCallback(Timer t)
        {
            // spawn enemies
            for (int i = 0; i < spawnAmount + GameMath.Random.Next(spawnAmount); i++)
            {
                int spawnIndex = GameMath.Random.Next(spawnCandidates.Count);
                Vec2 s = spawnCandidates[spawnIndex];

                // retry if enemy is too close to player
                if (GameMath.DistanceBetweenPoints(s, player.Position) < 20)
                {
                    i--;
                    continue;
                }

                Enemy e = new(spawnCandidates[spawnIndex].Copy(), player);
                entityManager.AddEntity(e);
            }

            // restart
            t.Restart();
        }
	}
}

