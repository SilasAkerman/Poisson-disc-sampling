using System;
using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;


namespace Poisson_disc_sampling
{
    class Program
    {
        const int WIDTH = 1000;
        const int HEIGHT = 800;

        static double w = R / Math.Sqrt(2);
        static int cols = (int)Math.Floor(WIDTH / w);
        static int rows = (int)Math.Floor(HEIGHT / w);

        static Vector2[] grid = new Vector2[cols * rows];
        static List<Vector2> active = new List<Vector2>();
        static List<Vector2> ordered = new List<Vector2>();

        const float CIRCLE_SIZE = 1.5f;

        const int R = 5;
        const int K = 30;

        static Random rand = new Random();

        static void Main(string[] args)
        {
            Raylib.InitWindow(WIDTH, HEIGHT, "Poisson-disc-sampling");
            Raylib.SetTargetFPS(60);

            GenerateStartingPoint();

            while (!Raylib.WindowShouldClose())
            {
                //while (active.Count > 0) GeneratePoints();
                for (int total = 0; total < 100; total++)
                GeneratePoints();

                Display();
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_R)) GenerateStartingPoint();
            }
        }

        static void GenerateStartingPoint()
        {
            active.Clear();
            ordered.Clear();

            // STEP 0
            for (int i = 0; i < cols * rows; i++)
            {
                grid[i] = new Vector2(-1, -1);
            }

            // STEP 1
            //Vector2 pos = new Vector2(rand.Next(WIDTH), rand.Next(HEIGHT));
            Vector2 pos = new Vector2(WIDTH / 2, HEIGHT / 2);
            int c = (int)Math.Floor(pos.X / w);
            int r = (int)Math.Floor(pos.Y / w);
            grid[c + r * cols] = pos;
            active.Add(pos);
        }

        static void GeneratePoints()
        {
            if (active.Count <= 0) return;
            int randIndex = (int)Math.Floor(rand.NextDouble() * active.Count);
            Vector2 pos = active[randIndex];

            bool found = false;
            for (int n = 0; n < K; n++)
            {
                double angle = rand.NextDouble() * Math.PI * 2;
                Vector2 sample = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                float magnitude = (float)rand.NextDouble() * R + R;
                sample *= magnitude;
                sample += pos;

                int col = (int)(sample.X / w);
                int row = (int)(sample.Y / w);

                if (col >= cols || row >= rows || col < 0 || row < 0) break;

                bool ok = true;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int index = (col + i) + (row + j) * cols;
                        if (index < 0 || index >= cols * rows) continue;
                        Vector2 neighbour = grid[index];
                        if (neighbour.X != -1)
                        {
                            float d = Vector2.Distance(sample, neighbour);
                            if (d < R)
                            {
                                ok = false;
                            }
                        }
                    }
                }
                if (ok)
                {
                    found = true;
                    grid[col + row * cols] = sample;
                    active.Add(sample);
                    ordered.Add(sample);
                    break;
                }
            }
            if (!found)
            {
                active.RemoveAt(randIndex);
            }
        }

        static void Display()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            for (int i = 0; i < ordered.Count; i++)
            {
                Vector2 point = ordered[i];
                Color color = Raylib.ColorFromHSV(i / 60 % 360, 1, 1);
                if (point.X < 0) continue;
                Raylib.DrawCircleV(point, CIRCLE_SIZE, color);
            }

            //foreach (Vector2 point in active)
            //{
            //    Raylib.DrawCircleV(point, CIRCLE_SIZE, Color.PINK);
            //}

            Raylib.EndDrawing();
        }
    }
}
