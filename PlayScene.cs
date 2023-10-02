using Verdant;

namespace ChargeGame
{
	public class PlayScene : Scene
	{
		public PlayScene() : base("play")
		{
		}

        public override void Initialize()
        {
            base.Initialize();

            Player player = new Player(new Vec2());
            EntityManager.AddEntity(player);
        }
    }
}

