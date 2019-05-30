namespace GameStates
{
    /// <summary>
    /// The state of units falling.
    /// </summary>
    class StateFall : IMatch3GameState
    {
        public void NextGameState(GameManager game)
        {
            game.SetGameState(Match3GameStates.Fill);

            foreach (UnitInfo unit in game.UnitsToDieAndReborn)
                unit.ShowUnit();
        }

        public void MoveUnits(GameManager game)
        {
            bool nobodyIsMoving = true;

            foreach (UnitInfo unit in game.UnitsToMove)
                if (unit.Move(game.UnitSpeed * 2f)) //units must fall a little faster
                    nobodyIsMoving = false;

            if (nobodyIsMoving)
                NextGameState(game);
        }
    }
}
