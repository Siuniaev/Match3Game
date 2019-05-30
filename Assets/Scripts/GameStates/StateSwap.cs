namespace GameStates
{
    /// <summary>
    /// The state of units swapping.
    /// </summary>
    public class StateSwap : IMatch3GameState
    {
        public void NextGameState(GameManager game)
        {
            if (game.CheckForMatches())
            {
                game.State = Match3GameStates.ShowMatch;
                game.Invoke("NextState", 0.5f); //delay
            }
            else
                game.State = Match3GameStates.Wait;
        }

        public void MoveUnits(GameManager game)
        {
            bool nobodyIsMoving = true;

            foreach (UnitInfo unit in game.UnitsToMove)
                if (unit.Move(game.UnitSpeed))
                    nobodyIsMoving = false;

            if (nobodyIsMoving)
                NextGameState(game);
        }
    }
}
