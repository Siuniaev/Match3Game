namespace GameStates
{
    /// <summary>
    /// The implementation of the State-pattern.
    /// </summary>
    interface IMatch3GameState
    {
        void NextGameState(GameManager game);
        void MoveUnits(GameManager game);
    }
}
