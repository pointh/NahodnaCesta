using System;
using System.Linq;

namespace NahodnaCesta
{
    public enum Smer { Vpravo, Dolu, Vlevo, Nahoru, Nikam };
    public enum StavPole { BlokovanePole = -1, ZavrenePole = -2, Hranice = int.MaxValue};

    class Figura
    {
        int pocetKroku = 0;
        public bool debug = true; // zobrazovat/nezobrazovat mezivýsledky
        private int X, Y; // aktuální pozice
        private readonly int cilX, cilY; // pozice cíle
        private readonly int startX, startY; // pozice začátku
        private readonly int _sirka, _vyska; // šířka a výška hřiště
        private readonly int[,] hriste;
        private int znacka = 1; // hodnota navštíveného pole
        

        public Figura(int startRadek, int startSloupec, int endRadek, int endSloupec, int[,] hriste)
        {
            startX = startSloupec; startY = startRadek;
            cilX = endSloupec; cilY = endRadek;

            X = startSloupec; Y = startRadek;
            this._sirka = hriste.GetLength(1);
            this._vyska = hriste.GetLength(0);
            this.hriste = hriste;
        }

        int[] OkoliPole(int x, int y)
        {
            int[] okoli = new int[4];

            if (x + 1 < _sirka && hriste[y, x + 1] >= 0)
                okoli[(int)Smer.Vpravo] = hriste[y, x + 1]; // hodnota pole vpravo
            else
                okoli[(int)Smer.Vpravo] = (int)StavPole.Hranice; // vpravo

            if (y + 1 < _vyska && hriste[y + 1, x] >= 0)
                okoli[(int)Smer.Dolu] = hriste[y + 1, x];
            else
                okoli[(int)Smer.Dolu] = (int)StavPole.Hranice; // dolu

            if (x > 0 && hriste[y, x - 1] >= 0)
                okoli[(int)Smer.Vlevo] = hriste[y, x - 1];
            else
                okoli[(int)Smer.Vlevo] = (int)StavPole.Hranice; // vlevo

            if (y > 0 && hriste[y - 1, x] >= 0)
                okoli[(int)Smer.Nahoru] = hriste[y - 1, x];
            else
                okoli[(int)Smer.Nahoru] = (int)StavPole.Hranice; // nahoru

            return okoli;
        }

        private Smer Strategy(int[] okoli)
        {
            if (Array.IndexOf(okoli, 0) == -1) // v okolí není volné pole, které by už nebylo procházeno
            {
                return Smer.Nikam;
            }

            // tady je co zlepšovat
            // jdi směrem na MINIMÁLNÍ sousední hodnotu
            return (Smer)Array.IndexOf(okoli, okoli.Min());
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

        // Ústup na předchozí značku
        private Smer OptimalniUstup(int znacka)
        {
            int[] okoli = OkoliPole(X, Y);
            Smer ustup = (Smer)Array.IndexOf(okoli, znacka);
            return ustup;
        }

        public void UkonciHledani()
        {
            Hriste.VypisHriste(cilX, cilY, startX, startY);
            Console.WriteLine("\nKonec po {0} krocích", pocetKroku);
            Console.ReadLine();
        }

        public bool DalsiKrok(Smer s)
        {
            pocetKroku++;
            if (X == cilX && Y == cilY)
            {
                UkonciHledani();
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
                Hriste.VypisHriste(cilX, cilY,startX, startY);
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
                hriste[Y, X] = (int) StavPole.ZavrenePole;
                DalsiKrok(OptimalniUstup(--znacka));
            }
            return false;
        }

       
    }
    class Hriste
    {
        public static int[,] hriste = {
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
            for (int i = 0; i < hriste.GetLength(0); i++)
            {
                for (int j = 0; j < hriste.GetLength(1); j++)
                {
                    if (hriste[i, j] > 0) // kroky jsou vždy kladné, záporná jsou omezení (hranice a ústupy)
                        hriste[i, j] = 0;
                }
            }
        }

        public static void VypisHriste(int cilX, int cilY, int startX, int startY)
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
                    if (hriste[i, j] == (int)StavPole.ZavrenePole)
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
        public static void Main()
        {
            Figura f = new Figura(2, 11, 6, 2, hriste);
            f.DalsiKrok(Smer.Vlevo);

            CistiHriste();

            f = new Figura(2, 11, 6, 2, hriste);
            f.DalsiKrok(Smer.Vlevo);

            CistiHriste();

            f = new Figura(2, 11, 6, 2, hriste); // napodruhé se už nezlepšuje - heuristika!
            f.DalsiKrok(Smer.Vpravo);
        }


    }
}
