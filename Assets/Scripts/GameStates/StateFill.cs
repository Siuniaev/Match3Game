namespace GameStates
{
    /// <summary>
    /// The state of units filling empty spaces.
    /// </summary>
    class StateFill : IMatch3GameState
    {
        public void NextGameState(GameManager game)
        {
            game.UnitsToDieAndReborn.Clear();

            if (game.CheckForMatches())
            {
                game.SetGameState(Match3GameStates.ShowMatch);
                game.Invoke("NextState", 0.5f); //delay
            }
            else
                game.SetGameState(Match3GameStates.Wait);
        }

        public void MoveUnits(GameManager game)
        {
            bool nobodyIsMoving = true;

            foreach (UnitInfo unit in game.UnitsToDieAndReborn)
                if (unit.Born(game.UnitSpeed))
                    nobodyIsMoving = false;

            if (nobodyIsMoving)
                NextGameState(game);
        }
    }
}
