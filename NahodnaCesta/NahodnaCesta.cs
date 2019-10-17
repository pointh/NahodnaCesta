using System;
using System.Linq;

namespace NahodnaCesta
{
    public enum Smer { Vpravo, Dolu, Vlevo, Nahoru, Nikam };

    class Figura
    {
        const int Hranice = int.MaxValue;
        const int BlokovanePole = -1;
        const int ZavrenePole = -2;
        int pocetKroku = 0;
        public bool debug = true; // zobrazovat/nezobrazovat mezivýsledky

        private int X, Y; // aktuální pozice
        private readonly int cilX, cilY; // pozice cíle
        private readonly int startX, startY; // pozice začátku

        private int _sirka, _vyska; // šířka a výška hřiště
        private int[,] hriste;
        private int znacka = 1; // hodnota navštíveného pole
        Random rnd;

        public Figura(int startRadek, int startSloupec, int endRadek, int endSloupec, int[,] arr)
        {
            startX = startSloupec; startY = startRadek;
            cilX = endSloupec; cilY = endRadek;

            X = startSloupec; Y = startRadek;
            this._sirka = arr.GetLength(1);
            this._vyska = arr.GetLength(0);
            hriste = arr;
            //hriste[cilY, cilX] = -9;
            rnd = new Random();
        }

        int[] OkoliPole(int x, int y)
        {
            int[] hodnotyVeSmeru = new int[4];

            if (x + 1 < _sirka && hriste[y, x + 1] >= 0)
                hodnotyVeSmeru[(int)Smer.Vpravo] = hriste[y, x + 1]; // hodnota pole vpravo
            else
                hodnotyVeSmeru[(int)Smer.Vpravo] = Hranice; // vpravo

            if (y + 1 < _vyska && hriste[y + 1, x] >= 0)
                hodnotyVeSmeru[(int)Smer.Dolu] = hriste[y + 1, x];
            else
                hodnotyVeSmeru[(int)Smer.Dolu] = Hranice; // dolu

            if (x > 0 && hriste[y, x - 1] >= 0)
                hodnotyVeSmeru[(int)Smer.Vlevo] = hriste[y, x - 1];
            else
                hodnotyVeSmeru[(int)Smer.Vlevo] = Hranice; // vlevo

            if (y > 0 && hriste[y - 1, x] >= 0)
                hodnotyVeSmeru[(int)Smer.Nahoru] = hriste[y - 1, x];
            else
                hodnotyVeSmeru[(int)Smer.Nahoru] = Hranice; // nahoru

            return hodnotyVeSmeru;
        }

        private Smer Strategy(int[] okoli)
        {
            if (Array.IndexOf(okoli, 0) == -1)
            {
                return Smer.Nikam;
            }

            return (Smer)Array.IndexOf(okoli, okoli.Min());
            // jdi směrem na MINIMÁLNÍ sousední hodnotu

            // return (Smer)rnd.Next(0, 4);

        }
        private Smer OptimalniSmer()
        {
            int[] okoliAktualniPozice = OkoliPole(X, Y);
            Smer smer = Strategy(okoliAktualniPozice);
            return smer;
        }

        private void Vpravo()
        {
            if (X + 1 < _sirka && hriste[Y, X + 1] >= 0)
                X += 1;
        }

        private void Vlevo()
        {
            if (X - 1 >= 0 && hriste[Y, X - 1] >= 0)
                X -= 1;
        }

        private void Nahoru()
        {
            if (Y - 1 >= 0 && hriste[Y - 1, X] >= 0)
                Y -= 1;
        }

        private void Dolu()
        {
            if (Y + 1 < _vyska && hriste[Y + 1, X] >= 0)
                Y += 1;
        }

        public void WriteHriste(int[,] hriste)
        {
            for (int i = 0; i < hriste.GetLength(0); i++)
            {
                for (int j = 0; j < hriste.GetLength(1); j++)
                {
                    if (i == cilY && j == cilX)
                    {
                        ConsoleColor c = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("{0,4}", hriste[i, j]);
                        Console.ForegroundColor = c;
                        continue;
                    }
                    if (i == startY && j == startX)
                    {
                        ConsoleColor c = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("{0,4}", hriste[i, j]);
                        Console.ForegroundColor = c;
                        continue;
                    }
                    if (hriste[i, j] == ZavrenePole)
                    {
                        ConsoleColor c = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("{0,4}", hriste[i, j]);
                        Console.ForegroundColor = c;
                        continue;
                    }
                    Console.Write("{0,4}", hriste[i, j]);
                }
                Console.WriteLine();
            }
        }

        public bool DalsiKrok(Smer s)
        {
            pocetKroku++;
            if (X == cilX && Y == cilY)
            {
                WriteHriste(hriste);
                Console.WriteLine("\nKonec po {0} krocích", pocetKroku);
                Console.ReadLine();
                return true;
            }

            switch (s)
            {
                case Smer.Vpravo:
                    Vpravo();
                    break;
                case Smer.Dolu:
                    Dolu();
                    break;
                case Smer.Vlevo:
                    Vlevo();
                    break;
                case Smer.Nahoru:
                    Nahoru();
                    break;
            }

            if (debug)
            {
                WriteHriste(hriste);
                Console.WriteLine();
                //Console.ReadLine();
            }

            Smer smer = OptimalniSmer();
            if (smer != Smer.Nikam) // v této cestě nejde pokračovat
            {
                hriste[Y, X] = znacka++;
                DalsiKrok(smer);
            }
            else
            {
                hriste[Y, X] = ZavrenePole;
                DalsiKrok(OptimalniUstup(--znacka));
            }
            return false;
        }

        // Ústup na předchozí značku
        private Smer OptimalniUstup(int znacka)
        {
            int[] okoli = OkoliPole(X, Y);
            Smer ustup = (Smer)Array.IndexOf(okoli, znacka);
            return ustup;
        }
    }
    class Hriste
    {
        public static int[,] t = {
            { 0,  0,  0,  0,  0, 0, 0,  0, 0,  0,  0,  0 }, // -1 = bariéra
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0,  0,  0 },
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0,  0,  0 },
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0,  0,  0 },
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0,  0,  0 },
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0, -1,  0 },
            { 0,  0,  0,  0, -1, 0, 0, -1, 0, -1, -1, -1 },
            { 0, -1, -1, -1, -1, 0, 0, -1, 0,  0,  0, -1 },
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 },
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 },
            {-1, -1,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 },
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 },
        };

        private static void CistiHriste()
        {
            for (int i = 0; i < t.GetLength(0); i++)
            {
                for (int j = 0; j < t.GetLength(1); j++)
                {
                    if (t[i, j] > 0) // kroky jsou vždy kladné, záporná jsou omezení (hranice a ústupy)
                        t[i, j] = 0;
                }
            }
        }

        public static void Main(string[] args)
        {
            Figura f = new Figura(2, 11, 6, 2, t);
            f.DalsiKrok(Smer.Vlevo);

            CistiHriste();

            f = new Figura(2, 11, 6, 2, t);
            f.DalsiKrok(Smer.Vlevo);

            CistiHriste();

            f = new Figura(2, 11, 6, 2, t); // napodruhé se už nezlepšuje - heuristika!
            f.DalsiKrok(Smer.Vpravo);
        }


    }
}
