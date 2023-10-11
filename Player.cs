using System;
using System.Collections.Generic;
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

        private const float minDashDist = 0f;
        private const float maxDashDist = 170f;
        private const float maxChainDashDist = maxDashDist * 1.1f;
        private const float dashChargeTime = 2_000f;

        private bool dashChargeTimerCompleted = false;
        private Timer dashChargeTimer;
        private bool chainingDash = false;
        private float CurrentDashDist
        {
            get
            {
                float timePercent = dashChargeTimer.ElapsedTime / dashChargeTimer.Duration;
                // count up towards maximum when not chaining dash
                if (!chainingDash)
                {
                    return dashChargeTimerCompleted ?
                           maxDashDist :
                           timePercent * (maxDashDist - minDashDist) + minDashDist;
                }
                // count down towards zero when chaining dash
                else
                {
                    return dashChargeTimerCompleted ?
                           0 :
                           (1 - timePercent) * (maxChainDashDist - minDashDist) + minDashDist;
                }
            }
        }

        private float moveAngle = 0f;

        private bool dashing = false;
        private Vec2 dashStartPos = new();
        private Vec2 dashTargetPos = new();

        private List<SlashPath> slashPaths = new();
        private List<Vec2> slashPoints = new();
        private int lastSlashCount = 0;

        private bool slashAnimationTimerCompleted = false;
        private const float slashAnimationDuration = 200f;
        private Timer slashAnimationTimer;

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
			: base(Resources.PlayerIdle, position, 12, 19, 100f)
		{
			AngleFriction = 1f; // prevent rotation
            Friction = 0.7f;
            Trigger = true;

            dashChargeTimer = new(dashChargeTime, (t) =>
            {
                dashChargeTimerCompleted = true;
            });

            slashAnimationTimer = new(slashAnimationDuration, (t) =>
            {
                slashAnimationTimerCompleted = true;
            });
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
                if (chainingDash && CurrentDashDist == 0)
                {
                    chainingDash = false;
                }

                if (!chainingDash)
                {
                    dashChargeTimerCompleted = false;
                    dashChargeTimer.Start();
                }
            }

            if (InputHandler.IsKeyFirstReleased(Keys.Space))
            {
                dashing = true;
                dashStartPos = Position.Copy();
                dashTargetPos = Position + GameMath.AngleToVec2(moveAngle) * CurrentDashDist;

                // reset timer
                dashChargeTimerCompleted = false;
                dashChargeTimer.Reset();

                // begin slash animation
                slashAnimationTimerCompleted = false;
                slashAnimationTimer.Restart();
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
            // reset last hit counter
            lastSlashCount = 0;

            // mark slash path
            SlashPath path = new(startPos.Copy(), endPos.Copy());
            slashPaths.Add(path);

            // try to slash all enemies within camera bounds
            foreach (Enemy e in Manager.GetEntitiesInBounds<Enemy>(
                Manager.Scene.Camera.Position,
                (int) Manager.Scene.Camera.Width,
                (int) Manager.Scene.Camera.Height))
            {
                List<Vec2> points = GameMath.LineOnRectIntersectionPoints(startPos, endPos,
                                                                            e.Position.X - e.Width / 2,
                                                                            e.Position.Y - e.Height / 2,
                                                                            e.Position.X + e.Width / 2,
                                                                            e.Position.Y + e.Height / 2);
                slashPoints.AddRange(points);

                if (points.Count > 0)
                {
                    e.Hit();
                }

                lastSlashCount += points.Count;
            }

            if (lastSlashCount > 0)
            {
                chainingDash = true;
                dashChargeTimer.Restart();
            }
            else
            {
                chainingDash = false;
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

            for (int i = 0; i < slashPaths.Count; i++)
            {
                SlashPath s = slashPaths[i];
                float segProgress = 0f;
                float segInc = 1f / s.VisualPoints.Count;
                foreach ((Vec2 vA, Vec2 vB) in s.VisualPoints)
                {
                    segProgress += segInc;
                    // skip drawing segments that shouldn't appear yet
                    if (!slashAnimationTimerCompleted && // if timer completed, draw entire segment
                        i == slashPaths.Count - 1 && // only animate the last slash in the list
                        segProgress > slashAnimationTimer.ElapsedTime / slashAnimationTimer.Duration)
                    {
                        continue;
                    }

                    int vLength = (int) GameMath.DistanceBetweenPoints(vA, vB);
                    float vAngle = -GameMath.AngleBetweenPoints(vA, vB) + MathHelper.PiOver2;
                    spriteBatch.Draw(Resources.Slash.Texture,
                                 new Microsoft.Xna.Framework.Rectangle(
                                     (int)vA.X * Renderer.WorldScale,
                                     (int)vA.Y * Renderer.WorldScale,
                                     vLength * Renderer.WorldScale,
                                     1),
                                 null,
                                 Color.White,
                                 vAngle,
                                 Vector2.Zero,
                                 SpriteEffects.None,
                                 0
                                 );
                }
            }

            foreach (Vec2 p in slashPoints)
            {
                spriteBatch.Draw(Renderer.Pixel,
                                 new Microsoft.Xna.Framework.Rectangle(
                                     Manager.Scene.Camera.GetRenderPosition(p).X,
                                     Manager.Scene.Camera.GetRenderPosition(p).Y,
                                     4,
                                     4),
                                 Color.Magenta);
            }

            // draw player
            base.Draw(spriteBatch);
        }
    }

    class SlashPath
    {
        public Vec2 Start { get; private set; }
        public Vec2 End { get; private set; }

        public List<(Vec2, Vec2)> VisualPoints { get; private set; }

        public SlashPath(Vec2 start, Vec2 end)
        {
            Start = start;
            End = end;

            CalculateVisualPoints();
        }

        private void CalculateVisualPoints()
        {
            VisualPoints = new();

            const float segmentLength = 4f;
            //float slashLength = GameMath.DistanceBetweenPoints(Start, End);
            float slashAngle = GameMath.AngleBetweenPoints(Start, End);

            // add randomized segments from the start to the end of the slash
            Vec2 lastSegEnd = Start;
            while (GameMath.DistanceBetweenPoints(lastSegEnd, End) > 2f)
            {
                // add some random variation to the segment length
                float segLength = segmentLength * GameMath.RandomFloat(0.8f, 2f);

                // check if we are finishing the path and closing the gap to the player
                bool isLastSegment = false;
                float distToEnd = GameMath.DistanceBetweenPoints(lastSegEnd, End);
                if (segLength > distToEnd)
                {
                    segLength = distToEnd;
                    isLastSegment = true;
                }

                // start where the last segment ended
                Vec2 segStart = lastSegEnd.Copy();

                float segAngle = GameMath.AngleBetweenPoints(segStart, End);
                // if not last segment add some random angle variation
                if (!isLastSegment)
                {
                    segAngle *= GameMath.RandomFloat(0.5f, 1.5f);
                }
                Vec2 segEnd = segStart + GameMath.AngleToVec2(segAngle) * segLength;
                lastSegEnd = segEnd;

                VisualPoints.Add((segStart, segEnd));
            }
        }
    }
}

