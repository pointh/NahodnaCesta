using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace StromoveHledani
{
    public enum Smer { Vpravo, Nahoru, Vlevo, Dolu, Nikam };
    public enum StavPole { BlokovanePole = -1, Navstivene = -2, Hranice = int.MaxValue, Volne = 0 };
    class Hriste
    {
        public static int[,] hriste = {
            { 0,  0,  0,  0, -1, 0, 0,  0, 0, -1, -1,  0 }, // -1 = bariéra
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0, -1,  0 },
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0, -1,  0 },
            { 0,  0,  0,  0, -1, 0, 0,  0, 0,  0,  0,  0 },
            { 0,  0,  0,  0, -1, 0, 0, -1, 0,  0, -1,  0 },
            { 0, -1,  0,  0, -1, 0, 0, -1, 0,  0, -1,  0 },
            { 0, -1,  0,  0, -1, 0, 0, -1, 0, -1, -1, -1 },
            { 0, -1, -1, -1, -1, 0, 0, -1, 0,  0,  0, -1 },
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 },
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 },
            {-1, -1,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 },
            { 0,  0,  0,  0,  0, 0, 0, -1, 0,  0,  0,  0 },
        };

        public static void VypisHriste(int cilX, int cilY, int startX, int startY)
        {
            Thread.Sleep(500);
            Console.Clear();
            for (int i = 0; i < hriste.GetLength(0); i++)
            {
                for (int j = 0; j < hriste.GetLength(1); j++)
                {
                    if (i == cilX && j == cilY)
                    {
                        ConsoleColor c = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("{0,4}", hriste[i, j]);
                        Console.ForegroundColor = c;
                        continue;
                    }
                    if (i == startX && j == startY)
                    {
                        ConsoleColor c = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("{0,4}", hriste[i, j]);
                        Console.ForegroundColor = c;
                        continue;
                    }
                    if (hriste[i, j] == (int)StavPole.Navstivene)
                    {
                        ConsoleColor c = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("{0,4}", "#");
                        Console.ForegroundColor = c;
                        continue;
                    }
                    Console.Write("{0,4}", hriste[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static bool ObsahujeIdentickyUzel(List<Uzel> list, Uzel u)
        {
            foreach (Uzel p in list)
                if (p.x == u.x && p.y == u.y) return true;

            return false;
        }

        // Používá se od začátku do konce i obráceně
        public static List<Uzel> GenerujPotomky(Uzel u)
        {
            List<Uzel> l = new List<Uzel>();
            foreach ( var s in Enum.GetValues(typeof(Smer)))
                switch (s) {
                    case Smer.Vpravo:
                        if (u.x < hriste.GetLength(1) - 1 && hriste[u.x + 1, u.y] != (int)StavPole.BlokovanePole)
                            l.Add(new Uzel(u.x + 1, u.y, hriste));
                        break;
                    case Smer.Nahoru:
                        if (u.y > 0 && hriste[u.x, u.y - 1] != (int)StavPole.BlokovanePole)
                            l.Add(new Uzel(u.x, u.y-1, hriste));
                        break;
                    case Smer.Vlevo:
                        if (u.x - 1 > -1 && hriste[u.x - 1, u.y] != (int)StavPole.BlokovanePole)
                            l.Add(new Uzel(u.x - 1, u.y, hriste));
                        break;
                    case Smer.Dolu:
                        if (u.y < hriste.GetLength(0) - 1 && hriste[u.x, u.y + 1] != (int)StavPole.BlokovanePole)
                            l.Add(new Uzel(u.x, u.y + 1, hriste));
                        break;
                }
            return l;
        }

        public static void Deduplikuj(List<Uzel> l)
        {
            for (int i = 0; i < l.Count-1; i++)
                for (int j = i + 1; j < l.Count; j++)
                    if (l[i].x == l[j].x && l[i].y == l[j].y)
                        l.RemoveAt(j);
        }

        // Ze současné generace smaž uzly z předchozí generace
        public static void VycistiMinulouGeneraci(List<Uzel> soucasna, List<Uzel> minula)
        {
            for (int i = 0; i < soucasna.Count; i++)
                for (int j = i + 1; j < minula.Count; j++)
                    if (soucasna[i].x == minula[j].x && soucasna[i].y == minula[j].y)
                        soucasna.RemoveAt(i);
        }

        public static void VypisListUzlu(List<Uzel> l)
        {
            int i = 1;
            foreach (Uzel u in l)
                Console.WriteLine($"{i++}. ({u.x}, {u.y})");
        }

        public static List<Uzel> ZpetnyChodHledani(List<List<Uzel>> l, Uzel konec)
        {
            List<Uzel> cesta = new List<Uzel>();
            cesta.Add(konec);
            
            for (int i = l.Count - 2; i >= 0; i--)
            {
                List<Uzel> n = GenerujPotomky(konec);
                for (int j = 0; j < n.Count; j++)
                {
                    if (ObsahujeIdentickyUzel(l[i], n[j]))
                    {
                        cesta.Add(n[j]);
                        konec = n[j];
                        break;
                    }
                }
            }

            return cesta;
        }

        public static void Main()
        {
            int startX = 0, startY = 2, endX = 2, endY = 11;
            List<List<Uzel>> generace = new List<List<Uzel>>(1);

            Uzel konecnyUzel = new Uzel(endX, endY, hriste);
            hriste[endX, endY] = (int)StavPole.Volne;

            generace.Add(new List<Uzel>());
            generace[0].Add(new Uzel(startX, startY, hriste));
            
            int genIdx = 0;

            while (true)
            {
                genIdx++;
                generace.Add(new List<Uzel>());

                foreach (Uzel u in generace[genIdx - 1])
                {
                    List<Uzel> novaGenerace = GenerujPotomky(u);
                    generace[genIdx].AddRange(novaGenerace);
                }

                Deduplikuj(generace[genIdx]);
                VycistiMinulouGeneraci(generace[genIdx], generace[genIdx - 1]);
                Hriste.VypisHriste(startX, startY, endX, endY);            

                if (ObsahujeIdentickyUzel(generace[genIdx], konecnyUzel))
                {
                    Console.WriteLine($"Dosaženo v {genIdx} krocích");
                    List<Uzel> reversedSteps = ZpetnyChodHledani(generace, konecnyUzel);
                    reversedSteps.Reverse();
                    VypisListUzlu(reversedSteps);
                    break;
                }  
            }
            Console.ReadLine();
        }


    }
}
