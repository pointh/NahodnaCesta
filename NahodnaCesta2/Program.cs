using System;
using System.Collections.Generic;


namespace NahodnaCesta2
{
    struct Coord
    {
        public int x, y;
        public override string ToString()
        {
            return $"{x}, {y}";
        }
    }

    class Robot
    {
        int PosX, PosY, EndX, EndY;
        readonly int[,] field;
        readonly Random rnd;
        readonly List<Coord> path;
 
        public Robot(int[,] field, int x, int y, int endX, int endY)
        {
            this.field = field;

            if (IsIllegalPosition(x, y))
            {
                throw new ArgumentException($"Startovací souřadnice: {x}, {y}");
            }

            if (IsIllegalPosition(endX, endY))
            {
                throw new ArgumentException($"Konečná souřadnice: {endX}, {endY}");
            }

            PosX = x; PosY = y;
            EndX = endX; EndY = endY;
            rnd = new Random();
            path = new List<Coord>();
        }

        bool InInterval(int x, int min, int max) => x >= min && x <= max;

        bool PositionOutOfField(int x, int y)
        {
            return InInterval(x, 0, field.GetLength(0) - 1) == false ||
                   InInterval(y, 0, field.GetLength(1) - 1) == false;
        }

        bool PositionInBarrier(int x, int y)
        {
            if (PositionOutOfField(x, y))
                return true;

            return field[x, y] == -1;
        }

        public bool IsIllegalPosition(int x, int y)
        {
            if (PositionOutOfField(x, y))
                return true;
            if (PositionInBarrier(x, y))
                return true;

            return false;
        }

        static List<int[]> possibleDirections = new List<int[]>()
        {
                new int[] { -1, 0 },
                new int[] { 1, 0 },
                new int[] { 0, -1 },
                new int[] { 0, 1 }
        };

        void RandomStep(ref int x, ref int y)
        {
            do
            {               
                int[] distances = possibleDirections[rnd.Next(0, 4)];
                
                x += distances[0];
                y += distances[1];
                
                if (IsIllegalPosition(x, y))
                {
                    x -= distances[0];
                    y -= distances[1];
                }
                else
                {
                    break;
                }
            } while (true);
        }

        public void Go(int FromX, int FromY)
        {
            path.Add(new Coord { x = PosX, y = PosY });

            if (FromX == EndX && FromY == EndY)
            {
                Console.WriteLine($"Jsem u cíle za {path.Count} kroků.");
                return;
            }

            RandomStep(ref PosX, ref PosY);

            Go(PosX, PosY);
        }

        public void Walk()
        {
            Go(PosX, PosY);
            Console.WriteLine(string.Join("\n", path));
            Console.WriteLine(path.Count);
        }
    }

    class Program
    {
        public static int[,] field = {
          //  0   1   2   3   4  5  6   7  8   9   0   1
            { 0,  0,  0,  0,  0, 0, 0,  0, 0,  0,  0,  0 }, // 0
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0,  0,  0 }, // 1
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0,  0,  0 }, // 2 
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0,  0,  0 }, // 3
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0,  0,  0 }, // 4
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0, -1,  0 }, // 5
            { 0,  0,  0,  0, -1, 0, 0, -1, 0, -1, -1, -1 }, // 6
            { 0, -1, -1, -1, -1, 0, 0, -1, 0,  0,  0, -1 }, // 7
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 8
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 9
            {-1, -1,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 0
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 1
        };
        public static void Main()
        {
            Robot r = new Robot(field, 2, 11, 6, 2);

            r.Walk();
        }
    }
}
