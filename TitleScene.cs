using Microsoft.Xna.Framework;
using Verdant;
namespace ChargeGame
{
	public class TitleScene : Scene
	{
		public TitleScene() : base("title")
		{
			// Empty
		}

        public override void Initialize()
        {
            base.Initialize();

            Manager.RemoveScene("play"); // reset each time
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputHandler.IsKeyFirstPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                Timer t = new(100f, (t) =>
                {
                    PlayScene playScene = new();
                    Manager.AddScene(playScene);
                    Manager.ActiveID = "play";
                });

                t.Start();
            }
        }
    }
}

