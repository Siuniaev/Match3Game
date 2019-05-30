namespace GameStates
{
    /// <summary>
    /// Pre-created game states.
    /// </summary>
    static class Match3GameStates
    {
        internal static StateWait Wait = new StateWait();
        internal static StateSwap Swap = new StateSwap();
        internal static StateShowMatch ShowMatch = new StateShowMatch();
        internal static StateBurn Burn = new StateBurn();
        internal static StateFall Fall = new StateFall();
        internal static StateFill Fill = new StateFill();
    }
}
