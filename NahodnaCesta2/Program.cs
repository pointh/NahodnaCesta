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
        int StartX, StartY;
        int originPassCount;

        int PosX, PosY, EndX, EndY;
        readonly int[,] field;
        readonly Random rnd;
        readonly List<Coord> path;
 
        public Robot(int[,] field, int x, int y, int endX, int endY)
        {
            StartX = x;
            StartY = y;
            originPassCount = 0;
            this.field = field;

            if (IsIllegalPosition(x, y))
            {
                throw new ArgumentException($"Chybná startovací souřadnice: {x}, {y}");
            }

            if (IsIllegalPosition(endX, endY))
            {
                throw new ArgumentException($"Chybná konečná souřadnice: {endX}, {endY}");
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

        // tady by se dala upravit preference pro určitý směr
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
                // všechny směry se stejnou pravděpodobností
                int[] distances = possibleDirections[rnd.Next(0, 4)]; 
                x += distances[0];
                y += distances[1];

                if (x == StartX && y == StartY)
                {
                    originPassCount++;
                    if (originPassCount > 199)
                        throw new ApplicationException(
                            $"Úloha nemá řešení, robot prošel {originPassCount}x počátkem.");
                }
                
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
                Console.WriteLine($"Hotovo.");
                return;
            }

            RandomStep(ref PosX, ref PosY);
            Go(PosX, PosY);
        }

        private void Squash(List<Coord> lst)
        {
            int squashIdx;

            int i = lst.Count - 2;
            // zruš v seznamu cyklické kroky
            while (i > 0)
            {
                squashIdx = lst.FindIndex(r => r.x == lst[i].x && r.y == lst[i].y);
                if (squashIdx > 0 && squashIdx < i)
                {
                    lst.RemoveRange(squashIdx, i - squashIdx);
                    i = squashIdx;
                }
                i--;
            }
        }

        public void Walk()
        {
            Go(PosX, PosY);
            
            Console.WriteLine($"Původní velikost: {path.Count}");
            Console.WriteLine($"Robot prošel počátkem {originPassCount}x.");

            Squash(path);

            for (int i = 0; i < path.Count; i++)
            {
                Console.WriteLine($"{i}. {path[i]}");
            }

            Console.WriteLine($"Po redukci: {path.Count}");
        }
    }

    class Program
    {
        public static int[,] field = {
          //  0   1   2   3   4  5  6   7  8   9   0   1
            { 0,  0,  0,  0, -1, 0, 0,  0, 0, -1, -1,  0 }, // 0
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0, -1,  0 }, // 1
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0, -1,  0 }, // 2 
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0,  0,  0 }, // 3
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0, -1,  0 }, // 4
            { 0, -1,  0,  0, -1, 0, 0, -1, 0,  0, -1,  0 }, // 5
            { 0, -1,  0,  0, -1, 0, 0, -1, 0, -1, -1, -1 }, // 6
            { 0, -1, -1, -1, -1, 0, 0, -1, 0,  0,  0, -1 }, // 7
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 8
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 9
            {-1, -1,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 0
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 }, // 1
        };
        public static void Main()
        {
            Robot r = new Robot(field, 0, 2, 2, 11);

            r.Walk();
        }
    }
}
