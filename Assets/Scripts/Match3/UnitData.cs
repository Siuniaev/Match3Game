namespace Match3
{
    /// <summary>
    /// Unit data.
    /// </summary>
    public struct UnitData
    {
        public int Id;
        public Position Pos;

        public UnitData(int id, Position pos) : this()
        {
            Id = id;
            Pos = pos;
        }
    }
}
