namespace GameStates
{
    /// <summary>
    /// The state of matched units burning.
    /// </summary>
    public class StateBurn : IMatch3GameState
    {
        public void NextGameState(GameManager game)
        {
            game.State = Match3GameStates.Fall;

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
