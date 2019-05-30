namespace GameStates
{
    /// <summary>
    /// The state of waiting for player's activity.
    /// </summary>
    public class StateWait : IMatch3GameState
    {
        public void NextGameState(GameManager game)
        {
            //just wait
        }

        public void MoveUnits(GameManager game)
        {
            //just wait
        }
    }    
}
