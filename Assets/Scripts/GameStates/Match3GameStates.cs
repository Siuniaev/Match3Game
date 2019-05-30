namespace GameStates
{
    /// <summary>
    /// Pre-created game states.
    /// </summary>
    static class Match3GameStates
    {
        internal static readonly StateWait Wait = new StateWait();
        internal static readonly StateSwap Swap = new StateSwap();
        internal static readonly StateShowMatch ShowMatch = new StateShowMatch();
        internal static readonly StateBurn Burn = new StateBurn();
        internal static readonly StateFall Fall = new StateFall();
        internal static readonly StateFill Fill = new StateFill();
    }
}
