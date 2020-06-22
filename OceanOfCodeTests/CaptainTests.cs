using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestClass()]
    public class CaptainTests
    {
        [TestInitialize]
        public void SetMap()
        {
            Random r = new Random(4);
            Map.matrix = new Map.GroundType[Map.MAP_SIZE, Map.MAP_SIZE];
            for (int i = 0; i < Map.MAP_SIZE; i++)
            {
                for (int j = 0; j < Map.MAP_SIZE; j++)
                {
                    Map.GroundType type;
                    switch (r.Next(3))
                    {
                        case 0:
                            type = Map.GroundType.LAND;
                            break;

                        default: type = Map.GroundType.WATER; break;

                    }
                    Map.matrix[i, j] = type;
                }
            }

        }
        [TestMethod()]
        public void CaptainTest()
        {

        /*    Submarine sub = new Submarine(new Position(3, 3));

            Captain cap = new Captain(sub);
            cap.possibleEnemyPosition.Clear();
            cap.possibleEnemyPosition.Add(new EnemySubmarine(new Position(2, 3)));

            cap.strategy = new SearchAndDestroyStrategy(cap);
            cap.strategy.Decide();*/

        }
        [TestMethod()]
        public void DjikstraTest()
        {
            Map.matrix[0, 0] = Map.GroundType.WATER;

            Map.matrix[0, 10] = Map.GroundType.WATER;

            string map = Map.PrintMap();

            Utils.DistanceDijkstra(new Position(0, 0), new Position(10, 0));

        }
        [TestMethod()]
        public void DecideActionTest()
        {

            Submarine submarine = new Submarine(new Position(7, 7));
            Captain captain = new Captain(submarine);

            for(int i = 0; i<100;i++)
            {
                string action = captain.DecideAction();

                ActionsManager.Execute(submarine, action);
            }


        }
    }
}