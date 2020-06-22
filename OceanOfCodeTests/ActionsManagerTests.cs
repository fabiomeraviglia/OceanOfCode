using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestClass()]
    public class ActionsManagerTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IsValidActionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidMapPositionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void MovedPositionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void MapSectorTest()
        {
            Position p1, p2, p3;

            p1 = new Position(14, 14);
            p2 = new Position(5, 9);
            p3 = new Position(14, 0);

            int s1=9, s2=5, s3=3;

            Assert.IsTrue(Utils.MapSector(p1) == s1);
            Assert.IsTrue(Utils.MapSector(p2) == s2);
            Assert.IsTrue(Utils.MapSector(p3) == s3);
        }
    }
}