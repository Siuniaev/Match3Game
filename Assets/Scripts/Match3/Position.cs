namespace Match3
{
    /// <summary>
    /// Units' coordinates.
    /// I did not use Unity's Vector2 so that I could later transfer the logic of calculating matches to the server for an online game.
    /// </summary>
    public struct Position
    {
        public int X;
        public int Y;

        public Position(int x, int y) : this()
        {
            X = x;
            Y = y;
        }
    }
}
