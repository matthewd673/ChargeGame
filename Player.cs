using Verdant;
using Verdant.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChargeGame
{
	public class Player : BoxEntity
	{

        private float moveSpeed = 0.005f;

        private const float minDashDist = 1f;
        private const float maxDashDist = 200f;
        private const float dashChargeTime = 2_000f;

        private bool dashChargeTimerCompleted = false;
        private Timer dashChargeTimer;
        private float CurrentDashDist
        {
            get
            {
                return dashChargeTimerCompleted ?
                       maxDashDist :
                       dashChargeTimer.ElapsedTime / dashChargeTimer.Duration * (maxDashDist - minDashDist) + minDashDist;
            }
        }

        private float moveAngle = 0f;

        private bool dashing = false;
        private Vec2 dashTarget = new();

		public Player(Vec2 position)
			: base(Resources.Player, position, 16, 16, 1f)
		{
			AngleFriction = 1f; // prevent rotation
            Mass = 100f;
            Friction = 0.7f;

            dashChargeTimer = new(dashChargeTime, (t) => { dashChargeTimerCompleted = true; });
		}

        public override void Update()
        {
            base.Update();

            HandleInput();
        }

        private void HandleInput()
        {
            // maintain angle of movement
            Vec2Int mousePos = InputHandler.MousePosition;
            Vec2 mouseWorldPos = Manager.Scene.Camera.ScreenToWorldPosition(mousePos);
            moveAngle = GameMath.AngleBetweenPoints(Position, mouseWorldPos);

            // movement
            if (InputHandler.KeyboardState.IsKeyDown(Keys.W))
            {
                Acceleration = GameMath.AngleToVec2(moveAngle) * moveSpeed;
            }
            else
            {
                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            // only start timer once
            // this is to prevent the timer from restarting
            // if the space key is held past the charge duration
            if (InputHandler.IsKeyFirstPressed(Keys.Space))
            {
                dashChargeTimerCompleted = false;
                dashChargeTimer.Start();
            }

            if (InputHandler.IsKeyFirstReleased(Keys.Space))
            {
                dashing = true;
                dashTarget = Position + GameMath.AngleToVec2(moveAngle) * CurrentDashDist;

                // reset timer
                dashChargeTimerCompleted = false;
                dashChargeTimer.Reset();
            }

            if (dashing)
            {
                Vec2 stepVec = Position - dashTarget;
                Acceleration = stepVec;

                if (stepVec.Magnitude() < 0.01f)
                {
                    dashing = false;
                }
                // 1.85 is the magic number, no idea why
                Velocity = (Position - dashTarget) / (1 - 1.85f*Friction);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Renderer.DrawLine(spriteBatch,
                (Vec2)Manager.Scene.Camera.GetRenderPosition(this),
                (Vec2)Manager.Scene.Camera.GetRenderPosition(this) + GameMath.AngleToVec2(moveAngle) * CurrentDashDist * Renderer.WorldScale,
                Color.White);
            base.Draw(spriteBatch);
			//DrawBody(spriteBatch);
			//DrawPosition(spriteBatch);
        }
    }
}

