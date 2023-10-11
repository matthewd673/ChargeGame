using Verdant;

namespace ChargeGame
{
	public class PlayScene : Scene
	{

        private long _score = 0;
        public long Score
        {
            get { return _score; }
            set
            {
                _score = value;
                scoreDisplay.Value = _score;
            }
        }
        private UINumericDisplay scoreDisplay;

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

            GenerateUI();
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

        private void GenerateUI()
        {
            scoreDisplay = new(new(8, 8), Resources.NumbersBig);
            scoreDisplay.FontSheetOffset = 10;

            UIManager.AddElement(scoreDisplay);
        }
    }
}

