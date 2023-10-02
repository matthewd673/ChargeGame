using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Verdant;
using Verdant.Physics;

namespace ChargeGame
{
	public class Player : BoxEntity
	{

        private float baseSpeed = 0.005f;
        private float blinkSpeed = 100f;

		public Player(Vec2 position)
			: base(Resources.Player, position, 16, 16, 1f)
		{
			AngleFriction = 1f; // prevent rotation
            Mass = 100f;
            Friction = 0.6f;
		}

        public override void Update()
        {
            base.Update();

            HandleInput();
        }

        private void HandleInput()
        {
            // movement
            Vec2Int mousePos = InputHandler.MousePosition;
            Vec2 mouseWorldPos = Manager.Scene.Camera.ScreenToWorldPosition(mousePos);

            float angleToMouse = GameMath.AngleBetweenPoints(Position, mouseWorldPos);

            if (InputHandler.KeyboardState.IsKeyDown(Keys.W))
            {
                Acceleration = GameMath.Vec2FromAngle(angleToMouse) * baseSpeed;
            }

            if (InputHandler.IsKeyFirstPressed(Keys.Space))
            {
                Velocity *= blinkSpeed;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
			DrawBody(spriteBatch);
			DrawPosition(spriteBatch);
        }
    }
}

