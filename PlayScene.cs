using System.Collections.Generic;
using Verdant;
using Verdant.Physics;

namespace ChargeGame
{
	public class PlayScene : Scene
	{

        private const int worldCellWidth = 65;
        private const int worldCellHeight = 37;

        private const float worldNoiseProb = 0.6f;
        private const int worldGenerations = 5;

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

        public List<Boundary> Boundaries { get; private set; }
        public List<Vec2> SpawnCandidates { get; private set; }

        public PlayScene() : base("play")
		{
            // Empty
		}

        public override void Initialize()
        {
            base.Initialize();
            EntityManager = new(32); // magic number, constrained because demon pumpkin is 33 wide so must be ~ >16 or something
            EntityManager.Scene = this;

            GenerateWorld();
            GenerateUI();
        }

        private void GenerateWorld()
        {
            // seed initial map
            int[,] cellMap = new int[worldCellWidth, worldCellHeight];
            for (int i = 0; i < worldCellWidth; i++)
            {
                for (int j = 0; j < worldCellHeight; j++)
                {
                    cellMap[i, j] = GameMath.RandomFloat() <= worldNoiseProb ? 1 : 0;
                }
            }

            // run cave cellular automata
            for (int n = 0; n < worldGenerations; n++)
            {
                for (int i = 0; i < worldCellWidth; i++)
                {
                    for (int j = 0; j < worldCellHeight; j++)
                    {
                        int walls = 0;
                        // count all surrounding walls
                        for (int k = -1; k <= 1; k++)
                        {
                            for (int l = -1; l <= 1; l++)
                            {
                                int x = i + k;
                                int y = j + l;

                                // out of bounds
                                if (x < 0 || x >= worldCellWidth ||
                                    y < 0 || y >= worldCellHeight)
                                {
                                    continue;
                                }

                                walls += cellMap[x, y];
                            }
                        }

                        if (cellMap[i, j] == 0)
                        {
                            cellMap[i, j] = walls >= 5 ? 1 : 0;
                        }
                        else if (cellMap[i, j] == 1)
                        {
                            cellMap[i, j] = walls >= 4 ? 1 : 0;
                        }
                    }
                }
            }

            // enforce 1 cell border around map
            for (int i = 0; i < worldCellWidth; i++)
            {
                cellMap[i, 0] = 0;
                cellMap[i, worldCellHeight - 1] = 0;
            }
            for (int j = 0; j < worldCellHeight; j++)
            {
                cellMap[0, j] = 0;
                cellMap[worldCellWidth - 1, j] = 0;
            }

            // generate spawn point candidates
            SpawnCandidates = new();
            for (int i = 0; i < worldCellWidth; i++)
            {
                for (int j = 0; j < worldCellHeight; j++)
                {
                    int walls = 0;
                    // count all surrounding walls
                    for (int k = -1; k <= 1; k++)
                    {
                        for (int l = -1; l <= 1; l++)
                        {
                            int x = i + k;
                            int y = j + l;

                            // out of bounds
                            if (x < 0 || x >= worldCellWidth ||
                                y < 0 || y >= worldCellHeight)
                            {
                                continue;
                            }

                            walls += cellMap[x, y] != 1 ? 1 : 0; // != 1 so that spawn candidates spawn every other
                        }
                    }

                    if (walls == 0)
                    {
                        cellMap[i, j] = 2;
                        SpawnCandidates.Add(new Vec2(i * Wall.WallWidth, j * Wall.WallHeight));
                    }
                }
            }

            // generate wall entities
            for (int i = 0; i < worldCellWidth; i++)
            {
                for (int j = 0; j < worldCellHeight; j++)
                {
                    if (cellMap[i, j] != 0)
                    {
                        continue;
                    }

                    int walls = 0;
                    for (int k = -1; k <= 1; k++)
                    {
                        for (int l = -1; l <= 1; l++)
                        {
                            if (i + k < 0 || i + k >= worldCellWidth ||
                                j + l < 0 || j + l >= worldCellHeight)
                            {
                                continue;
                            }

                            if (cellMap[i + k, j + l] == 0)
                            {
                                walls += 1;
                            }
                        }
                    }

                    // don't draw fully surrounded walls
                    if (walls == 9)
                    {
                        continue;
                    }

                    Wall.WallStyle wallStyle = 0x0;
                    if (j > 0 && cellMap[i, j - 1] == 1)
                    {
                        wallStyle |= Wall.WallStyle.Top;
                    }
                    if (j < worldCellHeight - 1 && cellMap[i, j + 1] == 1)
                    {
                        wallStyle |= Wall.WallStyle.Bottom;
                        // make bottom walls a little fancier
                        if (i > 0 && cellMap[i - 1, j + 1] == 0)
                        {
                            wallStyle |= Wall.WallStyle.Left;
                        }
                        if (i < worldCellWidth - 1 && cellMap[i + 1, j + 1] == 0)
                        {
                            wallStyle |= Wall.WallStyle.Right;
                        }
                    }
                    if (i > 0 && cellMap[i - 1, j] == 1)
                    {
                        wallStyle |= Wall.WallStyle.Left;
                    }
                    if (i < worldCellWidth - 1 && cellMap[i + 1, j] == 1)
                    {
                        wallStyle |= Wall.WallStyle.Right;
                    }

                    Wall w = new(new Vec2(i * Wall.WallWidth, j * Wall.WallHeight), wallStyle);
                    EntityManager.AddEntity(w);
                }
            }

            // generate physics walls
            Boundaries = new();
            for (int i = 0; i < worldCellWidth; i++)
            {
                for (int j = 0; j < worldCellHeight; j++)
                {
                    // skip floors and walls that have been scanned by the algo
                    if (cellMap[i, j] != 0)
                    {
                        continue;
                    }

                    // check four sides to see if they have a neighbor
                    // if there is no neighbor, extend in that direction
                    // LEFT
                    if (i > 0 && cellMap[i - 1, j] == 1)
                    {
                        cellMap[i, j] -= 1;
                        (Vec2, Vec2) w = new(
                            new(i * Wall.WallWidth - (Wall.WallWidth / 2), j * Wall.WallHeight - (Wall.WallHeight / 2)),
                            new(i * Wall.WallWidth - (Wall.WallWidth / 2), (j + 1) * Wall.WallHeight - (Wall.WallHeight / 2))
                            );

                        Boundary b = new(w.Item1, w.Item2);
                        Boundaries.Add(b);
                        EntityManager.AddEntity(b);
                    }
                    // RIGHT
                    if (i < worldCellWidth - 1 && cellMap[i + 1, j] == 1)
                    {
                        cellMap[i, j] -= 1;
                        (Vec2, Vec2) w = new(
                            new((i + 1) * Wall.WallWidth - (Wall.WallWidth / 2), j * Wall.WallHeight - (Wall.WallHeight / 2)),
                            new((i + 1) * Wall.WallWidth - (Wall.WallWidth / 2), (j + 1) * Wall.WallHeight - (Wall.WallHeight / 2))
                            );

                        Boundary b = new(w.Item1, w.Item2);
                        Boundaries.Add(b);
                        EntityManager.AddEntity(b);
                    }
                    // UP
                    if (j > 0 && cellMap[i, j - 1] == 1)
                    {
                        cellMap[i, j] -= 1;
                        (Vec2, Vec2) w = new(
                            new(i * Wall.WallWidth - (Wall.WallWidth / 2), j * Wall.WallHeight - (Wall.WallHeight / 2)),
                            new((i + 1) * Wall.WallWidth - (Wall.WallWidth / 2), j * Wall.WallHeight - (Wall.WallHeight / 2))
                            );

                        Boundary b = new(w.Item1, w.Item2);
                        Boundaries.Add(b);
                        EntityManager.AddEntity(b);
                    }
                    // DOWN
                    if (j < worldCellHeight - 1 && cellMap[i, j + 1] == 1)
                    {
                        cellMap[i, j] -= 1;
                        (Vec2, Vec2) w = new(
                            new(i * Wall.WallWidth - (Wall.WallWidth / 2), (j + 1) * Wall.WallHeight - (Wall.WallHeight / 2)),
                            new((i + 1) * Wall.WallWidth - (Wall.WallWidth / 2), (j + 1) * Wall.WallHeight - (Wall.WallHeight / 2))
                            );

                        Boundary b = new(w.Item1, w.Item2);
                        Boundaries.Add(b);
                        EntityManager.AddEntity(b);
                    }
                }
            }

            // place demon pumpkin
            // find center-most spawn candidate
            Vec2 centerMost = null;
            Vec2 trueCenter = new(Camera.Width / 4, Camera.Height / 4);
            foreach (Vec2 s in SpawnCandidates)
            {
                if (centerMost == null ||
                    GameMath.DistanceBetweenPoints(s, trueCenter) <
                    GameMath.DistanceBetweenPoints(centerMost, trueCenter)
                    )
                {
                    centerMost = s;
                }
            }
            SpawnCandidates.Remove(centerMost); // no longer a spawn candidate
            DemonPumpkin demonPumpkin = new(centerMost.Copy());
            EntityManager.AddEntity(demonPumpkin);

            // spawn player
            // find a point near the demon-pumpkin (but not too near)
            List<Vec2> possiblePlayerSpawns = new();
            foreach (Vec2 s in SpawnCandidates)
            {
                if (GameMath.DistanceBetweenPoints(s, centerMost) > 20 &&
                    GameMath.DistanceBetweenPoints(s, centerMost) < 50)
                {
                    possiblePlayerSpawns.Add(s);
                }
            }
            Player player = new(possiblePlayerSpawns[GameMath.Random.Next(possiblePlayerSpawns.Count)]);
            EntityManager.AddEntity(player);
        }

        private void GenerateUI()
        {
            scoreDisplay = new(new(8, 8), Resources.NumbersBig);
            scoreDisplay.FontSheetOffset = 10;

            UIManager.AddElement(scoreDisplay);
        }
    }
}

