using Microsoft.Xna.Framework;
using Verdant;
using Verdant.UI;
namespace ChargeGame
{
	public class TitleScene : Scene
	{

        public static long HighScore = 0;

		public TitleScene() : base("title")
		{
			// Empty
		}

        public override void Initialize()
        {
            base.Initialize();

            Manager.RemoveScene("play"); // reset each time

            // build ui
            UIStack stack = new(new(Renderer.ScreenWidth / 4, Renderer.ScreenHeight / 4))
            {
                Gap = 8,
            };
            UISprite logo = new(Resources.Logo, new());
            UISprite high_score = new(Resources.HighScore, new());
            UINumericDisplay score_display = new(new(), Resources.NumbersBig)
            {
                Value = HighScore,
                FontSheetOffset = 10,
            };
            UISprite press_space = new(Resources.PressSpace, new());

            stack.AddElement(logo);
            stack.AddElement(new(new(), 1, 20));
            stack.AddElement(high_score);
            stack.AddElement(score_display);
            stack.AddElement(new(new(), 1, 20));
            stack.AddElement(press_space);

            stack.Position.Y -= 48;
            UIManager.AddElement(stack);
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

