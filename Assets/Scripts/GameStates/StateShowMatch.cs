namespace GameStates
{
    /// <summary>
    /// The state of showing finded matches.
    /// </summary>    
    public class StateShowMatch : IMatch3GameState
    {
        public void NextGameState(GameManager game)
        {
            game.State = Match3GameStates.Burn;
        }

        public void MoveUnits(GameManager game)
        {
            //just wait            
        }
    }
}
