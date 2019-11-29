using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StromoveHledani
{
    class Uzel
    {
        public int x, y;
        public Uzel(int x, int y, int[,] hriste)
        {
            if (hriste[x, y] == (int)StavPole.Blokovane)
            {
                return;
            }

            this.x = x;
            this.y = y;

            hriste[x, y] = (int)StavPole.Navstivene;
        }
    }
}
