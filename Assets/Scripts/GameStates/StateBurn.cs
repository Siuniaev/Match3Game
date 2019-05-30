namespace GameStates
{
    /// <summary>
    /// The state of matched units burning.
    /// </summary>
    class StateBurn : IMatch3GameState
    {
        public void NextGameState(GameManager game)
        {
            game.SetGameState(Match3GameStates.Fall);
            
            foreach (UnitInfo unit in game.UnitsToDieAndReborn)
                unit.Reborn();
        }

        public void MoveUnits(GameManager game)
        {
            bool nobodyIsMoving = true;

            foreach (UnitInfo unit in game.UnitsToDieAndReborn)
                if (unit.Die(game.UnitSpeed))
                    nobodyIsMoving = false;

            if (nobodyIsMoving)
                NextGameState(game);
        }
    }
}
