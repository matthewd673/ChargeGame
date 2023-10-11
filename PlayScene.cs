using Verdant;

namespace ChargeGame
{
	public class PlayScene : Scene
	{
		public PlayScene() : base("play")
		{
            // Empty
		}

        public override void Initialize()
        {
            base.Initialize();

            Player player = new Player(new Vec2());
            EntityManager.AddEntity(player);

            GenerateWorld();
        }

        private void GenerateWorld()
        {
            // generate enemies (TODO: temp)
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (GameMath.Random.Next(20) == 0)
                    {
                        Enemy e = new Enemy(new(i * 16, j * 16));
                        EntityManager.AddEntity(e);
                    }
                }
            }

            DemonPumpkin demonPumpkin = new(new(100, 100));
            EntityManager.AddEntity(demonPumpkin);
        }
    }
}

