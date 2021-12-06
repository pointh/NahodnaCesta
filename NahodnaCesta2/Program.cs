using System;
using System.Collections.Generic;

namespace NahodnaCesta2
{
    class Robot
    {
        public int PosX;
        public int PosY;
        public int EndX;
        public int EndY;

        int[,] field;

        readonly Random rnd;

        readonly List<int> path;

        public Robot(int[,] field, int x, int y, int endX, int endY) 
        { 
            PosX = x; PosY = y;
            EndX = endX; EndY = endY;
            this.field = field;
            rnd = new Random();
            path = new List<int>();
        }

        bool InInterval(int x, int min, int max) => x >= min && x <= max;

        void RandomStep(ref int x, ref int y)
        {
            int[] stepVariants = { -1, 1, 0 };
            int distanceX, distanceY;

            distanceX = stepVariants[rnd.Next(0, 3)];  // -1, 0, 1
            distanceY = 0;

            if (distanceX == 0)
            {
                distanceY = stepVariants[rnd.Next(0, 2)]; // -1, 1
                y += distanceY;
            }
            else
            {
                x += distanceX;
            }
        }

        public void Go(int FromX, int FromY)
        {
            if (FromX == EndX && FromY == EndY)
            {
                Console.WriteLine($"Jsem u cíle za {path.Count/2} kroků.");
                return;
            }

            do
            {
                RandomStep(ref PosX, ref PosY);

                if (InInterval(PosX, 0, field.GetLength(0) - 1) == false || // mimo pole, vrať se
                    InInterval(PosY, 0, field.GetLength(1) - 1) == false)
                {
                    Console.WriteLine($"Aut {PosX}, {PosY}");
                    PosX = FromX;
                    PosY = FromY;
                    continue;
                }

                if (field[PosX, PosY] == -1)                                // v bariéře, vrať se
                {
                    Console.WriteLine($"Crash {PosX}, {PosY}");
                    PosX = FromX;
                    PosY = FromY;
                    continue;
                }
            } while (field[PosX, PosY] == -1);

            Console.WriteLine($"{PosX}, {PosY}");
            path.Add(PosX); path.Add(PosY);

            Go(PosX, PosY);
        }
    }

    class Field
    {
        public int[,] field = {
          //  0   1   2   3   4  5  6   7  8   9   0   1
            { 0,  0,  0,  0,  0, 0, 0,  0, 0,  0,  0,  0 }, // 0
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0,  0,  0 }, // 1
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0,  0,  0 }, // 2 
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0,  0,  0 }, // 3
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0,  0,  0 }, // 4
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0, -1,  0 }, // 5
            { 0,  0,  0,  0, -1, 0, 0, -1, 0, -1, -1, -1 }, // 6
            { 0, -1, -1, -1, -1, 0, 0, -1, 0,  0,  0, -1 }, // 7
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 8
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 9
            {-1, -1,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 0
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 1
        };

        public void RobotWalk(Robot r)
        {
            r.Go(r.PosX, r.PosY);
        }    
    }
   
    class Program
    {
        public static void Main()
        {
            Field f = new Field();
            Robot r = new Robot(f.field, 2, 11, 6, 2);

            f.RobotWalk(r);
        }
    }
}
