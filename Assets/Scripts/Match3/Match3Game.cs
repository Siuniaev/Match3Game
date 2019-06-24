using System.Collections.Generic;
using System;

namespace Match3 {

    /// <summary>
    /// The main class of Match3 game.
    /// </summary>
    public class Match3Game : IDisposable
    {
        readonly int[,] Map;
        readonly int XMax;
        readonly int YMax;
        readonly int KindsCount;
        private int _score;
        public event Action<int[,]> OnUpdateMap;
        public event Action<int, int> OnUpdateScore;

        /// <summary>
        /// Сreate a new game.
        /// </summary>
        public Match3Game() : this(10, 10, 3, null) { }
        public Match3Game(int rows, int columns, int kinds, IMatch3GameHandler handler)
        {
            if (rows < 3 || columns < 3 || kinds < 3)                
                throw new Match3Exception($"New Match3Game Error: rows = {rows} ; columns = {columns}; kinds = {kinds}, values must be greater than 2.");

            XMax = columns; YMax = rows;
            KindsCount = kinds;
            _score = 0;
            Map = new int[XMax, YMax];

            if (handler != null)
            {
                OnUpdateMap += handler.MapUpdateHandler;
                OnUpdateScore += handler.ScoreUpdateHandler;
            }

            InitMap();
        }

        /// <summary>
        /// Filling the map with random units and clearing matches.
        /// </summary>
        private void InitMap()
        {
            var random = new Random();

            for (int x = 0; x < XMax; x++)
                for (int y = 0; y < YMax; y++)
                    Map[x, y] = random.Next(0, KindsCount);
            
            ClearMapForAllMatches();

            OnUpdateMap?.Invoke(Map);
        }

        /// <summary>
        /// Searching for matches and replacing units with new ones until there are no more matches.
        /// </summary>
        private void ClearMapForAllMatches()
        {
            var random = new Random();
            var needClear = true;

            while (needClear)
            {
                needClear = false;
                for (int x = 0; x < XMax; x++)
                {
                    for (int y = 0; y < YMax; y++)
                    {
                        var matches = GetMatches(new Position(x, y));
                        if (matches.Count > 0)
                        {
                            needClear = true;
                            var matchedUnits = RemoveMatches(matches, scoring: false); //no scoring

                            foreach (Position pos in matchedUnits)
                                Map[pos.X, pos.Y] = random.Next(0, KindsCount);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Swap specified units if this results in a match.
        /// </summary>
        /// <param name="firstPos"></param>
        /// <param name="secondPos"></param>
        /// <returns></returns>
        public bool TrySwapUnits(Position firstPos, Position secondPos)
        {
            bool ok = false;

            //distance check
            if ((firstPos.X == secondPos.X && Math.Abs(firstPos.Y - secondPos.Y) == 1) 
                || (firstPos.Y == secondPos.Y && Math.Abs(firstPos.X - secondPos.X) == 1))
            {
                SwapUnits(firstPos, secondPos);
                ok = CheckForExistingMatches(firstPos, secondPos);

                if (!ok) //if no match, swap back
                    SwapUnits(firstPos, secondPos);
            }

            return ok;
        }

        /// <summary>
        /// Swap specified units.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        private void SwapUnits(Position first, Position second)
        {
            int temp = Map[first.X, first.Y];
            Map[first.X, first.Y] = Map[second.X, second.Y];
            Map[second.X, second.Y] = temp;            
        }

        /// <summary>
        /// Find all matches moving recursively from specified points.
        /// </summary>
        /// <param name="poss"></param>
        /// <returns></returns>
        private List<List<Position>> GetMatches(params Position[] poss)
        {
            var allMatches = new List<List<Position>>();            

            foreach (Position unit in poss)
            {
                var horizontalMatches = new List<Position>();
                var verticalMatches = new List<Position>();

                int sample = Map[unit.X, unit.Y];                

                Fill(unit.X, unit.Y, -1, 0, sample, horizontalMatches);     //left
                Fill(unit.X + 1, unit.Y, 1, 0, sample, horizontalMatches);  //right
                Fill(unit.X, unit.Y, 0, 1, sample, verticalMatches);        //up
                Fill(unit.X, unit.Y - 1, 0, -1, sample, verticalMatches);   //down

                if (horizontalMatches.Count > 2)
                    allMatches.Add(horizontalMatches);

                if (verticalMatches.Count > 2)
                    allMatches.Add(verticalMatches);
            }

            return allMatches;
        }

        /// <summary>
        /// Recursive search for matches in a given direction from a specified point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sample"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="matches"></param>
        private void Fill(int x, int y, int xOffset, int yOffset, int sample, List<Position> matches)
        {
            if (x >= XMax || x < 0 || y >= YMax || y < 0 || matches == null || Map[x, y] != sample) return;

            matches.Add(new Position(x, y));
            Fill(x + xOffset, y + yOffset, xOffset, yOffset, sample, matches);
        }

        /// <summary>
        /// Check for existing matches from specified points.
        /// </summary>
        /// <param name="poss"></param>
        /// <returns></returns>
        private bool CheckForExistingMatches(params Position[] poss)
        {
            return GetMatches(poss).Count > 0;
        }

        /// <summary>
        /// Perform the main steps of the game line: find and remove matches, drop units into new empty spaces, fill new empty spaces with random units.
        /// </summary>
        /// <param name="matchedUnits"></param>
        /// <param name="fallingUnits"></param>
        /// <param name="newUnits"></param>
        /// <param name="poss"></param>
        public void DoMatchesPipeline(out List<Position> matchedUnits, out List<PositionVector> fallingUnits, out List<UnitData> newUnits, params Position[] poss)
        {
            var allMatches = GetMatches(poss);
            matchedUnits = RemoveMatches(allMatches);
            fallingUnits = FallDown();
            newUnits = FillEmptySpaces();

            if (matchedUnits != null && matchedUnits.Count != newUnits.Count)
                throw new Match3Exception($"FindAndRemoveMatches Error: matchedUnits.Count = {matchedUnits.Count} ; newUnits.Count = {newUnits.Count}; values must be equal.");
        }

        /// <summary>
        /// Remove matches found, optionally count points earned.
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="scoring"></param>
        /// <returns></returns>
        private List<Position> RemoveMatches(List<List<Position>> matches, bool scoring = true)
        {
            if (matches == null || matches.Count == 0) return null;
            
            int earnedScore = 0;
            var matchedUnits = new List<Position>();

            foreach (var match in matches)
            {
                earnedScore += (match.Count - 1) * 50;

                foreach (Position pos in match)
                {
                    if (!matchedUnits.Contains(pos))
                    {
                        Map[pos.X, pos.Y] = -1;
                        matchedUnits.Add(pos);
                    }
                }
            }

            if (scoring && earnedScore > 0)
            {
                _score += earnedScore;
                OnUpdateScore?.Invoke(_score, earnedScore);
            }

            return matchedUnits;
        }

        /// <summary>
        /// Drop units into empty spaces.
        /// </summary>
        /// <returns></returns>
        private List<PositionVector> FallDown()
        {
            var fallingUnits = new List<PositionVector>();

            //Along the rows we move up, on each row we move to the right. Finding emptiness, we "dump" the first element found above in this column.
            for (int y = 0; y < YMax; y++) 
                for (int x = 0; x < XMax; x++)
                    if (Map[x, y] == -1) 
                        for (int z = y + 1; z < YMax; z++)
                            if (Map[x, z] != -1)
                            {
                                fallingUnits.Add(new PositionVector(new Position(x, z), new Position(x, y))); //Collect a list of falling units coordinates (start and end of the fall).
                                Map[x, y] = Map[x, z];
                                Map[x, z] = -1;
                                break;
                            }

            return fallingUnits;
        }

        /// <summary>
        /// Fill empty spaces with random units.
        /// </summary>
        /// <returns></returns>
        private List<UnitData> FillEmptySpaces()
        {
            var r = new Random();
            var newUnits = new List<UnitData>();

            for (int x = 0; x < XMax; x++)
                for (int y = 0; y < YMax; y++)
                    if (Map[x, y] == -1)
                    {
                        Map[x, y] = r.Next(0, KindsCount);
                        newUnits.Add(new UnitData(Map[x, y], new Position(x, y))); //Collect a list of new units' data.
                    }

            return newUnits;
        }

        #region IDisposable
        private bool disposedValue = false;

        /// <summary>
        /// Unsubscribing from the events when the instance is destroyed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
            {
                if (OnUpdateMap != null)
                    foreach (Action<int[,]> act in OnUpdateMap.GetInvocationList())
                        OnUpdateMap -= act;

                if (OnUpdateScore != null)
                    foreach (Action<int, int> act in OnUpdateScore.GetInvocationList())
                        OnUpdateScore -= act;
            }

            disposedValue = true;            
        }

        ~Match3Game()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }       
}
