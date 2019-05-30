namespace GameStates
{
    /// <summary>
    /// The state of showing finded matches.
    /// </summary>    
    class StateShowMatch : IMatch3GameState
    {
        public void NextGameState(GameManager game)
        {
            game.SetGameState(Match3GameStates.Burn);
        }

        public void MoveUnits(GameManager game)
        {
            //just wait            
        }
    }
}
