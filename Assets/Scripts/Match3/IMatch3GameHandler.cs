namespace Match3
{
    /// <summary>
    /// Interface for the view of the visible part of the game, which will accept map updates after it has been built,
    /// and updates of earned points.
    /// </summary>
    public interface IMatch3GameHandler
    {
        void MapUpdateHandler(int[,] map);
        void ScoreUpdateHandler(int currentScore, int earnedScore);
    }
}
