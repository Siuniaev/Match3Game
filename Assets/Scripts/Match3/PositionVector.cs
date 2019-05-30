namespace Match3
{
    /// <summary>
    /// Two unit coordinates defining the movement of a unit.
    /// </summary>
    public struct PositionVector
    {
        public Position From;
        public Position To;

        public PositionVector(Position from, Position to) : this()
        {
            From = from;
            To = to;
        }
    }
}
