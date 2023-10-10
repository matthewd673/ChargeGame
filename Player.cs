﻿using System.Collections.Generic;
using Verdant;
using Verdant.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChargeGame
{
	public class Player : BoxEntity
	{

        struct SlashPath
        {
            public Vec2 Start { get; private set; }
            public Vec2 End { get; private set; }

            public SlashPath(Vec2 start, Vec2 end)
            {
                Start = start;
                End = end;
            }
        }

        private float moveSpeed = 0.005f;

        private const float minDashDist = 1f;
        private const float maxDashDist = 170f;
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
        private Vec2 dashStartPos = new();
        private Vec2 dashTargetPos = new();

        private List<SlashPath> slashPaths = new();

        private int _hitPoints = 3;
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

        public Player(Vec2 position)
			: base(Resources.Player, position, 16, 16, 100f)
		{
			AngleFriction = 1f; // prevent rotation
            Friction = 0.7f;

            dashChargeTimer = new(dashChargeTime, (t) => { dashChargeTimerCompleted = true; });
		}

        public override void Update()
        {
            base.Update();

            HandleInput();
            HandleCollisions();
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
                dashStartPos = Position.Copy();
                dashTargetPos = Position + GameMath.AngleToVec2(moveAngle) * CurrentDashDist;

                // reset timer
                dashChargeTimerCompleted = false;
                dashChargeTimer.Reset();
            }

            bool wasDashing = dashing;
            if (dashing)
            {
                Vec2 stepVec = Position - dashTargetPos;
                Acceleration = stepVec;

                if (GameMath.DistanceBetweenPoints(Position, dashTargetPos) < 0.3f)
                {
                    dashing = false;
                }
                // 1.85 is the magic number, no idea why
                Velocity = (Position - dashTargetPos) / (1 - 1.85f*Friction);
            }

            // check if dash just ended
            if (wasDashing && !dashing)
            {
                Slash(dashStartPos, dashTargetPos);
            }
        }

        private void Slash(Vec2 startPos, Vec2 endPos)
        {
            // spawn path entity
            SlashPath path = new(startPos.Copy(), endPos.Copy());
            slashPaths.Add(path);

            // try to slash all enemies within camera bounds
            foreach (Enemy e in Manager.GetEntitiesInBounds<Enemy>(
                Manager.Scene.Camera.Position,
                (int) Manager.Scene.Camera.Width,
                (int) Manager.Scene.Camera.Height))
            {
                if (GameMath.LineOnRectIntersection(startPos, endPos,
                                                e.Position.X - e.Width / 2,
                                                e.Position.Y - e.Height / 2,
                                                e.Position.X + e.Width / 2,
                                                e.Position.Y + e.Height / 2))
                {
                    e.Hit();
                }
            }
        }

        private void HandleCollisions()
        {
            // take damage from enemies while not dashing
            if (!dashing)
            {
                // TODO
            }
        }

        private void Hit()
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
            // draw aim line
            Renderer.DrawLine(spriteBatch,
                (Vec2)Manager.Scene.Camera.GetRenderPosition(this),
                (Vec2)Manager.Scene.Camera.GetRenderPosition(this) + GameMath.AngleToVec2(moveAngle) * CurrentDashDist * Renderer.WorldScale,
                Color.White);

            // draw slash path
            foreach (SlashPath s in slashPaths)
            {
                int length = (int) GameMath.DistanceBetweenPoints(s.Start, s.End);
                float angle = -GameMath.AngleBetweenPoints(s.Start, s.End) + MathHelper.PiOver2;
                spriteBatch.Draw(Resources.Wall.Texture,
                                 new Microsoft.Xna.Framework.Rectangle(
                                     (int)s.Start.X * Renderer.WorldScale,
                                     (int)s.Start.Y * Renderer.WorldScale,
                                     length * Renderer.WorldScale,
                                     4 * Renderer.WorldScale),
                                 null,
                                 Color.White,
                                 angle,
                                 Vector2.Zero,
                                 SpriteEffects.None,
                                 0
                                 );
            }

            // draw player
            base.Draw(spriteBatch);
        }
    }
}

