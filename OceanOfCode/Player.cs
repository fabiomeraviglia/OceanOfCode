using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player
{
    public static Random r = new Random(1);

    static void Main(string[] args)
    {
        //Test();

        string[] inputs = Console.ReadLine().Split(' ');

        int width = int.Parse(inputs[0]);
        int height = int.Parse(inputs[1]);
        int myId = int.Parse(inputs[2]);

        Parser.SetMap(height);

        Console.Error.WriteLine(Map.PrintMap());


        Position startingPos = Captain.DecideStartingPosition();

        Submarine submarine = new Submarine(startingPos);
        Captain captain = new Captain(submarine);

        Console.WriteLine(startingPos.X + " " + startingPos.Y);

        while (true)
        {

            inputs = Console.ReadLine().Split(' ');

            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
            int myLife = int.Parse(inputs[2]);
            int oppLife = int.Parse(inputs[3]);
            int torpedoCooldown = int.Parse(inputs[4]);
            int sonarCooldown = int.Parse(inputs[5]);
            int silenceCooldown = int.Parse(inputs[6]);
            int mineCooldown = int.Parse(inputs[7]);
            string sonarResult = Console.ReadLine();
            string opponentOrders = Console.ReadLine();



            Console.Error.WriteLine(opponentOrders);
            try
            {

                if (sonarResult != "NA")
                {
                    captain.UpdateSonarResult(sonarResult);
                }

                if (opponentOrders != "NA")
                {
                    captain.UpdateEnemyStatus(opponentOrders);
                }

                captain.UpdateEnemyStatus("LIFE " + oppLife);


                captain.UpdateStrategy();

                string action = captain.DecideAction();

                ActionsManager.Execute(submarine, action);

                Console.Error.WriteLine(submarine.ToString());

                Console.WriteLine(action);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                Console.WriteLine("SURFACE");
                submarine = new Submarine(new Position(x, y));
                submarine.Life = myLife;
                captain = new Captain(submarine);


            }
        }


    }

    static void Test()
    {
        // string[]  inputs = Console.ReadLine().Split(' ');

        string[] inputs = { "15", "15", "0" };

        int width = int.Parse(inputs[0]);
        int height = int.Parse(inputs[1]);
        int myId = int.Parse(inputs[2]);

        SetMapTest(height);

        Console.Error.WriteLine(Map.PrintMap());


        Position startingPos = Captain.DecideStartingPosition();

        Submarine submarine = new Submarine(startingPos);
        Captain captain = new Captain(submarine);

        Console.WriteLine(startingPos.X + " " + startingPos.Y);

        String[] opponentOrders = new string[] { "MOVE E", "MOVE E", "MOVE S", "MOVE S", "MOVE W", "MOVE W", "MOVE N", "MOVE N" };

        int i = 0;
        int oppLife = 6;
        while (true)
        {

            //     inputs = Console.ReadLine().Split(' ');

            inputs = new string[] { "3", "5", "6", "6", "3", "4", "6", "3", "NA" };

            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
            int myLife = int.Parse(inputs[2]);

            int torpedoCooldown = int.Parse(inputs[4]);
            int sonarCooldown = int.Parse(inputs[5]);
            int silenceCooldown = int.Parse(inputs[6]);
            int mineCooldown = int.Parse(inputs[7]);
            // string sonarResult = Console.ReadLine();
            // string opponentOrders = Console.ReadLine();


            //  string sonarResult = "3";

            if (opponentOrders[i % opponentOrders.Length] != "NA")
            {
                if (i % (opponentOrders.Length / 2) == 0 && i != 0)
                {

                    captain.UpdateEnemyStatus("SURFACE 1");
                    oppLife--;
                }
                else
                {
                    captain.UpdateEnemyStatus(opponentOrders[i % opponentOrders.Length]);
                }
            }
            captain.UpdateEnemyStatus("LIFE " + oppLife);

            captain.UpdateStrategy();
            string action = captain.DecideAction();

            ActionsManager.Execute(submarine, action);

            Console.Error.WriteLine(submarine.ToString());

            Console.WriteLine(action);
            i++;


        }
    }
    public static void SetMapTest(int height)
    {
        Map.MAP_SIZE = height;
        Map.matrix = new Map.GroundType[height, height];

        for (int i = 0; i < height; i++)
        {
            //string line = Console.ReadLine();

            string line = "...............";

            for (int j = 0; j < line.Length; j++)
            {
                Map.GroundType type;
                switch (line.Substring(j, 1))
                {
                    case ".":
                        type = Map.GroundType.WATER;
                        break;

                    case "x":
                        type = Map.GroundType.LAND;
                        break;

                    default: throw new Exception("unknown ground type");

                }
                Map.matrix[i, j] = type;
            }
        }


    }
}
public static class Parser
{
    public static void SetMap(int height)
    {
        Map.MAP_SIZE = height;
        Map.matrix = new Map.GroundType[height, height];

        for (int i = 0; i < height; i++)
        {
            string line = Console.ReadLine();

            for (int j = 0; j < line.Length; j++)
            {
                Map.GroundType type;
                switch (line.Substring(j, 1))
                {
                    case ".":
                        type = Map.GroundType.WATER;
                        break;

                    case "x":
                        type = Map.GroundType.LAND;
                        break;

                    default: throw new Exception("unknown ground type");

                }
                Map.matrix[i, j] = type;
            }
        }


    }




}
public static class Map
{
    public static int MAP_SIZE = 15;
    public static GroundType[,] matrix = new GroundType[MAP_SIZE, MAP_SIZE];
    public static string[] cardinalDirections = { "N", "E", "S", "W" };

    public static GroundType Get(Position position)
    {
        return matrix[position.Y, position.X];
    }

    public enum GroundType { WATER, LAND };

    public static string PrintMap()
    {
        string s = "";
        for (int i = 0; i < MAP_SIZE; i++)
        {
            for (int j = 0; j < MAP_SIZE; j++)
            {
                s += matrix[i, j].ToString().Substring(0, 1);
            }
            s += "\r\n";
        }
        return s;
    }
}
public class Position
{

    public Position(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }
    public int X { get; }
    public int Y { get; }

    public override bool Equals(object obj)
    {
        return obj != null && obj is Position position &&
               X == position.X &&
               Y == position.Y;
    }

    public static bool operator ==(Position a, Position b)
    {
        if (object.ReferenceEquals(a, null))
        {
            return object.ReferenceEquals(b, null);
        }

        return a.Equals(b);
    }
    public static bool operator !=(Position a, Position b)
    {
        if (object.ReferenceEquals(a, null))
        {
            return !object.ReferenceEquals(b, null);
        }

        return !a.Equals(b);

    }


    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }

    public override string ToString()
    {
        return X + " " + Y;
    }
}
public class Submarine
{

    public Submarine(Submarine submarine)
    {
        this.Position = submarine.Position;
        this.Life = submarine.Life;
        this.TorpedoCharge = submarine.TorpedoCharge;
        this.SonarCharge = submarine.SonarCharge;
        this.SilenceCharge = submarine.SilenceCharge;
        this.MineCharge = submarine.MineCharge;

        this.VisitedCells = new HashSet<Position>(submarine.VisitedCells);
        this.Mines.UnionWith(submarine.Mines);
        if (submarine is EnemySubmarine)
            Mines = new HashSet<Position>();

    }

    public Submarine(Position position)
    {
        this.Position = position;
        this.Life = 6;
        TorpedoCharge = 0;
        SonarCharge = 0;
        SilenceCharge = 0;
        this.MineCharge = 0;
        VisitedCells.Add(Position);
    }
    public Submarine(Position position, int life)
    {
        this.Position = position;
        this.Life = life;
        TorpedoCharge = 0;
        SonarCharge = 0;
        SilenceCharge = 0;
        this.MineCharge = 0;
        VisitedCells.Add(Position);
    }

    public virtual bool Alive { get => Life > 0; }

    public virtual int Life { get; set; }

    public virtual int TorpedoCharge { get; private set; }

    public virtual int SilenceCharge { get; private set; }

    public virtual int SonarCharge { get; private set; }

    public virtual int MineCharge { get; protected set; }

    public virtual HashSet<Position> VisitedCells { get; } = new HashSet<Position>();


    public virtual HashSet<Position> Mines { get; } = new HashSet<Position>();

    public virtual Position Position { get; private set; }



    public override string ToString()
    {
        return "sub at " + Position.ToString() + " life:" + Life + " torp:" + TorpedoCharge + " sil:" + SilenceCharge + " son:" + SonarCharge + " mines:" + Mines.Count;
    }

    public virtual void Move(string direction)
    {
        Position = Utils.MovedPosition(Position, direction);
        VisitedCells.Add(Position);
    }
    public virtual void Move(string direction, int distance)
    {
        for (int i = 0; i < distance; i++)
        {
            Move(direction);
        }
    }
    public virtual void Surface()
    {
        Life--;
        VisitedCells.Clear();
        VisitedCells.Add(Position);
    }

    public virtual void TakeDamage(int amount)
    {
        Life -= amount;
    }
    public virtual void ChargeTorpedo()
    {
        if (TorpedoCharge < 3)
        {
            TorpedoCharge++;
        }
    }
    public virtual void FireTorpedo()
    {
        TorpedoCharge = 0;
    }
    public virtual void Sonar()
    {
        SonarCharge = 0;
    }
    public virtual void ChargeSonar()
    {
        if (SonarCharge < 4)
        {
            SonarCharge++;
        }
    }
    public virtual void Silence()
    {
        SilenceCharge = 0;
    }
    public virtual void ChargeSilence()
    {
        if (SilenceCharge < 6)
        {
            SilenceCharge++;
        }
    }


    public virtual void Mine(Position position)
    {
        MineCharge = 0;
        Mines.Add(position);
    }
    public virtual void ChargeMine()
    {
        if (MineCharge < 3)
        {
            MineCharge++;
        }
    }
    public virtual void Trigger(Position position)
    {
        Mines.Remove(position);
    }
    public virtual Submarine Union(Submarine sub)
    {
        Submarine union = new Submarine(this);

        union.VisitedCells.IntersectWith(sub.VisitedCells);

        if (!(sub is EnemySubmarine))
        {
            union.Mines.UnionWith(sub.Mines);
        }

        return union;
    }
}

public class EnemySubmarine : Submarine
{
    int totalCharge = 0;
    public EnemySubmarine(Submarine submarine) : base(submarine)
    {
        this.totalCharge = submarine.TorpedoCharge + submarine.SilenceCharge + submarine.SonarCharge + submarine.MineCharge;

        this.possibleMines = new List<EnemyMine>();

        foreach (Position mine in submarine.Mines)
        {
            possibleMines.Add(new EnemyMine(new List<Position> { mine }));
        }

        possibleLife.Add(submarine.Life);
    }
    public EnemySubmarine(EnemySubmarine submarine) : base(submarine)
    {
        this.totalCharge = submarine.totalCharge;
        this.possibleMines = new List<EnemyMine>(submarine.possibleMines);
        possibleLife.UnionWith(submarine.possibleLife);
    }
    public EnemySubmarine(Position position) : base(position)
    {

        possibleLife.Add(Life);
    }
    public void Charge()
    {
        totalCharge++;
        base.ChargeTorpedo();
        base.ChargeSilence();
        base.ChargeSonar();
        base.ChargeMine();
    }
    public override string ToString()
    {
        return "enemysub at " + Position.ToString() + " life:" + Life + " torp:" + TorpedoCharge + " sil:" + SilenceCharge + " son:" + SonarCharge + " tot:" + totalCharge + " mines:" + possibleMines.Count + " lifes:" + possibleLife.Count;
    }
    public override int TorpedoCharge { get => Math.Min(base.TorpedoCharge, totalCharge); }

    public override int SilenceCharge { get => Math.Min(base.SilenceCharge, totalCharge); }

    public override int SonarCharge { get => Math.Min(base.SonarCharge, totalCharge); }

    public override int MineCharge { get => Math.Min(base.MineCharge, totalCharge); }

    public override HashSet<Position> Mines { get => PossibleMines(); }

    public List<EnemyMine> possibleMines = new List<EnemyMine>();

    public HashSet<int> possibleLife = new HashSet<int>();

    private HashSet<Position> PossibleMines()
    {

        HashSet<Position> positions = new HashSet<Position>();
        foreach (EnemyMine mine in possibleMines)
        {
            positions.UnionWith(mine.possiblePositions);

        }
        return positions;
    }

    public override void FireTorpedo()
    {
        totalCharge -= TorpedoCharge;
        base.FireTorpedo();
    }
    public override void Sonar()
    {
        totalCharge -= SonarCharge;
        base.Sonar();
    }
    public override void Silence()
    {
        totalCharge -= SilenceCharge;
        base.Silence();
    }
    public override void Surface()
    {
        base.Surface();
        HashSet<int> tmp = new HashSet<int>();
        foreach (int i in possibleLife)
        {
            tmp.Add(i - 1);
        }
        possibleLife = tmp;
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        HashSet<int> tmp = new HashSet<int>();
        foreach (int i in possibleLife)
        {
            tmp.Add(i - amount);
        }
        possibleLife = tmp;
    }
    public override void Mine(Position position)
    {
        totalCharge -= MineCharge;
        MineCharge = 0;

        List<Position> possiblePositions = new List<Position>();
        foreach (string dir in Map.cardinalDirections)
        {
            Position p = Utils.MovedPosition(Position, dir);
            if (Utils.ValidMapPosition(p) && Map.Get(p) == Map.GroundType.WATER)
            {
                possiblePositions.Add(p);
            }
        }

        possibleMines.Add(new EnemyMine(possiblePositions));



    }

    public override void Trigger(Position position)
    {
        List<EnemyMine> triggered = new List<EnemyMine>();
        foreach (EnemyMine mine in possibleMines)
        {
            if (mine.possiblePositions.Contains(position))
            {
                triggered.Add(mine);
            }
        }
        if (triggered.Count == 1)
        {
            triggered[0].Multiplicity--;
            if (triggered[0].Multiplicity <= 0)
            {
                possibleMines.Remove(triggered[0]);
            }
        }
        if (triggered.Count > 1)
        {
            EnemyMine newMine = new EnemyMine();
            newMine.Multiplicity = -1;
            for (int i = 0; i < triggered.Count; i++)
            {
                newMine.possiblePositions.UnionWith(triggered[i].possiblePositions);
                newMine.Multiplicity += triggered[i].Multiplicity;
                possibleMines.Remove(triggered[i]);
            }
            newMine.possiblePositions.Remove(position);

            possibleMines.Add(newMine);
        }
    }
    public EnemySubmarine Union(EnemySubmarine sub)
    {

        EnemySubmarine unionSub = new EnemySubmarine(base.Union(sub));

        unionSub.possibleLife = new HashSet<int>();
        unionSub.possibleLife.UnionWith(sub.possibleLife);
        unionSub.possibleLife.UnionWith(this.possibleLife);

        if (sub.totalCharge != this.totalCharge)
            throw new Exception("Total charge should be the same!");

        unionSub.totalCharge = this.totalCharge;

        if (sub.possibleMines.Count != this.possibleMines.Count)
            throw new Exception("Total mines should be the same!" + sub.possibleMines.Count + " " + this.possibleMines.Count);

        unionSub.possibleMines = new List<EnemyMine>();

        for (int i = 0; i < possibleMines.Count; i++)
        {
            EnemyMine mine = new EnemyMine();
            mine.Multiplicity = possibleMines[i].Multiplicity + sub.possibleMines[i].Multiplicity;

            mine.possiblePositions.UnionWith(possibleMines[i].possiblePositions);
            mine.possiblePositions.UnionWith(sub.possibleMines[i].possiblePositions);

            unionSub.possibleMines.Add(mine);
        }

        return unionSub;
    }
}

public class EnemyMine
{
    public HashSet<Position> possiblePositions = new HashSet<Position>();

    public int Multiplicity { get; set; } = 1;

    public EnemyMine(List<Position> possiblePositions)
    {
        this.possiblePositions = new HashSet<Position>(possiblePositions);

    }
    public EnemyMine()
    {

    }
}
public class PossibleSubmarines
{
    Dictionary<Position, EnemySubmarine> submarinesByPosition = new Dictionary<Position, EnemySubmarine>();

    public List<EnemySubmarine> List { get => submarinesByPosition.Values.ToList(); }

    public PossibleSubmarines()
    {

    }


    public void Add(EnemySubmarine submarine)
    {
        if (!submarinesByPosition.ContainsKey(submarine.Position))
        {
            submarinesByPosition[submarine.Position] = submarine;
        }
        else
        {
            submarinesByPosition[submarine.Position] = UniteSubmarines(submarinesByPosition[submarine.Position], submarine);

        }
    }

    private EnemySubmarine UniteSubmarines(EnemySubmarine s1, EnemySubmarine s2)
    {

        if (s1 == null)
        {
            Console.Error.WriteLine("s1 null");
            return s2;
        }
        if (s2 == null)
        {
            Console.Error.WriteLine("s2 null");
            return s1;
        }

        return s1.Union(s2);
    }

    public int Count { get => submarinesByPosition.Count; }

    public void Add(List<EnemySubmarine> enemySubmarines)
    {
        foreach (EnemySubmarine s in enemySubmarines)
        {
            if (s == null)
                Console.Error.WriteLine("null submarine!!");
            else
                Add(s);
        }
    }

    public EnemySubmarine Get(Position position)
    {
        if (submarinesByPosition.ContainsKey(position))
        {
            return submarinesByPosition[position];
        }
        return null;
    }

    public EnemySubmarine Get(int index)
    {
        if (index >= submarinesByPosition.Count)
            throw new Exception("Out of range index");
        return submarinesByPosition.ToList()[index].Value;

    }

    public void Update(List<EnemySubmarine> submarines)
    {
        submarinesByPosition.Clear();
        Add(submarines);
    }
}


public class Captain
{
    public Submarine submarine;
    public enum Phase { INVISIBLE, SEARCH_AND_DESTROY }
    public Strategy strategy;
    public Phase phase = Phase.INVISIBLE;

    public PossibleSubmarines possibleEnemyPosition = new PossibleSubmarines();

    public PossibleSubmarines ourPossiblePosition = new PossibleSubmarines();

    public Captain(Submarine submarine)
    {
        this.submarine = submarine;

        possibleEnemyPosition.Add(PossibilitiesCalculator.GetAllPossibleSubmarine());
        ourPossiblePosition.Add(PossibilitiesCalculator.GetAllPossibleSubmarine());
        strategy = new InvisibleStrategy(this);
    }

    public int sonarSectorLastTime = 0;
    public void UpdateSonarResult(string result)
    {
        possibleEnemyPosition.Update(PossibilitiesCalculator.GetPossibleSubmarines(possibleEnemyPosition.List, "R_SONAR " + sonarSectorLastTime + " " + result));

    }

    public void UpdateEnemyStatus(string enemyActions)
    {
        //  Console.Error.WriteLine("BEFORE UPdating enemy status " + possibleEnemyPosition.Count+ " " +enemyActions);

        possibleEnemyPosition.Update(PossibilitiesCalculator.GetPossibleSubmarines(possibleEnemyPosition.List, enemyActions));

        //    Console.Error.WriteLine("after UPdating enemy status = " + possibleEnemyPosition.Count + " " + enemyActions);

        /*   for (int i = 0; i < possibleEnemyPosition.Count && i < 30; i++)
           {
               Console.Error.WriteLine(possibleEnemyPosition.Get(i).ToString());
           }*/
    }

    public void UpdateStrategy()
    {
        /*  Console.Error.WriteLine("enemy positions count = " + possibleEnemyPosition.Count);

           Console.Error.WriteLine("our positions count = " + ourPossiblePosition.Count);

           if (possibleEnemyPosition.Count == 1) Console.Error.WriteLine("enemy at" + possibleEnemyPosition.Get(0).Position.ToString());
           */
        TargetPoint possibleTarget = PossibilitiesCalculator.BestTargetPoint(possibleEnemyPosition, submarine);

        if (phase == Phase.INVISIBLE && possibleTarget.AverageDamage > Constants.DamageToSwitchStrategy)
        {
            Console.Error.WriteLine("Updating strategy to Search and Destroy");
            phase = Phase.SEARCH_AND_DESTROY;
            strategy = new SearchAndDestroyStrategy(this);
            return;
        }
        if (phase == Phase.SEARCH_AND_DESTROY && possibleTarget.AverageDamage < Constants.DamageToSwitchStrategy)
        {
            Console.Error.WriteLine("Updating strategy to invisible");
            phase = Phase.INVISIBLE;
            strategy = new InvisibleStrategy(this);
            return;
        }
    }

    public string DecideAction()
    {

        /*   Console.Error.WriteLine("DECIDE ACTION... Possible enemies:");
           for (int i = 0; i < possibleEnemyPosition.Count && i < 70; i++)
           {
               Console.Error.WriteLine(possibleEnemyPosition.Get(i).ToString());
           }*/
        string action = strategy.Decide();

        Console.Error.WriteLine("Action decided:" + action);
        ourPossiblePosition.Update(PossibilitiesCalculator.GetPossibleSubmarines(ourPossiblePosition.List, action));

        possibleEnemyPosition.Update(PossibilitiesCalculator.GetPossibleSubmarinesAfterEnemyActions(possibleEnemyPosition.List, action));

        /*    Console.Error.WriteLine("Possible enemies after action:");
            for (int i = 0; i < possibleEnemyPosition.Count && i < 10; i++)
            {
                Console.Error.WriteLine(possibleEnemyPosition.Get(i).ToString());
            }*/

        return action;
    }


    public static Position DecideStartingPosition()
    {
        Position pos = null;
        while (pos == null)
        {
            int x = Player.r.Next(Map.MAP_SIZE);
            int y = Player.r.Next(Map.MAP_SIZE);
            Position rpos = new Position(x, y);
            if (Map.Get(rpos) == Map.GroundType.WATER)
                pos = rpos;
        }
        return pos;
    }
}



public abstract class Strategy
{
    protected Captain captain;
    public Strategy(Captain captain)
    {
        this.captain = captain;
    }
    public abstract string Decide();
}

public class InvisibleStrategy : Strategy
{
    public InvisibleStrategy(Captain captain) : base(captain)
    { }

    public override string Decide()
    {
        string action = "";
        bool sonar = false;
        string silenceAction = "";
        if (captain.submarine.SilenceCharge == 6 && ShouldSilentAction())
        {
            silenceAction = DecideSilenceAction();
            action += silenceAction + "|";
        }
        else
        {
            if (captain.submarine.SonarCharge == 4)
            {
                action += DecideSonarAction() + "|";
                sonar = true;
            }
        }

        string powerUp = DecidePowerUp(sonar);

        string movement = DecideMovementAction(silenceAction);

        if (movement.Substring(0, 4) == "MOVE")
        {
            movement += " " + powerUp;
        }

        action += movement + "|";


        return action.Substring(0, action.Length - 1);
    }

    private bool ShouldSilentAction()
    {
        if (captain.ourPossiblePosition.Count > Constants.LOW_NUMBER_POSSIBLE_POSITIONS)
            return false;
        int possiblePositionAfterAction = PossibilitiesCalculator.GetPossibleSubmarines(captain.ourPossiblePosition.List, "SILENCE").Count;

        float relativeIncrement = (float)possiblePositionAfterAction / (float)captain.ourPossiblePosition.Count;
        Console.Error.WriteLine("Possible positions after silence: " + possiblePositionAfterAction + " increment:" + relativeIncrement);


        return relativeIncrement > Constants.MIN_RELATIVE_INCREMENT_SILENT_ACTION;
    }

    private string DecidePowerUp(bool useSonarThisTurn)
    {
        string powerUp;

        if (captain.submarine.TorpedoCharge == 3)
        {
            if (captain.submarine.SilenceCharge == 6)
            {
                if (captain.submarine.SonarCharge == 4 && useSonarThisTurn == false)
                {
                    powerUp = "MINE";
                }
                else
                {
                    powerUp = "SONAR";
                }
            }
            else
            {
                powerUp = "SILENCE";
            }
        }
        else
        {
            powerUp = "TORPEDO";
        }

        return powerUp;
    }

    private string DecideSonarAction()
    {
        int bestSector = 0;
        int minPossibilities = captain.possibleEnemyPosition.Count + 1;
        for (int sector = 1; sector <= 9; sector++)
        {
            int possibleYes = PossibilitiesCalculator.GetPossibleSubmarines(captain.possibleEnemyPosition.List, "R_SONAR " + sector + " Y").Count;


            int possibleNo = PossibilitiesCalculator.GetPossibleSubmarines(captain.possibleEnemyPosition.List, "R_SONAR " + sector + " N").Count;

            Console.Error.WriteLine("Sonar decision sector " + sector + " Y:" + possibleYes + " N:" + possibleNo);

            int max = Math.Max(possibleNo, possibleYes);
            if (max < minPossibilities)
            {
                minPossibilities = max;
                bestSector = sector;
            }
        }

        if (bestSector == 0) throw new Exception("Not found best sector");
        //   int sector = Player.r.Next(1, 10);


        captain.sonarSectorLastTime = bestSector;

        return "SONAR " + bestSector;
    }

    private string DecideSilenceAction()
    {
        List<EnemySubmarine> possibleFuturePositions = PossibilitiesCalculator.GetPossibleSubmarinesAfterUnknownEffectsAction(new EnemySubmarine(captain.submarine), "SILENCE");
        List<EnemySubmarine> biggestAreaFuture = new List<EnemySubmarine>();
        int biggestArea = 0;
        foreach (EnemySubmarine submarine in possibleFuturePositions)
        {

            int area = Utils.AreaSize(submarine, submarine.Position) + Player.r.Next(3);//RANDOM FACTOR FOR UNPREDICTABILITY
            if (area > biggestArea)
            {
                biggestAreaFuture.Clear();
                biggestArea = area;
            }

            if (area == biggestArea)
            {
                biggestAreaFuture.Add(submarine);
            }
        }



        Position p = biggestAreaFuture[Player.r.Next(biggestAreaFuture.Count)].Position;

        string direction = "N";
        int distance = 0;
        if (p.X > captain.submarine.Position.X)
        {
            direction = "E";
            distance = p.X - captain.submarine.Position.X;
        }
        if (p.X < captain.submarine.Position.X)
        {
            direction = "W";
            distance = captain.submarine.Position.X - p.X;
        }
        if (p.Y > captain.submarine.Position.Y)
        {
            direction = "S";
            distance = p.Y - captain.submarine.Position.Y;
        }
        if (p.Y < captain.submarine.Position.Y)
        {
            direction = "N";
            distance = captain.submarine.Position.Y - p.Y;
        }


        return "SILENCE " + direction + " " + distance;
    }

    public string DecideMovementAction(string silenceAction)
    {
        int maxAreaLeft = 0;
        string bestAction = "SURFACE";
        int bestPossiblePositions = captain.ourPossiblePosition.Count;

        EnemySubmarine submarine = new EnemySubmarine(captain.submarine);
        var ourPossiblePosition = captain.ourPossiblePosition.List;

        Console.Error.WriteLine(silenceAction + "  " + submarine);
        if (silenceAction != "")
        {
            ActionsManager.Execute(submarine, silenceAction);
            ourPossiblePosition = PossibilitiesCalculator.ComputePossibleSubmarines(ourPossiblePosition, silenceAction);
        }


        Console.Error.WriteLine(submarine);

        foreach (string direction in Map.cardinalDirections)
        {
            string action = "MOVE " + direction;

            if (ActionsManager.IsValidAction(submarine, action))
            {
                int areaSize = Utils.AreaSize(submarine, Utils.MovedPosition(submarine.Position, direction));


                int possiblePositions = ourPossiblePosition.Count;
                if (ourPossiblePosition.Count < 40)
                {
                    possiblePositions = PossibilitiesCalculator.ComputePossibleSubmarines(ourPossiblePosition, action).Count;
                }

                Console.Error.WriteLine(action + " " + areaSize + " pp:" + possiblePositions);

                if (areaSize > maxAreaLeft || (areaSize == maxAreaLeft && bestPossiblePositions < possiblePositions))
                {
                    maxAreaLeft = areaSize;
                    bestAction = action;
                    bestPossiblePositions = possiblePositions;
                }
            }
        }
        return bestAction;
    }
}

public class SearchAndDestroyStrategy : Strategy
{
    public SearchAndDestroyStrategy(Captain captain) : base(captain)
    { }
    public override string Decide()
    {
        List<Submarine> possibleFiringPosition = new List<Submarine>();

        if (captain.submarine.TorpedoCharge >= 2)
        {
            Dictionary<Submarine, string> actionToSub = new Dictionary<Submarine, string>();

            AddPossibleFiringPositions(possibleFiringPosition, actionToSub);

            if (possibleFiringPosition.Count > 0)
            {
                TargetPoint bestTarget = null;
                Submarine bestSub = null;
                foreach (Submarine sub in possibleFiringPosition)
                {
                    TargetPoint tar = PossibilitiesCalculator.BestTargetPoint(captain.possibleEnemyPosition, sub);
                    Console.Error.WriteLine("pos: " + sub.Position + " target:" + tar.Position + " " + tar.AverageDamage);

                    if (bestTarget == null || bestTarget.AverageDamage < tar.AverageDamage)
                    {
                        bestTarget = tar;
                        bestSub = sub;
                    }
                }

                Console.Error.WriteLine("Best target:" + bestTarget.Position + " " + bestTarget.AverageDamage);

                return GetAction(actionToSub, bestTarget, bestSub);
            }
        }

        //da espandere aggiungendo la possibilità di avvicinarsi a ottimi target futuri
        Strategy strategy = new InvisibleStrategy(captain);
        return strategy.Decide();

    }

    private string GetAction(Dictionary<Submarine, string> actionToSub, TargetPoint bestTarget, Submarine bestSub)
    {
        if (ActionsManager.TorpedoReach(bestSub.Position, bestTarget.Position))
        {
            //fire to target
            Console.Error.WriteLine("reachable target " + bestTarget.Position);

            string moveAction = actionToSub[bestSub];
            if (moveAction == "")
            {
                Strategy strategy = new InvisibleStrategy(captain);
                return "TORPEDO " + bestTarget.Position.X + " " + bestTarget.Position.Y + "|" + strategy.Decide();
            }
            else
            {
                return moveAction + " TORPEDO|TORPEDO " + bestTarget.Position.X + " " + bestTarget.Position.Y;
            }
        }
        else
        {
            //move to target
            //usare algoritmo A*
            //DA MODIFICARE
            Console.Error.WriteLine("Unreachable target " + bestTarget.Position);

            string moveAction = actionToSub[bestSub];
         //   if (moveAction == "")
        //    {
                Strategy strategy = new InvisibleStrategy(captain);
                return strategy.Decide();
        //    }
       //     else
       //     {
       //         return moveAction + " TORPEDO";
        //    }

        }
    }

    private void AddPossibleFiringPositions(List<Submarine> possibleFiringPosition, Dictionary<Submarine, string> actionToSub)
    {
        if (captain.submarine.TorpedoCharge == 3)
        {
            Submarine sub = new Submarine(captain.submarine);
            possibleFiringPosition.Add(sub);
            actionToSub[sub] = "";
        }
        foreach (string direction in Map.cardinalDirections)
        {
            string moveAction = "MOVE " + direction;
            if (ActionsManager.IsValidAction(captain.submarine, moveAction))
            {
                Submarine movedSub = new Submarine(captain.submarine);
                ActionsManager.Execute(movedSub, moveAction);
                possibleFiringPosition.Add(movedSub);
                actionToSub[movedSub] = moveAction;
            }
        }
    }
}


public static class Constants
{
    public static float ReducedValueForDistance = 0.03f;

    public static float DamageToSwitchStrategy = 0.9f;

    public static int LOW_NUMBER_POSSIBLE_POSITIONS = 25;

    public static float MIN_RELATIVE_INCREMENT_SILENT_ACTION = 2;
}
public class TargetPoint
{
    public TargetPoint(Position position, float averageDamage)
    {
        this.Position = position;
        this.AverageDamage = averageDamage;
    }

    public float AverageDamage { get; }

    public Position Position { get; }

    public override bool Equals(object obj)
    {
        return obj is TargetPoint point &&
               EqualityComparer<Position>.Default.Equals(Position, point.Position) &&
               AverageDamage == point.AverageDamage;
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode() + AverageDamage.GetHashCode();
    }

}



public static class PossibilitiesCalculator
{
    public static List<EnemySubmarine> GetPossibleSubmarines(List<EnemySubmarine> possibleSubmarines, string actions)
    {//get a set of submarines that are updated with the actions. if a submarine cannot do one of the actions, it will not be in the results
        string[] splittedActions = actions.Split('|');
        List<EnemySubmarine> updatedPossibilities = possibleSubmarines;

        foreach (string action in splittedActions)
        {
            Console.Error.WriteLine("UPDATED POSSIBILITIES before updating" + updatedPossibilities.Count + " ACTION:" + action);
            updatedPossibilities = ComputePossibleSubmarines(updatedPossibilities, action);
            Console.Error.WriteLine("UPDATED POSSIBILITIES " + updatedPossibilities.Count);
        }

        return updatedPossibilities;
    }
    public static List<EnemySubmarine> GetPossibleSubmarinesAfterEnemyActions(List<EnemySubmarine> possibleSubmarines, string enemyActions)
    {//get a set of submarines that are affected by the enemy actions. all the input submarines will be present in the output
        string[] splittedActions = enemyActions.Split('|');
        List<EnemySubmarine> updatedPossibilities = possibleSubmarines;
        foreach (string action in splittedActions)
        {
            updatedPossibilities = ComputePossibleSubmarines(updatedPossibilities, "SUFFER " + action);
        }

        return updatedPossibilities;
    }
    public static List<EnemySubmarine> GetAllPossibleSubmarine()
    {
        List<EnemySubmarine> possibleSubmarine = new List<EnemySubmarine>();
        for (int i = 0; i < Map.MAP_SIZE; i++)
        {
            for (int j = 0; j < Map.MAP_SIZE; j++)
            {
                Position pos = new Position(i, j);
                if (Map.Get(pos) == Map.GroundType.WATER)
                {
                    possibleSubmarine.Add(new EnemySubmarine(pos));
                }
            }
        }
        return possibleSubmarine;
    }

    /// <summary>
    /// Compute all possible submarines given the input submarines and the action
    /// </summary>
    /// <param name="submarines"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static List<EnemySubmarine> ComputePossibleSubmarines(List<EnemySubmarine> submarines, string action)
    {
        List<EnemySubmarine> possibleSubmarines = new List<EnemySubmarine>();
        int i = 0;
        foreach (EnemySubmarine submarine in submarines)
        {
            EnemySubmarine subCopy = new EnemySubmarine(submarine);
            i++;
            if (ActionsManager.IsValidAction(subCopy, action))
            {
                if (i % 4 == 0)
                    Console.Error.WriteLine(submarine.ToString() + " VALID ACTION : " + action + "  " + i);

                if (action.Substring(0, 4) == "MOVE")
                {
                    subCopy.Charge();
                }

                if (!HasUnkownEffets(action))
                {
                    ActionsManager.Execute(subCopy, action);
                    possibleSubmarines.Add(subCopy);
                }
                else
                {
                    List<EnemySubmarine> newPossibleSubmarines = GetPossibleSubmarinesAfterUnknownEffectsAction(subCopy, action);

                    possibleSubmarines.AddRange(newPossibleSubmarines);
                }
            }
            else
            {
                if (i % 4 == 0)
                    Console.Error.WriteLine(submarine.ToString() + " NOT VALID ACTION : " + action + "  " + i);

            }
        }

        return possibleSubmarines;
    }

    public static List<EnemySubmarine> GetPossibleSubmarinesAfterUnknownEffectsAction(EnemySubmarine submarine, string action)
    {//get a new set of submarines from actions that generate several possibilities (aka unknown effects)
        List<EnemySubmarine> possibleSubs = new List<EnemySubmarine>();

        if (action.Substring(0, 4) == "SILE")
        {

            AddPossibility(submarine, possibleSubs, "SILENCE N 0");

            foreach (string dir in Map.cardinalDirections)
            {
                for (int i = 1; i <= 4; i++)
                {
                    string a = "SILENCE " + dir + " " + i;

                    AddPossibility(submarine, possibleSubs, a);
                }


            }

        }


        return possibleSubs;
    }

    private static void AddPossibility(EnemySubmarine submarine, List<EnemySubmarine> possibleSubs, string a)
    {
        EnemySubmarine possibleSub = new EnemySubmarine(submarine);

        if (ActionsManager.IsValidAction(possibleSub, a))
        {
            ActionsManager.Execute(possibleSub, a);
            possibleSubs.Add(possibleSub);
        }
    }

    private static bool HasUnkownEffets(string action)
    {
        if (action.Substring(0, 4) == "SILE")
            return true;
        return false;
    }

    public static TargetPoint BestTargetPoint(PossibleSubmarines enemyPosition, Submarine ourSubmarine)
    {
        TargetPoint best = null;
        for (int i = 0; i < Map.MAP_SIZE; i++)
        {
            for (int j = 0; j < Map.MAP_SIZE; j++)
            {
                Position pos = new Position(i, j);
                float damage = DamageCreated(pos, enemyPosition, ourSubmarine);
                int distance = Utils.DistanceManhattan(pos, ourSubmarine.Position);
                if (distance > 4)
                {
                    damage -= Constants.ReducedValueForDistance * (distance - 2);
                }


                if (best == null || best.AverageDamage < damage)
                {
                    best = new TargetPoint(pos, damage);
                }
            }
        }

        return best;
    }

    public static float DamageCreated(Position targetPosition, PossibleSubmarines enemyPossibleSubmarine, Submarine ourSubmarine)
    {
        float damage = 0;

        foreach (Position pos in Utils.GetAllPositionsInArea(targetPosition, 1))
        {
            EnemySubmarine enemySubmarine = enemyPossibleSubmarine.Get(pos);
            if (enemySubmarine != null)
            {
                damage += DamageCreated(targetPosition, enemySubmarine, ourSubmarine);
            }
        }

        return damage / enemyPossibleSubmarine.Count;

    }
    public static float DamageCreated(Position targetPosition, Submarine enemySubmarine, Submarine ourSubmarine)
    {
        float damage = 0;

        int enemyDistance = Utils.DistanceLagrange(targetPosition, enemySubmarine.Position);
        if (enemyDistance == 1)
        {
            damage += 1;

        }
        if (enemyDistance == 0)
        {
            damage += Math.Min(2, enemySubmarine.Life);
        }

        int ourDistance = Utils.DistanceLagrange(targetPosition, ourSubmarine.Position);
        if (ourDistance == 1)
        {
            damage -= 1;
        }
        if (ourDistance == 0)
        {
            damage -= Math.Min(2, ourSubmarine.Life); ;
        }

        return damage;
    }


}

public static class Utils
{
    public static int AreaSize(Submarine submarine, Position position)
    {

        HashSet<Position> visited, toVisit;
        visited = new HashSet<Position>();
        toVisit = new HashSet<Position>();

        toVisit.Add(position);

        while (toVisit.Count > 0)
        {
            Position p = toVisit.First();
            toVisit.Remove(p);
            visited.Add(p);

            foreach (string direction in Map.cardinalDirections)
            {
                Position p1 = MovedPosition(p, direction);
                if (!visited.Contains(p1) && Utils.ValidMapPosition(p1) && !submarine.VisitedCells.Contains(p1))
                {
                    toVisit.Add(p1);
                }
            }
        }
        return visited.Count;
    }


    public static bool ValidMapPosition(Position position)
    {
        if (position.X < Map.MAP_SIZE && position.X >= 0 && position.Y < Map.MAP_SIZE && position.Y >= 0 && Map.Get(position) == Map.GroundType.WATER)
        {
            return true;
        }
        return false;
    }

    public static int MapSector(Position position)
    {
        int sector;

        sector = (position.X / 5 + 1) + (position.Y / 5) * 3;



        return sector;
    }
    public static int DistanceLagrange(Position p1, Position p2)
    {
        return Math.Max(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));

    }
    public static int DistanceManhattan(Position p1, Position p2)
    {
        return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);

    }
    public static Position MovedPosition(Position startingPosition, string direction)
    {

        if (direction == "N")
        {
            return new Position(startingPosition.X, startingPosition.Y - 1);
        }
        if (direction == "S")
        {
            return new Position(startingPosition.X, startingPosition.Y + 1);
        }
        if (direction == "E")
        {
            return new Position(startingPosition.X + 1, startingPosition.Y);
        }
        if (direction == "W")
        {
            return new Position(startingPosition.X - 1, startingPosition.Y);
        }
        throw new Exception("Unknown direction");
    }

    public static List<Position> GetAllPositionsInArea(Position center, int radius)
    {
        List<Position> positions = new List<Position>();
        int x = center.X;
        int y = center.Y;

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                Position p = new Position(x + i, y + j);
                if (ValidMapPosition(p))
                {
                    positions.Add(p);
                }
            }
        }
        return positions;
    }
   
    public static int DistanceDijkstra(Position startingPosition, Position target)
    {
        Dictionary<Position, int> distances = new Dictionary<Position, int>();
        Dictionary<Position, Position> precedent = new Dictionary<Position, Position>();
        SortedList<int, Position> Q = new SortedList<int, Position>(new DuplicateKeyComparer<int>());
        //initialize
        if (Map.Get(target) == Map.GroundType.LAND)
            return int.MaxValue;

        foreach (Position p in Utils.GetAllPositionsInArea(startingPosition, Utils.DistanceManhattan(startingPosition, target)))
        {
            if (Map.Get(p) == Map.GroundType.LAND)
                continue;

            precedent[p] = null;
            if (p == startingPosition)
            {
                Q.Add(0, p);

                distances[startingPosition] = 0;
            }
            else
            {
                Q.Add(int.MaxValue, p);

                distances[p] = int.MaxValue;
            }
        }


        //
        while (Q.Any())
        {
            var el = Q.First();
            Q.RemoveAt(0);

            if (el.Key == int.MaxValue)
                break;
            foreach (string d in Map.cardinalDirections)
            {
                Position neighbour = Utils.MovedPosition(el.Value, d);
                if (!distances.ContainsKey(neighbour))
                { continue; }

                int alt = el.Key + 1;
                if (alt < distances[neighbour])
                {
                    distances[neighbour] = alt;
                    precedent[neighbour] = el.Value;
                    Q.Remove(Q.IndexOfValue(neighbour));
                    Q.Add(alt, neighbour);
                }
            }

        }

        return distances[target];
    }
    public class DuplicateKeyComparer<TKey>
                :
             IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;   // Handle equality as beeing greater
            else
                return result;
        }

        #endregion
    }
}
public static class ActionsManager
{
    public static void Execute(Submarine submarine, string action)
    {
        string[] actions = action.Split('|');
        foreach (string a in actions)
        {

            if (a.Length < 4)
                throw new Exception("Unknown action");


            if (a.Substring(0, 4) == "SURF")
            {
                submarine.Surface();
            }

            if (a.Substring(0, 4) == "MOVE")
            {
                submarine.Move(a.Substring(5, 1));

                string[] s = a.Split(' ');
                if (s[s.Length - 1] == "TORPEDO")
                {
                    submarine.ChargeTorpedo();
                }

                if (s[s.Length - 1] == "SONAR")
                {
                    submarine.ChargeSonar();
                }

                if (s[s.Length - 1] == "SILENCE")
                {
                    submarine.ChargeSilence();
                }
                if (s[s.Length - 1] == "MINE")
                {
                    submarine.ChargeMine();
                }
            }


            if (a.Substring(0, 4) == "TORP")
            {
                submarine.FireTorpedo();
            }

            if (a.Substring(0, 4) == "SONA")
            {

                submarine.Sonar();
            }

            if (a.Substring(0, 4) == "SILE")
            {
                int distance = Convert.ToInt32(a.Substring(10, 1));
                submarine.Move(a.Substring(8, 1), distance);
                submarine.Silence();
            }

            if (a.Substring(0, 4) == "MINE")
            {
                string[] s = a.Split(' ');

                if (s.Length == 1)
                {
                    submarine.Mine(submarine.Position);
                }
                else
                {
                    string direction = s[1];

                    Position minePosition = Utils.MovedPosition(submarine.Position, direction);

                    submarine.Mine(minePosition);
                }
            }

            if (a.Substring(0, 4) == "TRIG")
            {
                string[] s = a.Split(' ');

                Position pos = new Position(Convert.ToInt32(s[1]), Convert.ToInt32(s[2]));

                submarine.Trigger(pos);
            }

            if (a.Substring(0, 4) == "SUFF")
            {
                //          Console.Error.WriteLine("SUFFERING: " + a);
                SufferAction(submarine, a.Replace("SUFFER ", ""));
            }
            else
            {
                SufferAction(submarine, a);
            }
        }
    }

    private static void SufferAction(Submarine submarine, string action)
    {
        string[] s = action.Split(' ');
        if (s[0].Substring(0, 4) == "TORP" || s[0].Substring(0, 4) == "TRIG")
        {
            Position pos = new Position(Convert.ToInt32(s[1]), Convert.ToInt32(s[2]));
            Explosion(pos, submarine);
        }
    }

    private static void Explosion(Position pos, Submarine submarine)
    {
        //    Console.Error.WriteLine("SUFFERING EXPLOSION " + pos + submarine.ToString());
        int dist = Utils.DistanceLagrange(pos, submarine.Position);
        int damage = 0;
        if (dist == 1) damage = 1;
        if (dist == 0) damage = 2;
        //     Console.Error.WriteLine("DAMAGE " + damage);
        submarine.TakeDamage(damage);
        //     Console.Error.WriteLine("sub" + submarine.ToString());
    }

    public static bool IsValidAction(Submarine submarine, string action)
    {
        if (action.Substring(0, 4) == "LIFE")
        {//used when we get to know how much life enemy has
            int amount = Convert.ToInt32(action.Substring(5, 1));
            if (amount == submarine.Life)
            {
                return true;
            }
            if (submarine is EnemySubmarine)
            {
                EnemySubmarine es = submarine as EnemySubmarine;
                foreach (int i in es.possibleLife)
                {
                    if (i == amount) return true;
                }
            }
            return false;
        }
        if (action.Substring(0, 4) == "SUFF")
        {//used when it's suffering effects
            return true;
        }

        if (action.Substring(0, 4) == "R_SO")
        {//used after sonar 
            string[] s = action.Split(' ');
            //R_SONAR 3 Y 
            //R_SONAR 7 N

            int submarineSector = Utils.MapSector(submarine.Position);

            int sonarSector = Convert.ToInt32(s[1]);

            if ((sonarSector != submarineSector && s[2] == "N") || (sonarSector == submarineSector && s[2] == "Y"))
            {
                return true;
            }

            return false;
        }

        if (action.Substring(0, 4) == "MOVE")
        {
            string direction = action.Substring(5, 1);
            Position movedPosition = Utils.MovedPosition(submarine.Position, direction);

            if (Utils.ValidMapPosition(movedPosition) && !submarine.VisitedCells.Contains(movedPosition))
            {
                return true;
            }
            return false;
        }
        if (action.Substring(0, 4) == "SURF")
        {
            if (submarine.Life <= 1)
                return false;

            if (action.Length >= 9)
            {
                int sector = Convert.ToInt32(action.Substring(8, action.Length - 8));

                if (Utils.MapSector(submarine.Position) == sector)
                    return true;
                return false;
            }
            return true;
        }

        if (action.Substring(0, 4) == "TORP")
        {
            if (submarine.TorpedoCharge < 3)
                return false;

            string[] s = action.Split(' ');

            Position pos = new Position(Convert.ToInt32(s[1]), Convert.ToInt32(s[2]));
            //UTILIZZARE A* PER CALCOLO PIU ACCURATO

            if (TorpedoReach(pos, submarine.Position))
                return true;
            return false;
        }

        if (action.Substring(0, 4) == "SONA")
        {
            if (submarine.SonarCharge < 4)
                return false;
            return true;
        }

        if (action.Substring(0, 4) == "SILE")
        {
            if (submarine.SilenceCharge == 6)
            {
                if (action == "SILENCE")
                    return true;
                string direction = action.Substring(8, 1);
                int distance = Convert.ToInt32(action.Substring(10, 1));
                Position movedPosition = submarine.Position;
                for (int i = 0; i < distance; i++)
                {
                    movedPosition = Utils.MovedPosition(movedPosition, direction);

                    if (!(Utils.ValidMapPosition(movedPosition) && !submarine.VisitedCells.Contains(movedPosition)))
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }
        if (action.Substring(0, 4) == "MINE")
        {
            if (submarine.MineCharge < 3)
                return false;

            string[] s = action.Split(' ');

            if (s.Length == 1)
                return true;


            string direction = s[1];

            Position minePosition = Utils.MovedPosition(submarine.Position, direction);
            if (Map.Get(minePosition) == Map.GroundType.WATER && !submarine.Mines.Contains(minePosition))
            {
                return true;
            }
            return false;
        }

        if (action.Substring(0, 4) == "TRIG")
        {
            string[] s = action.Split(' ');

            Position pos = new Position(Convert.ToInt32(s[1]), Convert.ToInt32(s[2]));


            if (submarine.Mines.Contains(pos))
                return true;
            return false;

        }
        throw new Exception("Unknown action");

    }

    public static bool TorpedoReach(Position startingPosition, Position target)
    {
        if (Utils.DistanceDijkstra(startingPosition, target) <= 4)
            return true;
        return false;
    }
}

