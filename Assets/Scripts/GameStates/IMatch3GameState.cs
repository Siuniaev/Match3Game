namespace GameStates
{
    /// <summary>
    /// The implementation of the State-pattern.
    /// </summary>
    public interface IMatch3GameState
    {
        void NextGameState(GameManager game);
        void MoveUnits(GameManager game);
    }
}
