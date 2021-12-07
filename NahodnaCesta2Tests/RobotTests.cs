using Microsoft.VisualStudio.TestTools.UnitTesting;
using NahodnaCesta2;
using System;
using System.Collections.Generic;
using System.Text;

namespace NahodnaCesta2.Tests
{
    [TestClass()]
    public class RobotTests
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

        [TestMethod()]
        [DataRow(3, 11)]
        public void IsIlegalPositionTest(int x, int y)
        {
            Robot r = new Robot(field, 2, 11, 6, 2);
            Assert.IsTrue(r.IsIlegalPosition(3, 11) == false);
        }
    }
}